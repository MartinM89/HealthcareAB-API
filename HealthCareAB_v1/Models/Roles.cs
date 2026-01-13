namespace HealthCareAB_v1.Models;

public static class Roles
{
    public const string Caregiver = "CAREGIVER";
    public const string Patient = "PATIENT";

    // Helper method to validate roles
    public static bool IsValidRole(string role)
    {
        return role == Caregiver || role == Patient;
    }

    public static IReadOnlyList<string> AllRoles => [Caregiver, Patient];
}
