// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Contrib.LinqToDB
{
	internal class NopLogger : ILogger, IDisposable
	{
		public void Dispose()
		{
		}

		public IDisposable BeginScope<TState>(TState state)
		{
			return this;
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			return false;
		}

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
			Func<TState, Exception, string> formatter)
		{
		}
	}

	internal class NopLogger<T> : NopLogger, ILogger<T>
	{
	}
}