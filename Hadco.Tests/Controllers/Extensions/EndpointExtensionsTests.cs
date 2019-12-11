using NUnit.Framework;
using Hadco.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.OData;
using Hadco.Services.DataTransferObjects;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace Hadco.Test.Services.Unit
{
    [TestFixture()]
    public class EndpointExtensionsTests
    {
        [Test()]
        public void PatchTest()
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization());
            var id = fixture.Create<int>();
           var dto = new Delta<EquipmentTimerEntryDto>();

            var sut = fixture.Create<EquipmentTimerEntriesController>();

            var response = sut.Patch(id, dto);
        }
    }
}