using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Interfaces;
using HealthCareAB_v1.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HealthCareAB_v1.Services
{
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
            return await _context.Patients.AnyAsync(u => u.Username == username);
        }

        /// <inheritdoc />
        public async Task<Patient?> GetUserByUsernameAsync(string username)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(username);
            return await _context.Patients.FirstOrDefaultAsync(u => u.Username == username);
        }

        /// <inheritdoc />
        public async Task CreateUserAsync(Patient patient)
        {
            ArgumentNullException.ThrowIfNull(patient);
            await _context.Patients.AddAsync(patient);
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
}
