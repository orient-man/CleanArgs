using System.Collections.Generic;

namespace CSharpArgs
{
    public class StringArgumentMarshaler : IArgumentMarshaler
    {
        public object Set(IEnumerator<string> currentArgument)
        {
            if (!currentArgument.MoveNext())
                throw new ArgsException(ErrorCode.MissingString);

            return currentArgument.Current;
        }
    }
}