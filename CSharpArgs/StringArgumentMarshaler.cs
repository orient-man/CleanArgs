using System;

namespace CSharpArgs
{
    public class StringArgumentMarshaler : IArgumentMarshaler
    {
        private String stringValue = "";

        public void Set(Iterator<String> currentArgument)
        {
            try
            {
                stringValue = currentArgument.Next();
            }
            catch (NoSuchElementException)
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