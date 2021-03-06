using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using FluentAssertions;
using Stl.Fusion.Authentication;
using Xunit;
using Xunit.Abstractions;

namespace Stl.Fusion.Tests.Authentication
{
    public class UserPropertiesTest : FusionTestBase
    {
        public UserPropertiesTest(ITestOutputHelper @out, FusionTestOptions? options = null) : base(@out, options) { }

        [Fact]
        public void BasicTest()
        {
            var user = new User("none")
                .WithClaim("a", "b")
                .WithIdentity("Google/1", "Secret");

            user.Claims.Should().BeEquivalentTo(new [] {
                KeyValuePair.Create("a", "b")
            });
            var uid = user.Identities.Keys.Single();
            var (authType, userId) = uid;
            authType.Should().Be("Google");
            userId.Should().Be("1");
            user.Identities[uid].Should().Be("Secret");

            Out.WriteLine(user.ToString());
        }

        [Fact]
        public void ParseTest()
        {
            (string AuthType, string UserId) Parse(UserIdentity userIdentity) {
                var (authType, userId) = userIdentity;
                return (authType, userId);
            }

            Parse("1").Should().Be((UserIdentity.DefaultAuthenticationType, "1"));
            Parse("1/2").Should().Be(("1", "2"));
            Parse("1\\/2").Should().Be((UserIdentity.DefaultAuthenticationType, "1/2"));
        }
    }
}
