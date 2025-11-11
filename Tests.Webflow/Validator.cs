using Apps.Webflow.Connections;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Tests.Webflow.Base;

namespace Tests.Webflow;

[TestClass]
public class Validator : TestBaseWithContext
{
    [TestMethod]
    public async Task ValidateConnection_WithCorrectCredentials_ReturnsValidResult()
    {
        var validator = new ConnectionValidator();

        var tasks = CredentialGroups.Select(x => validator.ValidateConnection(x, CancellationToken.None).AsTask());
        var results = await Task.WhenAll(tasks);
        Assert.IsTrue(results.All(x => x.IsValid));
    }

    [TestMethod]
    public async Task ValidateConnection_WithIncorrectCredentials_ReturnsInvalidResult()
    {
        // Arrange
        var validator = new ConnectionValidator();
        var newCreds = CredentialGroups.First().Select(x => new AuthenticationCredentialsProvider(x.KeyName, x.Value + "_incorrect"));

        // Act
        var ex = await Assert.ThrowsExactlyAsync<PluginApplicationException>(async () =>
            await validator.ValidateConnection(newCreds, CancellationToken.None)
        );

        // Assert
        Assert.Contains("Request not authorized", ex.Message);
    }
}