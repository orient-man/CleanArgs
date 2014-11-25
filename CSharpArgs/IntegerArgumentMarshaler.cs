using System;

namespace CSharpArgs
{
    public class IntegerArgumentMarshaler : IArgumentMarshaler
    {
        private int intValue;

        public void Set(Iterator<string> currentArgument)
        {
            String parameter = null;
            try
            {
                parameter = currentArgument.Next();
                intValue = Int32.Parse(parameter);
            }
            catch (NoSuchElementException)
            {
                throw new ArgsException(ErrorCode.MissingInteger);
            }
            catch (FormatException)
            {
                throw new ArgsException(ErrorCode.InvalidInteger, parameter);
            }
        }

        public static int GetValue(IArgumentMarshaler am)
        {
            var marshaler = am as IntegerArgumentMarshaler;
            return marshaler != null ? marshaler.intValue : 0;
        }
    }
}