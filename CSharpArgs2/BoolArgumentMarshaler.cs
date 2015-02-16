using System.Collections.Generic;

namespace CSharpArgs2
{
    public static class BoolArgumentMarshaler
    {
        public static dynamic Marshal(IEnumerator<string> currentArgument)
        {
            return true;
        }
    }
}