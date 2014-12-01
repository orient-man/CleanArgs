using System;
using System.Collections.Generic;

namespace CSharpArgs2
{
    public class StringArgumentMarshaler : IArgumentMarshaler
    {
        public object Marshal(IEnumerator<string> currentArgument)
        {
            try
            {
                return currentArgument.Next();
            }
            catch (InvalidOperationException)
            {
                throw new ArgsException(ErrorCode.MissingString);
            }
        }
    }
}