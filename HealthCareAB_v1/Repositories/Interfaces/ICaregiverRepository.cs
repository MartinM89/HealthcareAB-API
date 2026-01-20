using HealthCareAB_v1.DTOs.User.Caregiver;
using HealthCareAB_v1.Models;

namespace HealthCareAB_v1.Repositories.Interfaces;

public interface ICaregiverRepository
{
    Task CreateCaregiverAsync(Caregiver caregiver);
    Task<ICollection<CaregiverDailySchedule>> GetSchedulesWithBookingsAsync(
        Guid caregiverId,
        DateTime startDate,
        DateTime endDate
    );
    Task<Patient?> GetPatientByIdAsync(CreateBookingDto request);
    Task<CaregiverDailySchedule?> GetCaregiversDailyScheduleAsync(CreateBookingDto request);

    Task<TimeSlot?> GetTimeSlotAsync(CreateBookingDto request);

    Task AddBookingAsync(Booking booking);
    Task<Booking?> GetBookingAsync(CancelBookingDto request);

    Task RemoveBookingAsync(Booking booking);
}
