using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace HealthCareAB_v1.DTOs.Auth;

[ExcludeFromCodeCoverage]
public class RegisterPatientDto
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(
        50,
        MinimumLength = 3,
        ErrorMessage = "Username must be between 3 and 50 characters"
    )]
    public required string Username { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
    [PasswordIncludesUppercase]
    [PasswordIncludesLowercase]
    [PasswordIncludeDigit]
    [PasswordIncludeSpecialCharacter]
    public required string Password { get; set; }
}

[ExcludeFromCodeCoverage]
public class RegisterCaregiverDto
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(
        50,
        MinimumLength = 3,
        ErrorMessage = "Username must be between 3 and 50 characters"
    )]
    public required string Username { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
    [PasswordIncludesUppercase]
    [PasswordIncludesLowercase]
    [PasswordIncludeDigit]
    [PasswordIncludeSpecialCharacter]
    public required string Password { get; set; }

    /// // <summary>
    /// Optional roles for the new user.
    /// Note: Admin role can be assigned manually through Swagger. This is ok in dev, in the future this should
    /// be changed to a more solid sulotion. For now you can leave it as it is if you want.
    /// Non-admin requests with Admin role will be ignored (defaults to User).
    /// </summary>
    public List<string> Roles { get; set; } = [];
}

[ExcludeFromCodeCoverage]
public class PasswordIncludesUppercaseAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        var password = value as string;

        return !string.IsNullOrWhiteSpace(password) && password.Any(char.IsUpper);
    }

    public override string FormatErrorMessage(string name)
    {
        return "Password must include at least one uppercase character.";
    }
}

[ExcludeFromCodeCoverage]
public class PasswordIncludesLowercaseAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        var password = value as string;

        return !string.IsNullOrWhiteSpace(password) && password.Any(char.IsLower);
    }

    public override string FormatErrorMessage(string name)
    {
        return "Password must include at least one lowercase character.";
    }
}

[ExcludeFromCodeCoverage]
public class PasswordIncludeDigitAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        var password = value as string;

        return !string.IsNullOrWhiteSpace(password) && password.Any(char.IsDigit);
    }

    public override string FormatErrorMessage(string name)
    {
        return "Password must include at least one digit.";
    }
}

[ExcludeFromCodeCoverage]
public class PasswordIncludeSpecialCharacterAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        var password = value as string;

        return !string.IsNullOrWhiteSpace(password) && password.Any(c => !char.IsLetterOrDigit(c));
    }

    public override string FormatErrorMessage(string name)
    {
        return "Password must include at least one special character.";
    }
}
