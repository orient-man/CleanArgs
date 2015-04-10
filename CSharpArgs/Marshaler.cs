using System.Collections.Generic;

namespace CSharpArgs
{
    public delegate object Marshaler(IEnumerator<string> currentArgument);
}