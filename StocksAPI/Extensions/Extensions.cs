using System;

namespace StocksAPI.Extensions
{
    public static class Extensions
    {
        private static readonly decimal Billion = 1000000000;
        private static readonly decimal Million = 1000000;
        private static readonly decimal Thousand = 1000;

        public static decimal TwoDecimalValues(this decimal value)
        {
            return decimal.Parse(value.ToString("0.00"));
        }

        public static decimal ThreeDecimalValues(this decimal value)
        {
            return decimal.Parse(value.ToString("0.000"));
        }

        public static decimal ZeroDecimalValues(this decimal value)
        {
            return decimal.Parse(value.ToString("0"));
        }

        public static decimal Upside(this decimal value, decimal target)
        {
            decimal perc = ((target / value) - 1) * 100;
            return decimal.Parse(perc.ToString("0"));
        }

        public static int LongDate(this DateTime value)
        {
            return value.Year * 10000 + value.Month * 100 + value.Day;
        }

        public static string ToDynamicString(this decimal value, StringfyModifiers modifier = StringfyModifiers.None)
        {
            if(value >= Billion)
            {
                var shortedValue = (value / Billion).TwoDecimalValues().ToString() + DynamicMagnitudes.B.ToString();
                return modifier == StringfyModifiers.None ? shortedValue : string.Format(GetModifier(modifier), shortedValue);
            }
            else if(value >= Million)
            {
                var shortedValue = (value / Million).TwoDecimalValues().ToString() + DynamicMagnitudes.M.ToString();
                return modifier == StringfyModifiers.None ? shortedValue : string.Format(GetModifier(modifier), shortedValue);
            }
            else if (value >= Thousand)
            {
                var shortedValue = (value / Thousand).TwoDecimalValues().ToString() + DynamicMagnitudes.K.ToString();
                return modifier == StringfyModifiers.None ? shortedValue : string.Format(GetModifier(modifier), shortedValue);
            }
            else
            {
                return modifier == StringfyModifiers.None ? value.ToString() : string.Format(GetModifier(modifier), value.ToString());
            }
        }

        private static string GetModifier(StringfyModifiers modifier)
        {
            switch(modifier)
            {
                case StringfyModifiers.CurrencyPesos: return "${0}";
                case StringfyModifiers.CurrencyDollar: return "{0} USD";
                case StringfyModifiers.Percentage: return "{0}%";
                default: return string.Empty;
            }
        }
    }

    public enum StringfyModifiers
    {
        None,
        CurrencyPesos,
        CurrencyDollar,
        Percentage
    }

    public enum DynamicMagnitudes
    {
        None,
        K,
        M,
        B
    }
}
