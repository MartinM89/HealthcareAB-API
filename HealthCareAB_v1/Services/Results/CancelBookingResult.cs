namespace HealthCareAB_v1.Services.Results;

public enum CancelBookingResult
{
    BookingDoesNotExist,
    NotOwnedByPatient,
    Cancelled,
    Unauthorized,
}
