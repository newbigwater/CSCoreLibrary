using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _000.Common._02._Format;
using _000.Common._03._Attribute;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _999.UnitTest._000._Common
{
    partial class _000_Common
    {
        [TestCategory("[CATEGORY] [000_Common] 02_Format")]
        [TestMethod("[DateTime] Get Individual String from DateTime")]
        [DataRow("2023-06-22 12:34:56.789")]
        public void UnitTest_GetIndividualStringFromDateTime(string condition)
        {
            DateTime dateTime = DateTime.Now;
            if (!DateTime.TryParse(condition, out dateTime))
                throw new Exception($"\"{condition}\"값을 DateTime으로 변환하지 못했습니다.");
            
            var cond_year = dateTime.ToString(DateTimeFormat.DateTimeYearStringFormat);
            Assert.AreEqual("2023", cond_year);

            var cond_month = dateTime.ToString(DateTimeFormat.DateTimeMonthStringFormat);
            Assert.AreEqual("06", cond_month);

            var cond_day = dateTime.ToString(DateTimeFormat.DateTimeDayStringFormat);
            Assert.AreEqual("22", cond_day);

            var cond_hour = dateTime.ToString(DateTimeFormat.DateTimeHourStringFormat);
            Assert.AreEqual("12", cond_hour);

            var cond_minute = dateTime.ToString(DateTimeFormat.DateTimeMinuteStringFormat);
            Assert.AreEqual("34", cond_minute);

            var cond_second = dateTime.ToString(DateTimeFormat.DateTimeSecondStringFormat);
            Assert.AreEqual("56", cond_second);

            var cond_milsecond = dateTime.ToString(DateTimeFormat.DateTimeMiliSecondStringFormat);
            Assert.AreEqual("789", cond_milsecond);
        }

        [TestCategory("[CATEGORY] [000_Common] 02_Format")]
        [TestMethod("[DateTime] Get String from Date")]
        [DataRow("2023-06-22 12:34:56.789")]
        public void UnitTest_GetStringFromDate(string condition)
        {
            DateTime dateTime = DateTime.Now;
            if (!DateTime.TryParse(condition, out dateTime))
                throw new Exception($"\"{condition}\"값을 DateTime으로 변환하지 못했습니다.");

            var cond_date = dateTime.ToString(DateTimeFormat.DateStringFormat);
            Assert.AreEqual("2023-06-22", cond_date);

            cond_date = dateTime.ToString(DateTimeFormat.DateStringFormatWithoutSeparator);
            Assert.AreEqual("20230622", cond_date);
        }

        [TestCategory("[CATEGORY] [000_Common] 02_Format")]
        [TestMethod("[DateTime] Get String from Time")]
        [DataRow("2023-06-22 12:34:56.789")]
        public void UnitTest_GetStringFromTime(string condition)
        {
            DateTime dateTime = DateTime.Now;
            if (!DateTime.TryParse(condition, out dateTime))
                throw new Exception($"\"{condition}\"값을 DateTime으로 변환하지 못했습니다.");

            var cond_time = dateTime.ToString(DateTimeFormat.TimeStringFormat);
            Assert.AreEqual("12:34:56.789", cond_time);

            cond_time = dateTime.ToString(DateTimeFormat.TimeStringFormatWithoutMilSec);
            Assert.AreEqual("12:34:56", cond_time);

            cond_time = dateTime.ToString(DateTimeFormat.TimeStringFormatWithoutSeparator);
            Assert.AreEqual("123456789", cond_time);

            cond_time = dateTime.ToString(DateTimeFormat.TimeStringFormatWithoutSeparatorAndMilSec);
            Assert.AreEqual("123456", cond_time);
        }

        [TestCategory("[CATEGORY] [000_Common] 02_Format")]
        [TestMethod("[DateTime] Get String from DateTime")]
        [DataRow("2023-06-22 12:34:56.789")]
        public void UnitTest_GetStringFromDateTime(string condition)
        {
            DateTime dateTime = DateTime.Now;
            if (!DateTime.TryParse(condition, out dateTime))
                throw new Exception($"\"{condition}\"값을 DateTime으로 변환하지 못했습니다.");

            var cond_time = dateTime.ToString(DateTimeFormat.DateTimeStringFormat);
            Assert.AreEqual("2023-06-22 12:34:56.789", cond_time);

            cond_time = dateTime.ToString(DateTimeFormat.DateTimeStringFormatWithoutSeparator);
            Assert.AreEqual("20230622 123456789", cond_time);
        }
    }
}
