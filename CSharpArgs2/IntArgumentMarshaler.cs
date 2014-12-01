using System;
using System.Collections.Generic;

namespace CSharpArgs2
{
    public class IntArgumentMarshaler : IArgumentMarshaler
    {
        public object Marshal(IEnumerator<string> currentArgument)
        {
            string parameter = null;

            try
            {
                parameter = currentArgument.Next();
                return Int32.Parse(parameter);
            }
            catch (InvalidOperationException)
            {
                throw new ArgsException(ErrorCode.MissingInteger);
            }
            catch (FormatException)
            {
                throw new ArgsException(ErrorCode.InvalidInteger, errorParameter: parameter);
            }
        }
    }
}