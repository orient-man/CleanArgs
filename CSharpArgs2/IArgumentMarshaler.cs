using System.Collections.Generic;

namespace CSharpArgs2
{
    public interface IArgumentMarshaler
    {
        dynamic Marshal(IEnumerator<string> currentArgument);
    }
}