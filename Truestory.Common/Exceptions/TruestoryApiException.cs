using System;

namespace Truestory.Common.Exceptions;

public class TruestoryApiException(string message) : Exception(message);
