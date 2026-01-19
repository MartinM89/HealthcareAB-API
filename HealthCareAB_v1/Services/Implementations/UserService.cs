using System.Diagnostics.CodeAnalysis;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Interfaces;
using HealthCareAB_v1.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HealthCareAB_v1.Services.Implementations;

[ExcludeFromCodeCoverage]
/// <summary>
/// Service for user-related operations including CRUD and password management.
/// </summary>
public class UserService(IAppDbContext context) : IUserService
{
    private readonly IAppDbContext _context =
        context ?? throw new ArgumentNullException(nameof(context));

    /// <inheritdoc />
    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        return await _context.Users.AnyAsync(u => u.Username == username);
    }

    /// <inheritdoc />
    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<Caregiver?> GetCaregiverByIdAsync(Guid caregiverId)
    {
        ArgumentException.ThrowIfNullOrEmpty(caregiverId.ToString());
        return await _context.Caregivers.FirstOrDefaultAsync(c => c.UserId == caregiverId);
    }

    public async Task<Patient?> GetPatientByIdAsync(Guid patientId)
    {
        ArgumentException.ThrowIfNullOrEmpty(patientId.ToString());
        return await _context.Patients.FirstOrDefaultAsync(p => p.UserId == patientId);
    }

    /// <inheritdoc />
    public async Task CreateUserAsync(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public string HashPassword(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <inheritdoc />
    public bool VerifyPassword(string enteredPassword, string storedHash)
    {
        if (string.IsNullOrWhiteSpace(enteredPassword) || string.IsNullOrWhiteSpace(storedHash))
        {
            return false;
        }
        return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);
    }
}
