using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.ElasticLoadBalancing.Model;
using Bot.Infrastructure;
using Bot.Infrastructure.AWS;
using NUnit.Framework;

namespace BotTests.Infrastructure.AWS
{
    [TestFixture]
    public class OutTimeStateTests
    {
        protected OutTimeState state;

        [SetUp]
        public void SetUp()
        {
            SystemTime.Set(DateTime.Parse("2000-01-01T00:00:00"));
            this.state = new OutTimeState(new InstanceState());
        }

        [TestFixture]
        public class TimeSincePulled : OutTimeStateTests
        {
            protected void Verify(TimeSpan ts, string format)
            {
                SystemTime.MoveForward(ts);
                var actual = this.state.TimeSincePulled();
                Assert.AreEqual(format, actual);
            }

            [Test]
            public void Seconds_ReturnsSecondsFormatted()
            {
                Verify(TimeSpan.FromSeconds(1), "1s");
            }

            [Test]
            public void Minutes_ReturnsMinutesFormatted()
            {
                Verify(TimeSpan.Parse("00:02:03"), "2m3s");
            }

            [Test]
            public void Hours_ReturnsHoursFormatted()
            {
                Verify(TimeSpan.Parse("01:02:03"), "1h2m3s");
            }

            [Test]
            public void Days_ReturnsDaysFormatted()
            {
                Verify(TimeSpan.Parse("4.01:02:03"), "4d1h2m WTF!");
            }
        }
    }
}
