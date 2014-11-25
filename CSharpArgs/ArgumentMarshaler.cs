using System;

namespace CSharpArgs
{
    public interface IArgumentMarshaler
    {
        void Set(Iterator<String> currentArgument);
    }
}