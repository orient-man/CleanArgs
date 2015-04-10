using System;
using System.Collections.Generic;
using System.Globalization;

namespace CSharpArgs
{
    public class DoubleArgumentMarshaler : IArgumentMarshaler
    {
        public object Set(IEnumerator<string> currentArgument)
        {
            if (!currentArgument.MoveNext())
                throw new ArgsException(ErrorCode.MissingDouble);
            try
            {
                return Double.Parse(
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
    }
}