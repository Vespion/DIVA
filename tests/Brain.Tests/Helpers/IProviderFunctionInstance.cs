using Microsoft.Extensions.Logging;
using VespionSoftworks.DIVA.Brain.Functions;

namespace VespionSoftworks.DIVA.Brain.Tests.Helpers;

public interface IProviderFunctionInstance<TFunction> where TFunction : FunctionBase<TFunction>
{
	TFunction CreateFunction(ILogger<TFunction> logger);
}