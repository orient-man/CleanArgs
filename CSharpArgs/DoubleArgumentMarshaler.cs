using System;
using System.Collections.Generic;
using System.Globalization;

namespace CSharpArgs
{
    public class DoubleArgumentMarshaler : IArgumentMarshaler
    {
        private double doubleValue;

        public void Set(IEnumerator<string> currentArgument)
        {
            if (!currentArgument.MoveNext())
                throw new ArgsException(ErrorCode.MissingDouble);
            try
            {
                doubleValue = Double.Parse(
                    currentArgument.Current,
                    CultureInfo.InvariantCulture.NumberFormat);
            }
            catch (FormatException)
            {
                throw new ArgsException(
                    ErrorCode.InvalidDouble,
                    currentArgument.Current);
            }
        }

        public static double GetValue(IArgumentMarshaler am)
        {
            var marshaler = am as DoubleArgumentMarshaler;
            return marshaler != null ? marshaler.doubleValue : 0.0;
        }
    }
}