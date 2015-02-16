using System;
using System.Collections.Generic;
using System.Globalization;

namespace CSharpArgs2
{
    public static class DoubleArgumentMarshaler
    {
        public static dynamic Marshal(IEnumerator<string> currentArgument)
        {
            if (!currentArgument.MoveNext())
                throw new ArgsException(ErrorCode.MissingDouble);

            try
            {
                return double.Parse(
                    currentArgument.Current,
                    CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                throw new ArgsException(
                    ErrorCode.InvalidDouble,
                    errorParameter: currentArgument.Current);
            }
        }
    }
}