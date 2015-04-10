using System.Collections.Generic;

namespace CSharpArgs
{
    public class BooleanArgumentMarshaler : IArgumentMarshaler
    {
        public object Set(IEnumerator<string> currentArgument)
        {
            return true;
        }
    }
}