using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using NodaTime;
using NodaTime.Testing;
using Xunit;
using Xunit.Abstractions;

namespace DateTimesDeepDive {
    public class TestingWithIClock {
        private readonly ITestOutputHelper _output;
        public TestingWithIClock(ITestOutputHelper output) {
            _output = output;
        }

        [Fact]
        public void when_on_a_market_day_do_thing1() {
            var clock = new FakeClock(
                Instant.FromUtc(2016, 5, 8, 0, 0));
            var stock = new Stock();
            var marketDay = new MarketDay(
                Instant.FromUtc(2016, 5, 8, 0, 0));

            var myClass = new MyClassNodaTime(stock, marketDay, clock);

            Assert.Equal("Thing1 done!", myClass.thingState);
        }

        [Fact]
        public void when_not_on_a_market_day_do_thing2() {
            var clock = new FakeClock(
                Instant.FromUtc(2016, 5, 8, 0, 0));
            var stock = new Stock();
            var marketDay = new MarketDay(
                Instant.FromUtc(2016, 5, 8, 0, 0));

            var myClass = new MyClassNodaTime(stock, marketDay, clock);

            Assert.Equal("Thing2 done!", myClass.thingState);
        }
    }

    public class MyClass {
        public IStock Stock { get; private set; }
        public IMarketDay MarketDay { get; private set; }
        public DateTime CreatedOn { get; }
        public string thingState { get; private set; }

        public MyClass(IStock stock, IMarketDay marketDay) {
            Stock = stock;
            MarketDay = marketDay;
            CreatedOn = DateTime.Now;

            if (marketDay.Date.ToDateTimeUtc() == CreatedOn.Date) {
                DoThing1();
            }
            else {
                DoThing2();
            }
        }
        private void DoThing1() {
            thingState = "Thing1 done!";
        }
        private void DoThing2() {
            thingState = "Thing2 done!";
        }
    }

    public class MyClassCurrentTimeInConstructor {
        public IStock Stock { get; private set; }
        public IMarketDay MarketDay { get; private set; }
        public DateTime CreatedOn { get; private set; }
        public string thingState { get; private set; }

        public MyClassCurrentTimeInConstructor(
                IStock stock,
                IMarketDay marketDay) {
            Stock = stock;
            MarketDay = marketDay;
            CreatedOn = SystemTime.Now();

            if (marketDay.Date.ToDateTimeUtc() == CreatedOn.Date) {
                DoThing1();
            }
            else {
                DoThing2();
            }
        }

        private void DoThing1() {
            thingState = "Thing1 done!";
        }
        private void DoThing2() {
            thingState = "Thing2 done!";
        }
    }

    public class MyClassNodaTime {
        public IStock Stock { get; private set; }
        public IMarketDay MarketDay { get; private set; }
        public Instant CreatedOn { get; private set; }
        public string thingState { get; private set; }

        public MyClassNodaTime(
                IStock stock,
                IMarketDay marketDay,
                IClock clock) {
            Stock = stock;
            MarketDay = marketDay;
            CreatedOn = clock.Now;

            if (marketDay.Date.InUtc().Date == CreatedOn.InUtc().Date) {
                DoThing1();
            }
            else {
                DoThing2();
            }
        }

        private void DoThing1() {
            thingState = "Thing1 done!";
        }
        private void DoThing2() {
            thingState = "Thing2 done!";
        }
    }

    public interface IStock {
    }

    class Stock : IStock {
    }

    public interface IMarketDay {
        Instant Date { get; }
    }

    class MarketDay : IMarketDay {
        public Instant Date { get; }
        public MarketDay(Instant date) {
            Date = date;
        }
    }
    public static class SystemTime {
        public static Func<DateTime> Now = () => DateTime.Now;
    }
}

