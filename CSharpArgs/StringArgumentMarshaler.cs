using System.Collections.Generic;

namespace CSharpArgs
{
    public class StringArgumentMarshaler : IArgumentMarshaler
    {
        private string stringValue = "";

        public void Set(IEnumerator<string> currentArgument)
        {
            if (!currentArgument.MoveNext())
                throw new ArgsException(ErrorCode.MissingString);

            stringValue = currentArgument.Current;
        }

        public static string GetValue(IArgumentMarshaler am)
        {
            var marshaler = am as StringArgumentMarshaler;
            return marshaler != null ? marshaler.stringValue : "";
        }
    }
}