namespace Accrual.Application.Exceptions;

public class ServiceNotAvailableException(string message, Exception inner) : Exception(message, inner);