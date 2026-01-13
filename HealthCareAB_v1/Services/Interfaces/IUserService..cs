using HealthCareAB_v1.Models;

namespace HealthCareAB_v1.Services.Interfaces;

public interface IUserService
{
    Task<bool> ExistsByUsernameAsync(string username);
    Task<Patient?> GetUserByUsernameAsync(string username);
    Task CreateUserAsync(Patient patient);
    string HashPassword(string password);
    bool VerifyPassword(string enteredPassword, string storedHash);
}
