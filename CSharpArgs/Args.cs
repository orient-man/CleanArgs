using System.Collections.Generic;

namespace CSharpArgs
{
    public class Args
    {
        private readonly string schema;
        private readonly IEnumerable<string> args;
        private Dictionary<char, IArgumentMarshaler> marshalers;
        private HashSet<char> argsFound;
        private IEnumerator<string> currentArgument;

        public Args(string schema, IEnumerable<string> args)
        {
            this.schema = schema;
            this.args = args;

            Parse();
        }

        private void Parse()
        {
            marshalers = new Dictionary<char, IArgumentMarshaler>();
            argsFound = new HashSet<char>();

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
            if (elementTail.Length == 0)
                marshalers.Add(elementId, new BooleanArgumentMarshaler());
            else if (elementTail == "*")
                marshalers.Add(elementId, new StringArgumentMarshaler());
            else if (elementTail == "#")
                marshalers.Add(elementId, new IntegerArgumentMarshaler());
            else if (elementTail == "##")
                marshalers.Add(elementId, new DoubleArgumentMarshaler());
            else
                throw new ArgsException(
                    ErrorCode.InvalidArgumentFormat,
                    elementId,
                    elementTail);
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
                var m = marshalers[argChar];
                argsFound.Add(argChar);
                m.Set(currentArgument);
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
            return argsFound.Count;
        }

        public bool Has(char arg)
        {
            return argsFound.Contains(arg);
        }

        public bool GetBoolean(char arg)
        {
            return BooleanArgumentMarshaler.GetValue(marshalers[arg]);
        }

        public string GetString(char arg)
        {
            return StringArgumentMarshaler.GetValue(marshalers[arg]);
        }

        public int GetInt(char arg)
        {
            return IntegerArgumentMarshaler.GetValue(marshalers[arg]);
        }

        public double GetDouble(char arg)
        {
            return DoubleArgumentMarshaler.GetValue(marshalers[arg]);
        }
    }
}