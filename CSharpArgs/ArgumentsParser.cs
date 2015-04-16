using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpArgs
{
    public static class ArgumentsParser
    {
        // example arguments: -l -p 4444 -d "C:\Windows\Temp"
        public static IEnumerable<Tuple<char, object>> Parse(
            IEnumerator<string> args,
            IReadOnlyDictionary<char, Marshaler> schema)
        {
            while (args.MoveNext())
                foreach (var el in FindElements(args.Current))
                    yield return Tuple.Create(el, ParseElement(el, args, schema));
        }

        private static IEnumerable<char> FindElements(string arg)
        {
            return arg.StartsWith("-") ? arg.Skip(1) : Enumerable.Empty<char>();
        }

        private static object ParseElement(
            char argChar,
            IEnumerator<string> currentArgument,
            IReadOnlyDictionary<char, Marshaler> schema)
        {
            try
            {
                return schema[argChar](currentArgument);
            }
            catch (KeyNotFoundException)
            {
                throw new ArgsException(ErrorCode.UnexpectedArgument, argChar, null);
            }
            catch (ArgsException e)
            {
                e.ErrorArgumentId = argChar;
                throw;
            }
        }
    }
}