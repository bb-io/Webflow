using Apps.Webflow.Connections;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Tests.Webflow
{
    [TestClass]
    public class Validator : TestBase
    {
        [TestMethod]
        public async Task ValidatesCorrectConnection()
        {
            var validator = new ConnectionValidator();

            var result = await validator.ValidateConnection(Creds, CancellationToken.None);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task DoesNotValidateIncorrectConnection()
        {
            var validator = new ConnectionValidator();

            var newCreds = Creds.Select(x => new AuthenticationCredentialsProvider(AuthenticationCredentialsRequestLocation.None,x.KeyName, x.Value + "_incorrect"));
            try
            {
                var result = await validator.ValidateConnection(newCreds, CancellationToken.None);

                Assert.Fail("Expected an exception to be thrown for invalid credentials, but no exception was thrown.");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Request not authorized", e.Message,
                    "Expected exception message to be 'Request not authorized'");
            }
        }
    }
}