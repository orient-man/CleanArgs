using System;
using System.Collections.Generic;

namespace CSharpArgs
{
    public class StringArgumentMarshaler : IArgumentMarshaler
    {
        private string stringValue = "";

        public void Set(IEnumerator<string> currentArgument)
        {
            try
            {
                stringValue = currentArgument.Next();
            }
            catch (InvalidOperationException)
            {
                throw new ArgsException(ErrorCode.MissingString);
            }
        }

        public static string GetValue(IArgumentMarshaler am)
        {
            var marshaler = am as StringArgumentMarshaler;
            return marshaler != null ? marshaler.stringValue : "";
        }
    }
}