using FluentResults;
using Microsoft.AspNetCore.Http;
using VespionSoftworks.DIVA.Spine.Auth;

namespace VespionSoftworks.DIVA.Spine.Tests.Helpers;

public class TestHeaderValidator: IDiscordHeaderValidator
{
	public Task<Result> ValidateHeaders(HttpRequest req, string? rawPublicKey)
	{
		return Task.FromResult(Result.Ok());
	}
}