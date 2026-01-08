namespace HealthCareAB_v1.Exceptions;

public class NotFoundException(string exception = "Resource was not found") : Exception(exception);

public class ValidationException(string exception = "User input is invalid") : Exception(exception);
