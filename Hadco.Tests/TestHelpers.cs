using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Hadco.Common;
using Moq;
using Ploeh.AutoFixture;

namespace Hadco.Tests
{
    public static class TestHelpers
    {

        public static void SetupAuthorizedUser(Fixture fixture, int currentUserID)
        {
            var currentUser = fixture.Create<Mock<ClaimsPrincipal>>();
            fixture.Inject<IPrincipal>(currentUser.Object);
            var claims = new List<Claim>()
                         {
                             new Claim(ClaimsExtensions.EMPLOYEEID_KEY, currentUserID.ToString())
                         };
            currentUser.Setup(x => x.Claims)
                       .Returns(claims);
        }

    }
}
