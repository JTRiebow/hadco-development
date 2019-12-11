using NUnit.Framework;
using Hadco.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Test
{
    [TestFixture()]
    public class EnumHelperTests
    {
        [Test()]
        public void ToDictionaryTest()
        {
            var result = EnumHelper.ToDictionary<Common.Enums.AuthActivityID>();
            Assert.AreEqual((int)Common.Enums.AuthActivityID.ViewTruckerDailies, result[Common.Enums.AuthActivityID.ViewTruckerDailies.ToString()]);
            Assert.AreEqual(Common.Enums.AuthActivityID.DownloadJobTimersCsv.ToString().Substring(0, 1).ToLower() + Common.Enums.AuthActivityID.DownloadJobTimersCsv.ToString().Substring(1), result[((int)Common.Enums.AuthActivityID.DownloadJobTimersCsv).ToString()]);
        }
    }
}