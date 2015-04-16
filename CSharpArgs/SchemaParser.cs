using System.Collections.Generic;
using System.Linq;

namespace CSharpArgs
{
    public static class SchemaParser
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

        // example schema: "l,p#,d*"
        public static IReadOnlyDictionary<char, Marshaler> Parse(string schema)
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
    }
}