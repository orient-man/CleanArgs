using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpArgs2
{
    public class Args
    {
        private static readonly IReadOnlyDictionary<string, Func<IArgumentMarshaler>>
            Marshalers =
                new Dictionary<string, Func<IArgumentMarshaler>>
                {
                    { "", () => new BoolArgumentMarshaler() },
                    { "*", () => new StringArgumentMarshaler() },
                    { "#", () => new IntArgumentMarshaler() },
                    { "##", () => new DoubleArgumentMarshaler() }
                };

        private readonly IReadOnlyDictionary<char, object> values;

        public Args(string schema, IEnumerable<string> args)
        {
            values = ParseArguments(args.GetEnumerator(), ParseSchema(schema));
        }

        private static IReadOnlyDictionary<char, IArgumentMarshaler> ParseSchema(
            string schema)
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

        private static IArgumentMarshaler ParseSchemaElement(
            char elementId,
            string elementTail)
        {
            ValidateSchemaElementId(elementId);
            try
            {
                return Marshalers[elementTail]();
            }
            catch (KeyNotFoundException)
            {
                throw new ArgsException(
                    ErrorCode.InvalidArgumentFormat,
                    elementId,
                    elementTail);
            }
        }

        private static void ValidateSchemaElementId(char elementId)
        {
            if (!char.IsLetter(elementId))
                throw new ArgsException(ErrorCode.InvalidArgumentName, elementId);
        }

        private static IReadOnlyDictionary<char, object> ParseArguments(
            IEnumerator<string> currentArgument,
            IReadOnlyDictionary<char, IArgumentMarshaler> marshalers)
        {
            var values = new Dictionary<char, object>();
            while (currentArgument.MoveNext())
                foreach (var arg in FindElements(currentArgument.Current))
                    values[arg] = ParseArgument(arg, currentArgument, marshalers);

            return values;
        }

        private static IEnumerable<char> FindElements(string arg)
        {
            return arg.StartsWith("-") ? arg.Skip(1) : Enumerable.Empty<char>();
        }

        private static object ParseArgument(
            char arg,
            IEnumerator<string> currentArgument,
            IReadOnlyDictionary<char, IArgumentMarshaler> marshalers)
        {
            try
            {
                return marshalers[arg].Marshal(currentArgument);
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

        public string GetString(char arg)
        {
            return GetValue(arg, "");
        }

        public int GetInt(char arg)
        {
            return GetValue(arg, 0);
        }

        public bool GetBoolean(char arg)
        {
            return GetValue(arg, false);
        }

        public double GetDouble(char arg)
        {
            return GetValue(arg, 0.0);
        }

        private T GetValue<T>(char arg, T defaultValue)
        {
            object value;
            if (!values.TryGetValue(arg, out value))
                return defaultValue;

            return value == null ? defaultValue : (T)value;
        }

        public bool Has(char arg)
        {
            return values.ContainsKey(arg);
        }
    }
}