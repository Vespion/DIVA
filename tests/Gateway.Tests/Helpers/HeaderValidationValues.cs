namespace VespionSoftworks.DIVA.Gateway.Tests.Helpers;

internal class HeaderValidationValues
{
	internal const string ValidPublicKey = "62937154bdfd60e39acd6517d0962e879b2c3970e29dcbf0465c275398d06d24";
	internal const string InvalidPublicKey = "54AEBA4C254D6A570F0F874D524C0ECE2E7C9EEC9DC8E35CFC62FC43DFAB0E34";

	internal const string ExampleJson = """{"app_permissions":"562949953601536","application_id":"1266083078506418176","authorizing_integration_owners":{},"entitlements":[],"id":"1266118899867586562","token":"aW50ZXJhY3Rpb246MTI2NjExODg5OTg2NzU4NjU2Mjo0OEREdjNBc0R5d3pPMkh0Y2xsb0JtR1QySGc0T1hjU29RbHNvMDR5UFhuZzNVN2ZrOFowN1Bja0dwZEVwS0dZUm03YTZhUzNFeUc4aW5TTUtEQ0VBOXl6WWZhZFlZREZCN2UyUVZwWWxYYlNlRmVkTkxUVEdyMG5NSUZrQ0hXUw","type":1,"user":{"avatar":"c6a249645d46209f337279cd2ca998c7","avatar_decoration_data":null,"bot":true,"clan":null,"discriminator":"0000","global_name":"Discord","id":"643945264868098049","public_flags":1,"system":true,"username":"discord"},"version":1}""";

	internal const string ValidTimestamp = "1721936669";
	internal const string InvalidFutureTimestamp = "1921936669";
	internal const string InvalidPastTimestamp = "1721935669";
	internal const string ValidSig = "b8270d26694ec75990d7cc7dae65de6625787f42400599d36c14760309a31bf5efa7d14a8abbdee5d2aad0c7e2f627ee16922251a5658d57d2e68f70f57fb10f";
	internal const string InvalidSig = "14760309a31bf5efa7d14a8abbdee5d2aad0c7e2f627ee16922251a5658d57d2e68f70f57fb10fb8270d26694ec75990d7cc7dae65de6625787f42400599d36c";

}