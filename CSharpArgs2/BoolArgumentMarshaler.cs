using System.Collections.Generic;

namespace CSharpArgs2
{
    public class BoolArgumentMarshaler : IArgumentMarshaler
    {
        public dynamic Marshal(IEnumerator<string> currentArgument)
        {
            return true;
        }
    }
}