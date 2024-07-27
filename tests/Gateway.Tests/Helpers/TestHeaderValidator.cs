using FluentResults;
using Microsoft.AspNetCore.Http;
using VespionSoftworks.DIVA.Gateway.Auth;

namespace VespionSoftworks.DIVA.Gateway.Tests.Helpers;

public class TestHeaderValidator: IDiscordHeaderValidator
{
	public Task<Result> ValidateHeaders(HttpRequest req, string? rawPublicKey)
	{
		return Task.FromResult(Result.Ok());
	}
}