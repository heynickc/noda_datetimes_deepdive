using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;
using NodaTime.TimeZones;
using Xunit;
using Xunit.Abstractions;
using TimeZoneNames;

namespace DateTimesDeepDive {
    public class WithNodaTime {
        private readonly ITestOutputHelper _output;
        public WithNodaTime(ITestOutputHelper output) {
            _output = output;
        }

        [Fact]
        public void given_a_new_instant() {
            var instant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            Assert.Equal("2012-01-02T03:04:05Z", instant.ToString());
        }
        [Fact]
        public void instant_as_local_time() {
            // "America/New_York"
            var est = DateTimeZoneProviders.Tzdb["America/New_York"];
            _output.WriteLine(est.ToJson());
            var localDateTime = new LocalDateTime(2016, 1, 21, 20, 6);
            _output.WriteLine(localDateTime.ToString());
            var estDateTime = est.AtStrictly(localDateTime);
            Assert.Equal("2016-01-21T20:06:00 America/New_York (-05)", estDateTime.ToString());
        }

        [Fact]
        public void shift_timezone_for_instant() {
            var instant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            var localDateTimeZone = instant.InZone(DateTimeZoneProviders.Tzdb["America/New_York"]);

            Assert.Equal("2012-01-02T03:04:05Z", instant.ToString());
            Assert.Equal("2012-01-01T22:04:05 America/New_York (-05)", localDateTimeZone.ToString());
        }

        [Fact]
        public void localdatetime_to_offset() {
            var localDateTime = new LocalDateTime(2016, 1, 21, 20, 6).WithOffset(Offset.FromHours(-5));
            //var tz = DateTimeZoneProviders.Tzdb["America/New_York"];
            var sourceOffsetDateTime = new OffsetDateTime(new LocalDateTime(2016, 1, 21, 20, 6), Offset.FromHours(-5));
            var targetOffset = Offset.FromHours(-10);
            var targetDateTime = sourceOffsetDateTime.WithOffset(targetOffset);

            Assert.Equal("2016-01-21T20:06:00-05", sourceOffsetDateTime.ToString());
            Assert.Equal("2016-01-21T15:06:00-10", targetDateTime.ToString());
        }

        [Fact]
        public void get_current_utc_offset() {
            var currTz = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            var localDateTime = new LocalDateTime(2016, 1, 21, 20, 6);
            _output.WriteLine(currTz.AtStrictly(localDateTime).ToString());
        }

        [Fact]
        public void instant_with_offset() {
            var instant = Instant.FromUtc(2016, 1, 21, 20, 6);
            _output.WriteLine(instant.ToString());
            var offset = Offset.FromHours(-5);
            var sourceOffsetDateTime = instant.WithOffset(offset);
            _output.WriteLine(sourceOffsetDateTime.ToString());
        }

        [Fact]
        public void offsets_multiple() {
            var tz = DateTimeZoneProviders.Tzdb["America/Phoenix"];
            _output.WriteLine(tz.MinOffset.ToString());
            _output.WriteLine(tz.MaxOffset.ToString());
            
        }

        [Fact]
        public void go_to_another_offset_via_zoneddatetime() {
            var currTz = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            //var someonesTz = DateTimeZoneProviders.Tzdb["Someone Elses Time Zone"];
            var localDateTime = new LocalDateTime(2016, 1, 21, 20, 6);
            var sourceZonedDateTime = currTz.AtStrictly(localDateTime);
            var sourceOffsetDateTime = sourceZonedDateTime.ToOffsetDateTime();
            var targetOffset = Offset.FromHours(-10);
            var targetDateTime = sourceOffsetDateTime.WithOffset(targetOffset);
            var targetOffsetDateTime = targetDateTime.WithOffset(targetOffset);
            _output.WriteLine(targetDateTime.ToString());
            _output.WriteLine(targetOffsetDateTime.ToString());
        }

        [Fact]
        public void zoned_datetime_add() {
            var currTz = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            //var someonesTz = DateTimeZoneProviders.Tzdb["Someone Elses Time Zone"];
            var localDateTime = new LocalDateTime(2016, 1, 21, 20, 6);
            var sourceZonedDateTime = currTz.AtStrictly(localDateTime);
            var sourceOffsetDateTime = sourceZonedDateTime.ToOffsetDateTime();
            var targetOffset = Offset.FromHours(-10);
            var targetDateTime = sourceOffsetDateTime.WithOffset(targetOffset);
            var targetOffsetDateTime = targetDateTime.WithOffset(targetOffset);

            
        }

        [Fact]
        public void datetime_to_localdatetime() {
            var dateTime = new DateTime(2016, 1, 21, 20, 6, 0);
            var localDateTime = LocalDateTime.FromDateTime(dateTime);
            Assert.Equal("1/21/2016 8:06:00 PM", localDateTime.ToString());
        }

        [Fact]
        public void get_timezone_id_from_utc_offset() {
            var instant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            var offset = Offset.FromHours(-5);
            var tz = new ZonedDateTime(instant, DateTimeZone.ForOffset(offset));
            var odt = instant.WithOffset(offset);
            _output.WriteLine(odt.ToString());
            // InFixedZone means Fixed Time Zone - no DST observance
            _output.WriteLine(odt.InFixedZone().ToString());
            _output.WriteLine(DateTimeZone.ForOffset(offset).ToString());
            _output.WriteLine(tz.ToJson());
        }

        [Fact]
        public void more_offset_stuff() {
            var offset = Offset.FromHours(5);
            var localTime = new LocalDateTime(2012, 1, 2, 3, 4, 5);
            var localTimeWithOffset = localTime.WithOffset(offset);
           
        }

        [Fact]
        public void timezone_names_lookup() {
            var tzNames = TimeZoneNames.TimeZoneNames.GetTimeZonesForCountry("US", "en-US");
            _output.WriteLine(tzNames.ToJson());
        }

        public class TimeZoneConversionUtility {
            //public static DateTimeZone AdjustTimeZoneByOffset(DateTime localTime, int utcOffset) {
            //    DateTimeZoneProviders.Tzdb.
            //}
        }
    }
}
