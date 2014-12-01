using System.Collections.Generic;

namespace CSharpArgs2
{
    public class BoolArgumentMarshaler : IArgumentMarshaler
    {
        public object Marshal(IEnumerator<string> currentArgument)
        {
            return true;
        }
    }
}