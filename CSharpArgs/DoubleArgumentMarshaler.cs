using System;
using System.Globalization;

namespace CSharpArgs
{
    public class DoubleArgumentMarshaler : IArgumentMarshaler
    {
        private double doubleValue;

        public void Set(Iterator<string> currentArgument)
        {
            String parameter = null;
            try
            {
                parameter = currentArgument.Next();
                doubleValue = Double.Parse(parameter, CultureInfo.InvariantCulture.NumberFormat);
            }
            catch (NoSuchElementException)
            {
                throw new ArgsException(ErrorCode.MissingDouble);
            }
            catch (FormatException)
            {
                throw new ArgsException(ErrorCode.InvalidDouble, parameter);
            }
        }

        public static double GetValue(IArgumentMarshaler am)
        {
            var marshaler = am as DoubleArgumentMarshaler;
            return marshaler != null ? marshaler.doubleValue : 0.0;
        }
    }
}