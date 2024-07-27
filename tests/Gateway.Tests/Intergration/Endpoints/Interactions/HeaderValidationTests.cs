using VespionSoftworks.DIVA.Gateway.Endpoints;
using VespionSoftworks.DIVA.Gateway.Tests.Intergration.Helpers;
using Xunit.Abstractions;

namespace VespionSoftworks.DIVA.Gateway.Tests.Intergration.Endpoints.Interactions;

public class InteractionsHeaderValidationTests(ITestOutputHelper testOutputHelper): HeaderValidationTests(InteractionsEndpoint.routePrefix, testOutputHelper);