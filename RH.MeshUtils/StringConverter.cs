using System;
using System.Globalization;

namespace RH.MeshUtils
{
    /// <summary> Helper class for working with string </summary>
    public static class StringConverter
    {
        public static float ToFloat(string str)
        {
            return (float)ToDouble(str, float.NaN);
        }
        public static float ToFloat(string str, float nanValue)
        {
            return (float)ToDouble(str, nanValue);
        }

        public static double ToDouble(object value)
        {
            return value == null ? double.NaN : ToDouble(value.ToString());
        }
        public static double ToDouble(string str)
        {
            return ToDouble(str, Double.NaN);
        }
        public static double ToDouble(string str, double nanValue)
        {
            if (string.IsNullOrEmpty(str) || str.Length == 0) return nanValue;
            var newSeparator = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            var oldSeparator = newSeparator == "," ? "." : ",";
            var s = str.Replace(oldSeparator, newSeparator).Trim(new[] { ' ', '\t' });

            double result;
            if (double.TryParse(s, NumberStyles.Any, NumberFormatInfo.CurrentInfo, out result))
                return result;

            return nanValue;
        }

        public static DateTime ToDateTime(object value)
        {
            return ToDateTime(value, DateTime.MinValue);
        }
        public static DateTime ToDateTime(object value, DateTime nanValue)
        {
            return value == null ? nanValue : ToDateTime(value.ToString(), nanValue);
        }
        public static DateTime ToDateTime(string str)
        {
            return ToDateTime(str, DateTime.MinValue);
        }
        public static DateTime ToDateTime(string str, DateTime nanValue)
        {
            if (string.IsNullOrEmpty(str) || str.Length == 0)
                return nanValue;
            var newSeparator = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            var oldSeparator = newSeparator == "," ? "." : ",";
            var s = str.Replace(oldSeparator, newSeparator).Trim(new[] { ' ', '\t' });

            DateTime result;
            return DateTime.TryParse(s, NumberFormatInfo.CurrentInfo, DateTimeStyles.None, out result) ? result : nanValue;
        }

        public static int ToInt(object value, int nanValue = int.MinValue)
        {
            return value == null ? nanValue : ToInt(value.ToString(), nanValue);
        }
        public static int ToInt(string str, int nanValue = int.MinValue)
        {
            var value = ToDouble(str);
            return double.IsNaN(value) ? nanValue : (int)value;
        }
    }
}
