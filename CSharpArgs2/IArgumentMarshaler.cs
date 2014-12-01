using System.Collections.Generic;

namespace CSharpArgs2
{
    public interface IArgumentMarshaler
    {
        object Marshal(IEnumerator<string> currentArgument);
    }
}