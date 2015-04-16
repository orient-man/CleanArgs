using System.Collections.Generic;
using System.Linq;

namespace CSharpArgs
{
    public /* immutable */ class Args
    {
        private readonly IReadOnlyDictionary<char, object> values;

        public Args(string schema, IEnumerable<string> args)
        {
            values =
                ArgumentsParser.Parse(args.GetEnumerator(), SchemaParser.Parse(schema))
                    .ToDictionary(o => o.Item1, o => o.Item2);
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