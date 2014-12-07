using System;
using System.Collections.Generic;

namespace CSharpArgs
{
    public class StringArgumentMarshaler : IArgumentMarshaler
    {
        private String stringValue = "";

        public void Set(IEnumerator<String> currentArgument)
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

        public static String GetValue(IArgumentMarshaler am)
        {
            var marshaler = am as StringArgumentMarshaler;
            return marshaler != null ? marshaler.stringValue : "";
        }
    }
}