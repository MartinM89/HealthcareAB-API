using System.Diagnostics.CodeAnalysis;

namespace HealthCareAB_v1.Exceptions;

[ExcludeFromCodeCoverage]
public class NotFoundException(string exception = "Resource was not found") : Exception(exception);

[ExcludeFromCodeCoverage]
public class ValidationException(string exception = "User input is invalid") : Exception(exception);
