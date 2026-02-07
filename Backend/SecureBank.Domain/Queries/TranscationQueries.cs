namespace SecureBank.Domain.Queries;

using Microsoft.EntityFrameworkCore;
using SecureBank.Domain.Entities;

public enum TransferErrCode
{
    TransferAmountTooLow,
    SenderUnauthorized,
    ReceiverUnauthorized,
    DoubleSpendingDetected,
    SenderInsufficientFunds,
    MaxTransferExceeded,
    MinTransferNotReached,
    ExceededReceiverMaxBalance,
    PaymentLinkNotFound,
    RetryAgain,
    InvalidAmount,
    CurrencyConversionRequired
}

public class TransferException(TransferErrCode err_code) : Exception
{
    public TransferErrCode ErrCode = err_code;
}

public class TransactionsQueries(dynamic context)
{
    private static readonly string treasury_iban = "IQ20BABI701008888816342";

    public async Task Transfer(
        string sender_account_IBAN, string receiver_account_IBAN,
        decimal amount, string currency)
    {
        // Capturing transfer operation time (one source of truth for time)
        DateTime transaction_time = DateTime.UtcNow;

        // No negative balance
        if (amount <= 0)
        {
            throw new TransferException(TransferErrCode.InvalidAmount);
        }

        // Begin transaction with Serializable isolation level
        using var transaction = await context.Database.BeginTransactionAsync(
            System.Data.IsolationLevel.Serializable);

        try
        {
            // Check account validity
            BankAccount? sender_account = await context.BankAccounts.FromSql(
                $""" select * from "bank_accounts" where "iban"={sender_account_IBAN} AND "status"=1; """)
                .FirstOrDefaultAsync() ??
                throw new TransferException(TransferErrCode.SenderUnauthorized);

            BankAccount? receiver_account = await context.BankAccounts.FromSql(
                $""" select * from "bank_accounts" where "iban"={receiver_account_IBAN} AND "status"=1; """)
                .FirstOrDefaultAsync() ??
                throw new TransferException(TransferErrCode.ReceiverUnauthorized);

            // checking for currency mismatch
            if (sender_account.BalanceCurrency != receiver_account.BalanceCurrency)
            {
                throw new TransferException(TransferErrCode.CurrencyConversionRequired);
            }

            // Checking if transfer amount is authorized
            if (amount > sender_account.MaxTransfer)
            {
                throw new TransferException(TransferErrCode.MaxTransferExceeded);
            }

            if (amount < sender_account.MinTransfer)
            {
                throw new TransferException(TransferErrCode.TransferAmountTooLow);
            }

            // Double spending prevention
            if (sender_account.LastTransferReceiverId.HasValue && sender_account.LastTransferTime.HasValue)
            {
                DateTime transfer_timeout = sender_account.LastTransferTime.Value + TimeSpan.FromMinutes(5);

                if (sender_account.LastTransferReceiverId == receiver_account.Id && transaction_time < transfer_timeout)
                {
                    throw new TransferException(TransferErrCode.DoubleSpendingDetected);
                }
            }

            // Balance calculation
            decimal sender_balance_after_transfer = sender_account.Balance - amount;
            decimal transfer_fee = amount * receiver_account.TransferFee;
            decimal receiver_balance_after_transfer_and_fee = receiver_account.Balance + amount - transfer_fee;

            // Balance Limits check
            if (sender_balance_after_transfer < sender_account.MinBalance)
            {
                throw new TransferException(TransferErrCode.SenderInsufficientFunds);
            }

            if (receiver_balance_after_transfer_and_fee > receiver_account.MaxBalance)
            {
                throw new TransferException(TransferErrCode.ExceededReceiverMaxBalance);
            }

            // Generate new transaction ID
            var transaction_id = Guid.NewGuid();

            // Inserting transaction
            context.Database.ExecuteSql($"""
                insert into "transactions" 
                    (
                        "id",
                        "sender_balance_before","receiver_balance_before", 
                        "sender_balance_after", "receiver_balance_after", 
                        "transfer_amount","transfer_fee",
                        "sender_id", "receiver_id",
                        "created_at",
                        "currency"
                    )
                VALUES
                    (
                        {transaction_id},
                        {sender_account.Balance}, {receiver_account.Balance},
                        {sender_balance_after_transfer}, {receiver_balance_after_transfer_and_fee},
                        {amount}, {transfer_fee},
                        {sender_account.Id}, {receiver_account.Id},
                        {transaction_time},
                        {currency}
                    )
                ;
            """);

            // Updating sender account information
            context.Database.ExecuteSql($"""
                update "bank_accounts"
                SET
                    "balance" = {sender_balance_after_transfer},
                    "last_transfer_receiver_id" = {receiver_account.Id},
                    "last_transfer_time" = {transaction_time}
                WHERE
                    "iban" = {sender_account_IBAN}
                ;
            """);

            // Updating receiver account information
            context.Database.ExecuteSql($"""
                update "bank_accounts" 
                SET
                    "balance" = {receiver_balance_after_transfer_and_fee}
                WHERE
                    "iban" = {receiver_account_IBAN}
                ;
            """);

            // Commit the transaction
            await transaction.CommitAsync();
        }
        catch (Exception ex) when (ex.GetType().Name == "PostgresException" && ex.GetType().GetProperty("SqlState")?.GetValue(ex) as string == "40001")
        {
            // Serialization failure - transaction conflict
            await transaction.RollbackAsync();
            throw new TransferException(TransferErrCode.RetryAgain);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task PayPaymentLink(string sender_account_IBAN, string receiver_account_IBAN, decimal amount, Guid payment_link_id, string currency)
    {
        // Capturing transfer operation time (one source of truth for time)
        DateTime transaction_time = DateTime.UtcNow;

        // Begin transaction with Serializable isolation level
        using var transaction = await context.Database.BeginTransactionAsync(
            System.Data.IsolationLevel.Serializable);

        try
        {
            // Check account validity
            BankAccount? sender_account = await context.BankAccounts.FromSql(
                $""" select * from "bank_accounts" where "iban"={sender_account_IBAN} AND "status"=1; """)
                .FirstOrDefaultAsync() ??
                throw new TransferException(TransferErrCode.SenderUnauthorized);

            BankAccount? receiver_account = await context.BankAccounts.FromSql(
                $""" select * from "bank_accounts" where "iban"={receiver_account_IBAN} AND "status"=1; """)
                .FirstOrDefaultAsync() ??
                throw new TransferException(TransferErrCode.ReceiverUnauthorized);

            PaymentLink? payment_link = await context.PaymentLinks.FromSql(
                $""" select * from "payment_links" where "id"={payment_link_id} AND "is_deleted"=false; """)
                .FirstOrDefaultAsync() ?? 
                throw new TransferException(TransferErrCode.PaymentLinkNotFound);

            // checking for currency mismatch
            if (sender_account.BalanceCurrency != receiver_account.BalanceCurrency)
            {
                throw new TransferException(TransferErrCode.CurrencyConversionRequired);
            }

            // check if payment link belongs to the merchant
            if (receiver_account.Id != payment_link.MerchantId)
            {
                throw new TransferException(TransferErrCode.ReceiverUnauthorized);
            }

            // Checking if transfer amount is authorized
            if (amount > sender_account.MaxTransfer)
            {
                throw new TransferException(TransferErrCode.MaxTransferExceeded);
            }

            if (amount < sender_account.MinTransfer)
            {
                throw new TransferException(TransferErrCode.TransferAmountTooLow);
            }

            // Double spending prevention
            if (sender_account.LastTransferReceiverId.HasValue && sender_account.LastTransferTime.HasValue)
            {
                DateTime transfer_timeout = sender_account.LastTransferTime.Value + TimeSpan.FromMinutes(5);

                if (sender_account.LastTransferReceiverId == receiver_account.Id && transaction_time < transfer_timeout)
                {
                    throw new TransferException(TransferErrCode.DoubleSpendingDetected);
                }
            }

            // Balance calculation
            decimal sender_balance_after_transfer = sender_account.Balance - amount;
            decimal transfer_fee = amount * receiver_account.TransferFee;
            decimal receiver_balance_after_transfer_and_fee = receiver_account.Balance + amount - transfer_fee;

            // Balance Limits check
            if (sender_balance_after_transfer < sender_account.MinBalance)
            {
                throw new TransferException(TransferErrCode.SenderInsufficientFunds);
            }

            if (receiver_balance_after_transfer_and_fee > receiver_account.MaxBalance)
            {
                throw new TransferException(TransferErrCode.ExceededReceiverMaxBalance);
            }

            // Generate new transaction ID
            var transaction_id = Guid.NewGuid();

            // Inserting transaction
            context.Database.ExecuteSql($"""
                insert into "transactions" 
                    (
                        "id",
                        "sender_balance_before","receiver_balance_before", 
                        "sender_balance_after", "receiver_balance_after", 
                        "transfer_amount","transfer_fee",
                        "sender_id", "receiver_id",
                        "created_at",
                        "payment_link_id",
                        "currency"
                    )
                VALUES
                    (
                        {transaction_id},
                        {sender_account.Balance}, {receiver_account.Balance},
                        {sender_balance_after_transfer}, {receiver_balance_after_transfer_and_fee},
                        {amount}, {transfer_fee},
                        {sender_account.Id}, {receiver_account.Id},
                        {transaction_time},
                        {payment_link_id},
                        {currency}
                    )
                ;
            """);

            // Updating sender account information
            context.Database.ExecuteSql($"""
                update "bank_accounts"
                SET
                    "balance" = {sender_balance_after_transfer},
                    "last_transfer_receiver_id" = {receiver_account.Id},
                    "last_transfer_time" = {transaction_time}
                WHERE
                    "iban" = {sender_account_IBAN}
                ;
            """);

            // Updating receiver account information
            context.Database.ExecuteSql($"""
                update "bank_accounts" 
                SET
                    "balance" = {receiver_balance_after_transfer_and_fee}
                WHERE
                    "iban" = {receiver_account_IBAN}
                ;
            """);

            // Commit the transaction
            await transaction.CommitAsync();
        }
        catch (Exception ex) when (ex.GetType().Name == "PostgresException" && ex.GetType().GetProperty("SqlState")?.GetValue(ex) as string == "40001")
        {
            // Serialization failure - transaction conflict
            await transaction.RollbackAsync();
            throw new TransferException(TransferErrCode.RetryAgain);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task Deposit(string receiver_account_IBAN, decimal amount, string currency)
    {
        await this.Transfer(TransactionsQueries.treasury_iban, receiver_account_IBAN, amount, currency);
    }

    public async Task Withdraw(string receiver_account_IBAN, decimal amount, string currency)
    {
        await this.Transfer(receiver_account_IBAN, TransactionsQueries.treasury_iban, amount, currency);
    }
}
