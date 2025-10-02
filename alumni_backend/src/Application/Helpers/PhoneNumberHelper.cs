using System.Text.RegularExpressions;

namespace Application.Helpers;

/// <summary>
/// Helper class for Thai mobile phone number validation and normalization
/// Supports various Thai mobile number formats and converts them to standard format
/// </summary>
public static class PhoneNumberHelper
{
    private static readonly Regex DigitsOnlyRegex = new(@"[^\d]", RegexOptions.Compiled);
    
    /// <summary>
    /// Normalizes a Thai mobile phone number to international format (66xxxxxxxxx)
    /// </summary>
    /// <param name="phoneNumber">Input phone number in various formats</param>
    /// <returns>Normalized phone number in international format 66xxxxxxxxx</returns>
    /// <exception cref="ArgumentException">Thrown when phone number format is invalid</exception>
    public static string NormalizeMobilePhone(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber)) 
            return string.Empty;
        
        // Remove all non-digits
        string digits = DigitsOnlyRegex.Replace(phoneNumber, "");
        
        // Handle Thai mobile number formats
        if (digits.StartsWith("66")) // +66 format (international)
        {
            digits = "0" + digits.Substring(2);
        }
        else if (digits.Length == 9 && !digits.StartsWith("0")) // 9-digit without leading 0
        {
            digits = "0" + digits;
        }
        
        // Validate Thai mobile number format (0x-xxxx-xxxx where x is 6, 8, or 9)
        if (digits.Length == 10 && 
            digits.StartsWith("0") && 
            (digits.StartsWith("06") || digits.StartsWith("08") || digits.StartsWith("09")))
        {
            // Convert to international format (66xxxxxxxxx)
            return "66" + digits.Substring(1);
        }
        
        throw new ArgumentException($"Invalid Thai mobile phone number format: {phoneNumber}. Expected format: 06xxxxxxxx, 08xxxxxxxx, or 09xxxxxxxx");
    }
    
    /// <summary>
    /// Validates if a phone number is a valid Thai mobile phone number
    /// </summary>
    /// <param name="phoneNumber">Phone number to validate</param>
    /// <returns>True if valid Thai mobile phone number, false otherwise</returns>
    public static bool IsValidThaiMobilePhone(string phoneNumber)
    {
        try
        {
            var normalized = NormalizeMobilePhone(phoneNumber);
            return !string.IsNullOrEmpty(normalized);
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// Formats a normalized phone number for display (0x-xxxx-xxxx)
    /// </summary>
    /// <param name="normalizedPhone">Normalized phone number (10 digits starting with 0)</param>
    /// <returns>Formatted phone number for display</returns>
    public static string FormatForDisplay(string normalizedPhone)
    {
        if (string.IsNullOrWhiteSpace(normalizedPhone) || normalizedPhone.Length != 10)
            return normalizedPhone;
            
        return $"{normalizedPhone.Substring(0, 2)}-{normalizedPhone.Substring(2, 4)}-{normalizedPhone.Substring(6, 4)}";
    }
    
    /// <summary>
    /// Gets the mobile network operator based on the phone number prefix
    /// </summary>
    /// <param name="normalizedPhone">Normalized phone number</param>
    /// <returns>Network operator name</returns>
    public static string GetNetworkOperator(string normalizedPhone)
    {
        if (string.IsNullOrWhiteSpace(normalizedPhone) || normalizedPhone.Length != 10)
            return "Unknown";
            
        return normalizedPhone.Substring(0, 3) switch
        {
            "081" or "080" => "AIS",
            "082" or "083" or "084" or "085" => "DTAC", 
            "086" or "087" => "True Move",
            "089" => "CAT Telecom",
            "090" or "091" or "092" or "093" or "094" => "AIS",
            "095" or "096" or "097" or "098" => "DTAC",
            "099" => "True Move",
            "061" or "062" or "063" => "AIS",
            "064" or "065" => "DTAC",
            "066" => "True Move",
            _ => "Unknown"
        };
    }
}