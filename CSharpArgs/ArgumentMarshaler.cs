using System.Collections.Generic;

namespace CSharpArgs
{
    public interface IArgumentMarshaler
    {
        object Set(IEnumerator<string> currentArgument);
    }
}