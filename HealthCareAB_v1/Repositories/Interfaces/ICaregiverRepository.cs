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
    Task<Patient?> GetPatientByIdAsync(CaregiverCreateBookingDto request);
    Task<CaregiverDailySchedule?> GetCaregiversDailyScheduleAsync(
        CaregiverCreateBookingDto request
    );

    Task<TimeSlot?> GetTimeSlotAsync(CaregiverCreateBookingDto request);

    Task AddBookingAsync(Booking booking);
}
