using HealthCareAB_v1.DTOs;
using HealthCareAB_v1.Exceptions;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Interfaces;
using HealthCareAB_v1.Services.Interfaces;

namespace HealthCareAB_v1.Services.Implementations;

public class TimeSlotService(ITimeSlotRepository timeSlotRepository) : ITimeSlotService
{
    private readonly ITimeSlotRepository _timeSlotRepository = timeSlotRepository;

    public async Task<TimeSlot> GetByIdAsync(Guid timeSlotId)
    {
        if (timeSlotId == Guid.Empty)
        {
            throw new ValidationException("Guid can't be empty");
        }

        return await _timeSlotRepository.GetByIdAsync(timeSlotId)
            ?? throw new NotFoundException("Time slot not found");
    }

    public async Task<IEnumerable<TimeSlotAvailabilityDto>> GetAvailableTimeSlotsAsync(
        DateOnly selectedDate,
        CancellationToken ct
    )
    {
        var slots = await _timeSlotRepository.GetAllAsync(ct);

        var date = selectedDate.ToDateTime(slots.ToList()[0].Start).ToUniversalTime();

        var schedules = await _timeSlotRepository.GetByDateAsync(selectedDate, date, ct);

        if (schedules.Count == 0)
        {
            return [];
        }

        return slots.Select(s =>
        {
            var slotStartDt = selectedDate.ToDateTime(s.Start).ToUniversalTime();

            var scheduleIds = schedules
                .Where(s => s.StartTime <= slotStartDt)
                .Select(s => s.Id)
                .ToList();

            var bookedScheduleIds = schedules.SelectMany(bs =>
                bs.Bookings.Where(b => b.TimeSlot.Start == s.Start)
                    .Select(bs => bs.CaregiverDailyScheduleId)
                    .ToList()
            );

            var freeScheduleId = scheduleIds.FirstOrDefault(id => !bookedScheduleIds.Contains(id));

            return new TimeSlotAvailabilityDto
            {
                Id = s.Id,
                Start = s.Start.ToShortTimeString(),
                End = s.End.ToShortTimeString(),
                IsAvailable = freeScheduleId != Guid.Empty,
                ScheduleId = freeScheduleId != Guid.Empty ? freeScheduleId : null,
            };
        });
    }
}
