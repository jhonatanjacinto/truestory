using Truestory.Common.Exceptions;

namespace Truestory.WebApi.Exceptions;

public class ExternalApiServiceException(string message) : TruestoryApiException(message);
