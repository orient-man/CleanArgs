using System;
using System.Collections.Generic;
using System.Globalization;

namespace CSharpArgs2
{
    public class DoubleArgumentMarshaler : IArgumentMarshaler
    {
        public object Marshal(IEnumerator<string> currentArgument)
        {
            string parameter = null;

            try
            {
                parameter = currentArgument.Next();
                return double.Parse(parameter, CultureInfo.InvariantCulture);
            }
            catch (InvalidOperationException)
            {
                throw new ArgsException(ErrorCode.MissingDouble);
            }
            catch (FormatException)
            {
                throw new ArgsException(ErrorCode.InvalidDouble, errorParameter: parameter);
            }
        }
    }
}