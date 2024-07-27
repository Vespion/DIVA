﻿using Microsoft.Extensions.Options;

namespace VespionSoftworks.DIVA.Gateway.Tests.Helpers;

public class TestOptionsMonitor<T> : IOptionsMonitor<T>
	where T : class, new()
{
	public TestOptionsMonitor(T currentValue)
	{
		CurrentValue = currentValue;
	}

	public T Get(string name)
	{
		return CurrentValue;
	}

	public IDisposable OnChange(Action<T, string> listener)
	{
		throw new NotImplementedException();
	}

	public T CurrentValue { get; }
}