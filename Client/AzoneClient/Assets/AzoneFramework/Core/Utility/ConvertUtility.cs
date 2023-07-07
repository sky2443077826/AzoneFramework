using System;

namespace AzoneFramework
{
    /// <summary>
    /// 类型转换工具
    /// </summary>
    public class ConvertUtility
    {
        public static bool BoolConvert(string value, bool defaultValue = false)
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }
            return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
        }

        public static bool BoolConvert(int value)
        {
            return value != 0;
        }

        public static long LongConvert(string value, long defaultValue = 0)
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }
            value = value.Trim();
            long result = defaultValue;
            Int64.TryParse(value, out result);
            return result;
        }

        public static ulong ULongConvert(string value, ulong defaultValue = 0)
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }
            value = value.Trim();
            ulong result = defaultValue;
            UInt64.TryParse(value, out result);
            return result;
        }

        public static int IntConvert(string value, int defaultValue = 0)
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }
            value = value.Trim();
            int result = defaultValue;
            int.TryParse(value, out result);
            return result;
        }

        public static float FloatConvert(string value, float defaultValue = 0)
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }
            value = value.Trim();
            float result = defaultValue;
            float.TryParse(value, out result);
            return result;
        }

        public static double DoubleConvert(string value, double defaultValue = 0)
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }
            value = value.Trim();
            double result = defaultValue;
            double.TryParse(value, out result);
            return result;
        }
    }
}

