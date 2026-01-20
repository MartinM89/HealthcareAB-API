using HealthCareAB_v1.DTOs.User.Caregiver;
using HealthCareAB_v1.Models;

namespace HealthCareAB_v1.Services.Interfaces;

public interface ICaregiverService
{
    Task<ScheduleOverviewDto> GetScheduleOverviewAsync(
        Guid caregiverId,
        DateTime startDate,
        DateTime endDate
    );
    Task<ScheduleOverviewDto> GetUpcomingSchedulesAsync(Guid caregiverId, int daysAhead = 30);

    Task<Booking> CreateBookingForPatientAsync(Guid caregiverId, CreateBookingDto request);

    Task<Booking> CancelBookingForPatientAsync(Guid caregiverId, CancelBookingDto request);
}
