using System.Collections.Generic;

namespace CSharpArgs
{
    public static class BooleanArgumentMarshaler
    {
        public static object Marshal(IEnumerator<string> currentArgument)
        {
            return true;
        }
    }
}