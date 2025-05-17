using System;
using Truestory.Core.Exceptions;

namespace Truestory.WebApi.Exceptions;

public class ExternalApiServiceException(string message) : TruestoryApiException(message);
