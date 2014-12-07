using System.Collections.Generic;

namespace CSharpArgs
{
    public interface IArgumentMarshaler
    {
        void Set(IEnumerator<string> currentArgument);
    }
}