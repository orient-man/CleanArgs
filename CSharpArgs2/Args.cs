using System.Collections.Generic;
using System.Linq;

namespace CSharpArgs2
{
    public class Args
    {
        private delegate dynamic Marshaler(IEnumerator<string> currentArgument);

        private static readonly IReadOnlyDictionary<string, Marshaler>
            Marshalers =
                new Dictionary<string, Marshaler>
                {
                    { "", BoolArgumentMarshaler.Marshal },
                    { "*", StringArgumentMarshaler.Marshal },
                    { "#", IntArgumentMarshaler.Marshal },
                    { "##", DoubleArgumentMarshaler.Marshal }
                };

        private readonly IReadOnlyDictionary<char, dynamic> values;

        public Args(string schema, IEnumerable<string> args)
        {
            values = ParseArguments(args, ParseSchema(schema));
        }

        private static IReadOnlyDictionary<char, Marshaler> ParseSchema(string schema)
        {
            return schema
                .Split(',')
                .Select(o => o.Trim())
                .Where(o => o.Length > 0)
                .Select(o => new { ElementId = o[0], ElementTail = o.Substring(1) })
                .ToDictionary(
                    o => o.ElementId,
                    o => ParseSchemaElement(o.ElementId, o.ElementTail));
        }

        private static Marshaler ParseSchemaElement(
            char elementId,
            string elementTail)
        {
            ValidateSchemaElementId(elementId);

            Marshaler marshaler;
            if (!Marshalers.TryGetValue(elementTail, out marshaler))
                throw new ArgsException(
                    ErrorCode.InvalidArgumentFormat,
                    elementId,
                    elementTail);

            return marshaler;
        }

        private static void ValidateSchemaElementId(char elementId)
        {
            if (!char.IsLetter(elementId))
                throw new ArgsException(ErrorCode.InvalidArgumentName, elementId);
        }

        private static IReadOnlyDictionary<char, dynamic> ParseArguments(
            IEnumerable<string> args,
            IReadOnlyDictionary<char, Marshaler> marshalers)
        {
            var values = new Dictionary<char, dynamic>();
            var currentArgument = args.GetEnumerator();
            while (currentArgument.MoveNext())
                foreach (var arg in FindElements(currentArgument.Current))
                    values[arg] = ParseArgument(arg, currentArgument, marshalers);

            return values;
        }

        private static IEnumerable<char> FindElements(string arg)
        {
            return arg.StartsWith("-") ? arg.Skip(1) : Enumerable.Empty<char>();
        }

        private static dynamic ParseArgument(
            char arg,
            IEnumerator<string> currentArgument,
            IReadOnlyDictionary<char, Marshaler> marshalers)
        {
            try
            {
                return marshalers[arg](currentArgument);
            }
            catch (KeyNotFoundException)
            {
                throw new ArgsException(ErrorCode.UnexpectedArgument, arg);
            }
            catch (ArgsException e)
            {
                e.ErrorArgumentId = arg;
                throw;
            }
        }

        public int Cardinality()
        {
            return values.Count;
        }

        public T Get<T>(char arg, T defaultValue = default(T))
        {
            dynamic value;
            if (!values.TryGetValue(arg, out value))
                return defaultValue;

            return value is T ? (T)value : defaultValue;
        }

        public bool Has(char arg)
        {
            return values.ContainsKey(arg);
        }
    }
}