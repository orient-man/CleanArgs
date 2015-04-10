using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpArgs
{
    public class Args
    {
        private static readonly IReadOnlyDictionary<string, Marshaler>
            Marshalers =
                new Dictionary<string, Marshaler>
                {
                    { "", BooleanArgumentMarshaler.Marshal },
                    { "*", StringArgumentMarshaler.Marshal },
                    { "#", IntegerArgumentMarshaler.Marshal },
                    { "##", DoubleArgumentMarshaler.Marshal }
                };

        private readonly IReadOnlyDictionary<char, object> values;

        public Args(string schema, IEnumerable<string> args)
        {
            values = ParseArguments(args.GetEnumerator(), ParseSchema(schema))
                .ToDictionary(o => o.Item1, o => o.Item2);
        }

        // example schema: "l,p#,d*"
        private static IReadOnlyDictionary<char, Marshaler> ParseSchema(string schema)
        {
            return schema.Split(',')
                .Select(o => o.Trim())
                .Where(o => o.Length > 0)
                .Select(o => new { id = o[0], format = o.Substring(1) })
                .ToDictionary(o => o.id, o => ParseSchemaElement(o.id, o.format));
        }

        private static Marshaler ParseSchemaElement(char id, string format)
        {
            ValidateSchemaElementId(id);

            Marshaler marshaler;
            if (!Marshalers.TryGetValue(format, out marshaler))
                throw new ArgsException(
                    ErrorCode.InvalidArgumentFormat,
                    id,
                    format);

            return marshaler;
        }

        private static void ValidateSchemaElementId(char elementId)
        {
            if (!char.IsLetter(elementId))
                throw new ArgsException(ErrorCode.InvalidArgumentName, elementId, null);
        }

        // example arguments: -l -p 4444 -d "C:\Windows\Temp"
        private static IEnumerable<Tuple<char, object>> ParseArguments(
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

        public int Cardinality()
        {
            return values.Count;
        }

        public bool Has(char arg)
        {
            return values.ContainsKey(arg);
        }

        public T Get<T>(char arg)
        {
            object value;
            if (!values.TryGetValue(arg, out value))
                return default(T);

            return value is T ? (T)value : default(T);
        }
    }
}