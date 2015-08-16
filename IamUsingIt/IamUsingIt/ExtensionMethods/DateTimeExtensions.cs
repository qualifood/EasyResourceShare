using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IamUsingIt.ExtensionMethods
{
    public static class DateTimeExtensions
    {
        public static DateTime ConvertToUserTime(this DateTime dateTime)
        {
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            var currentUserTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time"); 
            return TimeZoneInfo.ConvertTime(dateTime, currentUserTimeZoneInfo);
        }
    }
}