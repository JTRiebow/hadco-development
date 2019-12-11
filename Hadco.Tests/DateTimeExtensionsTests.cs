using Hadco.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Ploeh.AutoFixture;

namespace Hadco.Test.Services.Unit
{
    [TestFixture()]
    public class DateTimeExtensionsTests
    {

        [Test]
        public void SetsSecondsAndMillisecondsToZero()
        {
            var now = DateTimeOffset.Now;

            var roundedNow = DateTimeExtensions.RoundToMinute(now);

            var difference = now - roundedNow;
            Assert.That(Math.Abs(difference.Minutes) < 1);
            Assert.That(roundedNow.Second == 0);
            Assert.That(roundedNow.Millisecond == 0);
        }
        [Test]
        public void RoundsTwoCloseTimesToTheExactSameTime()
        {
            var fixture = new Fixture();
            var time1 = fixture.Create<DateTimeOffset>();
            var time2 = new DateTimeOffset(time1.Year, time1.Month, time1.Day, time1.Hour, time1.Minute, time1.Second, time1.Offset);
            var time1Rounded = time1.RoundToMinute();
            var time2Rounded = time2.RoundToMinute();

            Assert.That(time1 != time2);
            Assert.That(time1Rounded == time2Rounded);
        }
    }
}