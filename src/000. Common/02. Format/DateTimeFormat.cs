using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _000.Common._02._Format
{
    public static class DateTimeFormat
    {
        /// <summary>
        /// Ex) 2023
        /// </summary>
        public const string DateTimeYearStringFormat = "yyyy";
        /// <summary>
        /// Ex) 06
        /// </summary>
        public const string DateTimeMonthStringFormat = "MM";
        /// <summary>
        /// Ex) 28
        /// </summary>
        public const string DateTimeDayStringFormat = "dd";
        /// <summary>
        /// Ex) 13
        /// </summary>
        public const string DateTimeHourStringFormat = "HH";
        /// <summary>
        /// Ex) 25
        /// </summary>
        public const string DateTimeMinuteStringFormat = "mm";
        /// <summary>
        /// Ex) 10
        /// </summary>
        public const string DateTimeSecondStringFormat = "ss";
        /// <summary>
        /// Ex) 123
        /// </summary>
        public const string DateTimeMiliSecondStringFormat = "fff";

        /// <summary>
        /// Ex) 2023-06-28
        /// </summary>
        public static readonly string DateStringFormat = $"{DateTimeYearStringFormat}-{DateTimeMonthStringFormat}-{DateTimeDayStringFormat}";
        /// <summary>
        /// Ex) 20230628
        /// </summary>
        public static readonly string DateStringFormatWithoutSeparator =
            $"{DateTimeYearStringFormat}{DateTimeMonthStringFormat}{DateTimeDayStringFormat}";

        /// <summary>
        /// Ex) 13:12:12.123
        /// </summary>
        public static readonly string TimeStringFormat =
            $"{DateTimeHourStringFormat}:{DateTimeMinuteStringFormat}:{DateTimeSecondStringFormat}.{DateTimeMiliSecondStringFormat}";
        /// <summary>
        /// Ex) 13:12:12
        /// </summary>
        public static readonly string TimeStringFormatWithoutMilSec =
            $"{DateTimeHourStringFormat}:{DateTimeMinuteStringFormat}:{DateTimeSecondStringFormat}";
        /// <summary>
        /// Ex) 131212123
        /// </summary>
        public static readonly string TimeStringFormatWithoutSeparator =
            $"{DateTimeHourStringFormat}{DateTimeMinuteStringFormat}{DateTimeSecondStringFormat}{DateTimeMiliSecondStringFormat}";
        /// <summary>
        /// Ex) 131212
        /// </summary>
        public static readonly string TimeStringFormatWithoutSeparatorAndMilSec =
            $"{DateTimeHourStringFormat}{DateTimeMinuteStringFormat}{DateTimeSecondStringFormat}";

        /// <summary>
        /// Ex) 2023-06-28 13:12:12.123
        /// </summary>
        public static readonly string DateTimeStringFormat = $"{DateStringFormat} {TimeStringFormat}";
        /// <summary>
        /// Ex) 20230628 131212123
        /// </summary>
        public static readonly string DateTimeStringFormatWithoutSeparator = $"{DateStringFormatWithoutSeparator} {TimeStringFormatWithoutSeparator}";
    }
}
