using SecureBank.Application.Common.Interfaces;
using SecureBank.Infrastructure.Persistence;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace SecureBank.Infrastructure.Services;

/// <summary>
/// Generates valid IBANs following the ISO 13616 standard.
/// Uses Iraq (IQ) country code with BABI bank code.
/// Format: IQkk BBBB CCCC CCCC CCCC CCC (23 characters)
/// - kk: Check digits (2)
/// - BBBB: Bank code (4)
/// - CCCC...: Account number (15)
/// </summary>
public class IbanGenerator : IIbanGenerator
{
    private readonly ApplicationDbContext _db;
    
    // Iraq IBAN: IQkk BBBB SSSS AAAA AAAA AAA
    // Country code: IQ
    // Bank code: BABI (Bank of Baghdad example)
    private const string CountryCode = "IQ";
    private const string BankCode = "BABI";
    private const int AccountNumberLength = 15;

    public IbanGenerator(ApplicationDbContext db)
    {
        _db = db;
    }

    public string Generate()
    {
        string candidate;
        int attempts = 0;
        const int maxAttempts = 100;

        do
        {
            if (attempts++ > maxAttempts)
                throw new InvalidOperationException("Unable to generate unique IBAN after maximum attempts.");

            // Generate random account number (15 digits)
            var accountNumber = GenerateRandomAccountNumber(AccountNumberLength);
            
            // Calculate check digits using MOD-97 algorithm
            var checkDigits = CalculateCheckDigits(CountryCode, BankCode + accountNumber);
            
            // Compose final IBAN
            candidate = $"{CountryCode}{checkDigits:D2}{BankCode}{accountNumber}";
            
        } while (_db.BankAccounts.Any(a => a.IBAN == candidate));

        return candidate;
    }

    /// <summary>
    /// Generates a random numeric account number of specified length
    /// </summary>
    private static string GenerateRandomAccountNumber(int length)
    {
        var sb = new StringBuilder(length);
        var bytes = new byte[length];
        RandomNumberGenerator.Fill(bytes);
        
        for (int i = 0; i < length; i++)
        {
            sb.Append((bytes[i] % 10).ToString());
        }
        
        return sb.ToString();
    }

    /// <summary>
    /// Calculates IBAN check digits using MOD-97-10 algorithm (ISO 7064)
    /// </summary>
    private static int CalculateCheckDigits(string countryCode, string bban)
    {
        // 1. Move country code and "00" to end: BBAN + CountryCode + "00"
        var rearranged = bban + countryCode + "00";
        
        // 2. Convert letters to numbers (A=10, B=11, ..., Z=35)
        var numericString = ConvertToNumeric(rearranged);
        
        // 3. Calculate MOD 97 and subtract from 98
        var bigNum = BigInteger.Parse(numericString);
        var remainder = (int)(bigNum % 97);
        var checkDigits = 98 - remainder;
        
        return checkDigits;
    }

    /// <summary>
    /// Converts letters to numbers for MOD-97 calculation (A=10, B=11, ..., Z=35)
    /// </summary>
    private static string ConvertToNumeric(string input)
    {
        var sb = new StringBuilder();
        foreach (var c in input.ToUpperInvariant())
        {
            if (char.IsLetter(c))
            {
                // A=10, B=11, ..., Z=35
                sb.Append((c - 'A' + 10).ToString());
            }
            else
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// Validates an IBAN using the MOD-97-10 algorithm
    /// </summary>
    public static bool ValidateIban(string iban)
    {
        if (string.IsNullOrWhiteSpace(iban) || iban.Length < 15)
            return false;

        // Remove spaces and convert to uppercase
        iban = iban.Replace(" ", "").ToUpperInvariant();

        // Move first 4 characters to end
        var rearranged = iban[4..] + iban[..4];
        
        // Convert to numeric
        var numericString = ConvertToNumeric(rearranged);
        
        // Check if MOD 97 == 1
        var bigNum = BigInteger.Parse(numericString);
        return bigNum % 97 == 1;
    }
}
