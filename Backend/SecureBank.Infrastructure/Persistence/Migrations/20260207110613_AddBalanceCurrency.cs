using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureBank.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBalanceCurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bank_accounts_customer_profiles_customer_profile_id",
                table: "bank_accounts");

            migrationBuilder.DropTable(
                name: "payment_cards");

            migrationBuilder.DropCheckConstraint(
                name: "CK_transactions_amount",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_bank_accounts_customer_profile_id",
                table: "bank_accounts");

            migrationBuilder.DropCheckConstraint(
                name: "CK_bank_accounts_balance",
                table: "bank_accounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_customer_profiles",
                table: "customer_profiles");

            migrationBuilder.DropColumn(
                name: "counterparty_iban",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "created_by_user_id",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "from_account_id",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "note",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "reference_id",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "type",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "customer_profile_id",
                table: "bank_accounts");

            migrationBuilder.DropColumn(
                name: "row_version",
                table: "bank_accounts");

            migrationBuilder.RenameTable(
                name: "customer_profiles",
                newName: "users");

            migrationBuilder.RenameColumn(
                name: "to_account_id",
                table: "transactions",
                newName: "payment_link_id");

            migrationBuilder.RenameColumn(
                name: "amount",
                table: "transactions",
                newName: "transfer_fee");

            migrationBuilder.RenameColumn(
                name: "account_type",
                table: "bank_accounts",
                newName: "type");

            migrationBuilder.AddColumn<string>(
                name: "currency",
                table: "transactions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "receiver_balance_after",
                table: "transactions",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "receiver_balance_before",
                table: "transactions",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "receiver_id",
                table: "transactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "sender_balance_after",
                table: "transactions",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "sender_balance_before",
                table: "transactions",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "sender_id",
                table: "transactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "transfer_amount",
                table: "transactions",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "balance_currency",
                table: "bank_accounts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "bank_accounts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "bank_accounts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "last_transfer_receiver_id",
                table: "bank_accounts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_transfer_time",
                table: "bank_accounts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "max_balance",
                table: "bank_accounts",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "max_transfer",
                table: "bank_accounts",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "min_balance",
                table: "bank_accounts",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "min_transfer",
                table: "bank_accounts",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "bank_accounts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "transfer_fee",
                table: "bank_accounts",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "bank_accounts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "national_id_number",
                table: "users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "city",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "province",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddUniqueConstraint(
                name: "AK_users_user_id",
                table: "users",
                column: "user_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                column: "id");

            migrationBuilder.CreateTable(
                name: "payment_links",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "text", nullable: false),
                    merchant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_name = table.Column<string>(type: "text", nullable: false),
                    product_description = table.Column<string>(type: "text", nullable: true),
                    product_image_url = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_links", x => x.id);
                    table.ForeignKey(
                        name: "FK_payment_links_bank_accounts_merchant_id",
                        column: x => x.merchant_id,
                        principalTable: "bank_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_transactions_payment_link_id",
                table: "transactions",
                column: "payment_link_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_receiver_id",
                table: "transactions",
                column: "receiver_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_sender_id",
                table: "transactions",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "IX_bank_accounts_owner_user_id",
                table: "bank_accounts",
                column: "owner_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_user_id",
                table: "users",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payment_links_merchant_id",
                table: "payment_links",
                column: "merchant_id");

            migrationBuilder.AddForeignKey(
                name: "FK_bank_accounts_users_owner_user_id",
                table: "bank_accounts",
                column: "owner_user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_bank_accounts_receiver_id",
                table: "transactions",
                column: "receiver_id",
                principalTable: "bank_accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_bank_accounts_sender_id",
                table: "transactions",
                column: "sender_id",
                principalTable: "bank_accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_payment_links_payment_link_id",
                table: "transactions",
                column: "payment_link_id",
                principalTable: "payment_links",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bank_accounts_users_owner_user_id",
                table: "bank_accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_bank_accounts_receiver_id",
                table: "transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_bank_accounts_sender_id",
                table: "transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_payment_links_payment_link_id",
                table: "transactions");

            migrationBuilder.DropTable(
                name: "payment_links");

            migrationBuilder.DropIndex(
                name: "IX_transactions_payment_link_id",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_transactions_receiver_id",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_transactions_sender_id",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_bank_accounts_owner_user_id",
                table: "bank_accounts");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_users_user_id",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_user_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "currency",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "receiver_balance_after",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "receiver_balance_before",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "receiver_id",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "sender_balance_after",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "sender_balance_before",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "sender_id",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "transfer_amount",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "balance_currency",
                table: "bank_accounts");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "bank_accounts");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "bank_accounts");

            migrationBuilder.DropColumn(
                name: "last_transfer_receiver_id",
                table: "bank_accounts");

            migrationBuilder.DropColumn(
                name: "last_transfer_time",
                table: "bank_accounts");

            migrationBuilder.DropColumn(
                name: "max_balance",
                table: "bank_accounts");

            migrationBuilder.DropColumn(
                name: "max_transfer",
                table: "bank_accounts");

            migrationBuilder.DropColumn(
                name: "min_balance",
                table: "bank_accounts");

            migrationBuilder.DropColumn(
                name: "min_transfer",
                table: "bank_accounts");

            migrationBuilder.DropColumn(
                name: "status",
                table: "bank_accounts");

            migrationBuilder.DropColumn(
                name: "transfer_fee",
                table: "bank_accounts");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "bank_accounts");

            migrationBuilder.DropColumn(
                name: "address",
                table: "users");

            migrationBuilder.DropColumn(
                name: "city",
                table: "users");

            migrationBuilder.DropColumn(
                name: "province",
                table: "users");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "users");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "customer_profiles");

            migrationBuilder.RenameColumn(
                name: "transfer_fee",
                table: "transactions",
                newName: "amount");

            migrationBuilder.RenameColumn(
                name: "payment_link_id",
                table: "transactions",
                newName: "to_account_id");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "bank_accounts",
                newName: "account_type");

            migrationBuilder.AddColumn<string>(
                name: "counterparty_iban",
                table: "transactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by_user_id",
                table: "transactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "from_account_id",
                table: "transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "note",
                table: "transactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "reference_id",
                table: "transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "type",
                table: "transactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "customer_profile_id",
                table: "bank_accounts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<byte[]>(
                name: "row_version",
                table: "bank_accounts",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AlterColumn<string>(
                name: "national_id_number",
                table: "customer_profiles",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_customer_profiles",
                table: "customer_profiles",
                column: "id");

            migrationBuilder.CreateTable(
                name: "payment_cards",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    bank_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    billing_address = table.Column<string>(type: "text", nullable: false),
                    card_holder_name = table.Column<string>(type: "text", nullable: false),
                    card_number_last4 = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    exp_month = table.Column<int>(type: "integer", nullable: false),
                    exp_year = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_cards", x => x.id);
                    table.ForeignKey(
                        name: "FK_payment_cards_bank_accounts_bank_account_id",
                        column: x => x.bank_account_id,
                        principalTable: "bank_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_transactions_amount",
                table: "transactions",
                sql: "amount > 0");

            migrationBuilder.CreateIndex(
                name: "IX_bank_accounts_customer_profile_id",
                table: "bank_accounts",
                column: "customer_profile_id");

            migrationBuilder.AddCheckConstraint(
                name: "CK_bank_accounts_balance",
                table: "bank_accounts",
                sql: "balance >= 0");

            migrationBuilder.CreateIndex(
                name: "IX_payment_cards_bank_account_id",
                table: "payment_cards",
                column: "bank_account_id");

            migrationBuilder.AddForeignKey(
                name: "FK_bank_accounts_customer_profiles_customer_profile_id",
                table: "bank_accounts",
                column: "customer_profile_id",
                principalTable: "customer_profiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
