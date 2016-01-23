using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace DateTimesDeepDive {
    public class WithBCLDateTime {
        private readonly ITestOutputHelper _output;
        public WithBCLDateTime(ITestOutputHelper output) {
            _output = output;
        }
        [Fact]
        public void given_a_new_datetime() {
            var dateTime = new DateTime();
            Assert.Equal("1/1/0001 12:00:00 AM", dateTime.ToString());
        }

        [Fact]
        public void given_a_new_datetime_with_meaning() {
            var dateTime = new DateTime(2012, 1, 2, 3, 4, 5);
            Assert.Equal("1/2/2012 3:04:05 AM", dateTime.ToString());
        }

        [Fact]
        public void given_a_new_datetime_kind_is_unspecified() {
            var dateTime = new DateTime(2012, 1, 2, 3, 4, 5);
            Assert.Equal("Unspecified", dateTime.Kind.ToString());
        }

        [Fact]
        public void given_datetime_can_set_kind_to_utc() {
            var dateTime = new DateTime(2012, 1, 2, 3, 4, 5, DateTimeKind.Utc);
            Assert.Equal("Utc", dateTime.Kind.ToString());
        }

        [Fact]
        public void whats_my_tz() {
            Assert.Equal("US Eastern Standard Time", TimeZone.CurrentTimeZone.StandardName);
            Assert.Equal("US Eastern Daylight Time", TimeZone.CurrentTimeZone.DaylightName);
        }

        [Fact]
        public void lol_offset_wut() {
            // DateTimeKind.Local
            var dateTime = new DateTime(2012, 1, 2, 3, 4, 5, DateTimeKind.Local);
            var offset = TimeZone.CurrentTimeZone.GetUtcOffset(dateTime);
            // Always results in -05:00:00 regardless of actual DateTime value
            Assert.Equal("-05:00:00", offset.ToString());
        }

        [Fact]
        public void is_it_dst() {
            var dateTime = new DateTime(2012, 1, 2, 3, 4, 5, DateTimeKind.Local);
            var isDst = TimeZone.CurrentTimeZone.IsDaylightSavingTime(dateTime);
            Assert.Equal(false, isDst);
        }

        [Fact]
        public void convert_utc_datetime_to_dst() {
            var dateTime = new DateTime(2012, 1, 2, 3, 4, 5, DateTimeKind.Local);
            var inUtc = TimeZone.CurrentTimeZone.ToUniversalTime(dateTime);
            Assert.Equal("1/2/2012 8:04:05 AM", inUtc.ToString());
        }

        [Fact]
        public void build_datetimeoffset_from_datetime_local() {
            var dateTime = new DateTime(2012, 1, 2, 3, 4, 5, DateTimeKind.Local);
            var dtOffset = new DateTimeOffset(dateTime);
            Assert.Equal("1/2/2012 3:04:05 AM -05:00", dtOffset.ToString());
        }

        [Fact]
        public void datetimeoffset_conversion_with_tooffset() {
            var dateTime = new DateTime(2012, 1, 2, 3, 4, 5, DateTimeKind.Local);
            var sourceTime = new DateTimeOffset(dateTime);
            var targetTime = sourceTime.ToOffset(new TimeSpan(-8, 0, 0));
            Assert.Equal("1/2/2012 12:04:05 AM -08:00", targetTime.ToString());
        }

        [Fact]
        public void back_and_forth_datetime_offset_conversions() {
            var dateTime = new DateTime(2012, 1, 2, 3, 4, 5, DateTimeKind.Local);
            // For illustration - dateTime in UTC is 1/2/2012 8:04:05 AM or +5 hrs
            var inUtc = TimeZone.CurrentTimeZone.ToUniversalTime(dateTime);
            // Existing Local Offset = -05:00
            var sourceTime = new DateTimeOffset(dateTime);
            // Converted to -02:00 Offset = Adds 3 hrs to sourceTime
            var targetTime = sourceTime.ToOffset(new TimeSpan(-2, 0, 0));

            Assert.Equal("1/2/2012 8:04:05 AM", inUtc.ToString());
            Assert.Equal("1/2/2012 3:04:05 AM -05:00", sourceTime.ToString());
            Assert.Equal("1/2/2012 6:04:05 AM -02:00", targetTime.ToString());
        }

        [Fact]
        public void timezoneinfo_conversion_utility() {
            TimeZoneInfo tzInfo = TimeZoneInfo.Local;
            //_output.WriteLine(tzInfo.ToJson());
            // Win 7 US Eastern Standard Time
            Assert.Equal("Eastern Standard Time", tzInfo.StandardName); // Win 10 "Eastern Standard Time"
            Assert.Equal("Eastern Daylight Time", tzInfo.DaylightName);
            Assert.Equal("-05:00:00", tzInfo.BaseUtcOffset.ToString());
        }

        [Fact]
        public void convert_from_hawaii_to_local() {
            var hwZone = TimeZoneInfo.FindSystemTimeZoneById("Hawaiian Standard Time");
            _output.WriteLine(hwZone.BaseUtcOffset.ToString()); // -10:00
            var dateTime = new DateTime(2012, 1, 2, 3, 4, 5); // -05:00
            var targetTime = TimeZoneInfo.ConvertTime(dateTime, hwZone, TimeZoneInfo.Local); // +5 hrs

            Assert.Equal("1/2/2012 8:04:05 AM", targetTime.ToString());
        }

        [Fact]
        public void try_convert_datetimes_with_offset_integer_only() {
            var sourceDateTime = new DateTime(2012, 1, 2, 3, 4, 5, DateTimeKind.Local);
            var offsetInt = 5;

            var sourceDateTimeOffset = new DateTimeOffset(sourceDateTime);
            var targetDateTimeOffset = sourceDateTimeOffset.ToOffset(new TimeSpan(offsetInt, 0, 0));

            var tzInfos = TimeZoneInfo.GetSystemTimeZones();
            var tzInfo = tzInfos.Where(x => x.BaseUtcOffset == targetDateTimeOffset.Offset).Select(y => y.DisplayName).FirstOrDefault();
            //_output.WriteLine(tzInfos.ToJson());
            Assert.Equal("(UTC+05:00) Ashgabat, Tashkent", tzInfo);
            Assert.Equal("1/2/2012 1:04:05 PM +05:00", targetDateTimeOffset.ToString());
        }

        [Fact]
        public void datetimeoffset_conversion_extension() {
            var sourceDateTime = new DateTime(2012, 1, 2, 3, 4, 5, DateTimeKind.Local);
            var offsetInt = 5;

            var targetDateTimeOffsetTz = DateTimeOffsetExtensions.ToDateTimeOffsetTz(sourceDateTime, offsetInt);
            _output.WriteLine(targetDateTimeOffsetTz.ToJson());
        }

        [Fact]
        public void spring_forward_issue() {
            // https://www.timeanddate.com/worldclock/converted.html?iso=20160313T04&p1=179&p2=80
            var tzi = TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time");
            var localTime = new DateTime(2016, 3, 13, 4, 0, 0);

            var actual = DateTimeOffsetExtensions.AdjustTimeZoneOffset(localTime, tzi);
            _output.WriteLine(actual.ToString());

            var expected = new DateTime(2016, 3, 13, 1, 0, 0);
            Assert.NotEqual(expected, actual);
            // Xunit.Sdk.EqualException: Assert.Equal() Failure
            // Expected: 2016-03-13T01:00:00.0000000
            // Actual:   2016-03-13T02:00:00.0000000
        }
        [Fact]
        public void spring_forward_issue_with_converttime() {
            // https://www.timeanddate.com/worldclock/converted.html?iso=20160313T04&p1=179&p2=80
            var tzi = TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time");
            var localTime = new DateTime(2016, 3, 13, 4, 0, 0);

            var actual = DateTimeOffsetExtensions.AdjustTimeZoneOffsetConvertTime(localTime, tzi);
            _output.WriteLine(actual.ToString());

            var expected = new DateTime(2016, 3, 13, 1, 0, 0);
            Assert.Equal(expected, actual);
        }

        public class DateTimeOffsetExtensions {
            public class DateTimeOffsetTz {
                public DateTime DateTime { get; set; }
                public DateTimeOffset DateTimeOffset { get; set; }
                public TimeZoneInfo TimeZoneInfo { get; set; }
            }

            public static DateTimeOffsetTz ToDateTimeOffsetTz(DateTime dateTime, int offsetInt) {
                var sourceDateTimeOffset = new DateTimeOffset(dateTime);
                var targetDateTimeOffset = sourceDateTimeOffset.ToOffset(new TimeSpan(offsetInt, 0, 0));
                // this isn't right because baseutcoffset is not tz/dst aware
                // assumes only one so it's like UTC-5, doesn't change at DST
                // we don't have a tzinfo object yet to use GetUtcOffset(DateTimeOffset) either
                var tzInfo = TimeZoneInfo.GetSystemTimeZones()
                    .FirstOrDefault(x => x.BaseUtcOffset == targetDateTimeOffset.Offset);

                return new DateTimeOffsetTz() {
                    DateTime = targetDateTimeOffset.DateTime,
                    DateTimeOffset = targetDateTimeOffset,
                    TimeZoneInfo = tzInfo
                };
            }
            // http://codeofmatt.com/2015/04/20/beware-the-edge-cases-of-time/
            public static DateTime AdjustTimeZoneOffset(DateTime localTime, TimeZoneInfo tzi = null) {
                var offset = tzi.GetUtcOffset(localTime).TotalHours;
                var offset2 = TimeZoneInfo.Local.GetUtcOffset(localTime).TotalHours;
                return localTime.AddHours(offset - offset2);
            }

            public static DateTime? AdjustTimeZoneByOffset(DateTime localTime, int offsetInt) {
                var tzi = TimeZoneInfo.GetSystemTimeZones()
                    .FirstOrDefault(x => x.BaseUtcOffset.Hours == offsetInt);
                if (tzi != null)
                    return TimeZoneInfo.ConvertTime(localTime, tzi);
                return null;
            }
            public static DateTime AdjustTimeZoneOffsetConvertTime(DateTime localTime, TimeZoneInfo tzi = null) {
                return TimeZoneInfo.ConvertTime(localTime, tzi);
            }
        }
    }
}
