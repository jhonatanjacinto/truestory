using System;

namespace Truestory.Core.Exceptions;

public class TruestoryApiException(string message) : Exception(message);
