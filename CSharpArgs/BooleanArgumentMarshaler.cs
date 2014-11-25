namespace CSharpArgs
{
    public class BooleanArgumentMarshaler : IArgumentMarshaler
    {
        private bool booleanValue;

        public void Set(Iterator<string> currentArgument)
        {
            booleanValue = true;
        }

        public static bool GetValue(IArgumentMarshaler am)
        {
            var marshaler = am as BooleanArgumentMarshaler;
            return marshaler != null && marshaler.booleanValue;
        }
    }
}