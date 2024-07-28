using VespionSoftworks.DIVA.Spine.Endpoints;
using VespionSoftworks.DIVA.Spine.Tests.Intergration.Helpers;
using Xunit.Abstractions;

namespace VespionSoftworks.DIVA.Spine.Tests.Intergration.Endpoints.Interactions;

public class InteractionsHeaderValidationTests(ITestOutputHelper testOutputHelper): HeaderValidationTests(InteractionsEndpoint.routePrefix, testOutputHelper);