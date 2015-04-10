using System.Collections.Generic;

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

        private readonly string schema;
        private readonly IEnumerable<string> args;
        private Dictionary<char, Marshaler> marshalers;
        private IEnumerator<string> currentArgument;

        private readonly IDictionary<char, object> values =
            new Dictionary<char, object>();

        public Args(string schema, IEnumerable<string> args)
        {
            this.schema = schema;
            this.args = args;

            Parse();
        }

        private void Parse()
        {
            marshalers = new Dictionary<char, Marshaler>();

            ParseSchema();
            ParseArguments();
        }

        // example schema: "l,p#,d*"
        private void ParseSchema()
        {
            foreach (var element in schema.Split(','))
                if (element.Length > 0)
                    ParseSchemaElement(element.Trim());
        }

        private void ParseSchemaElement(string element)
        {
            var elementId = element[0];
            var elementTail = element.Substring(1);
            ValidateSchemaElementId(elementId);

            Marshaler marshaler;
            if (!Marshalers.TryGetValue(elementTail, out marshaler))
                throw new ArgsException(
                    ErrorCode.InvalidArgumentFormat,
                    elementId,
                    elementTail);

            marshalers.Add(elementId, marshaler);
        }

        private static void ValidateSchemaElementId(char elementId)
        {
            if (!char.IsLetter(elementId))
                throw new ArgsException(ErrorCode.InvalidArgumentName, elementId, null);
        }

        // example arguments: -l -p 4444 -d "C:\Windows\Temp"
        private void ParseArguments()
        {
            currentArgument = args.GetEnumerator();
            while (currentArgument.MoveNext())
            {
                var arg = currentArgument.Current;
                ParseArgument(arg);
            }
        }

        private void ParseArgument(string arg)
        {
            if (arg.StartsWith("-"))
                ParseElements(arg);
        }

        private void ParseElements(string arg)
        {
            for (var i = 1; i < arg.Length; i++)
                ParseElement(arg[i]);
        }

        private void ParseElement(char argChar)
        {
            try
            {
                values.Add(argChar, marshalers[argChar](currentArgument));
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