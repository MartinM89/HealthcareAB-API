using System.Diagnostics.CodeAnalysis;

namespace HealthCareAB_v1.Models;

[ExcludeFromCodeCoverage]
public static class Roles
{
    public const string Patient = "PATIENT";
    public const string Caregiver = "CAREGIVER";
    public const string Dentist = "DENTIST";

    public static IReadOnlyList<string> ValidCaregiverRoles => [Caregiver, Dentist];

    // Helper method to validate caregiver roles
    public static bool IsValidCaregiverRole(string role)
    {
        return ValidCaregiverRoles.Contains(role);
    }
}
