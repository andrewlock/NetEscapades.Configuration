using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.Configuration;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace NetEscapades.Configuration.Yaml
{
    internal class YamlConfigurationStreamParser
    {
        private readonly Action<DeserializerBuilder> _configureDeserializer;
        private readonly IDictionary<string, string> _data = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly Stack<string> _context = new Stack<string>();

        public YamlConfigurationStreamParser(Action<DeserializerBuilder> configureDeserializer)
        {
            _configureDeserializer = configureDeserializer;
        }

        public IDictionary<string, string> Parse(Stream input)
        {
            _data.Clear();
            _context.Clear();

            using var reader = new StreamReader(input, detectEncodingFromByteOrderMarks: true);
            var parser = new Parser(reader);
            var document = CreateDeserializer().Deserialize(parser);

            if (document is not IDictionary<object, object> documentDict)
            {
                throw new FormatException("Root document must be a dictionary, but got " + document?.GetType().FullName);
            }

            VisitMap(documentDict);

            return _data;
        }

        private IDeserializer CreateDeserializer()
        {
            var builder = new DeserializerBuilder();
            builder.WithAttemptingUnquotedStringTypeDeserialization();
            _configureDeserializer?.Invoke(builder);
            return builder.Build();
        }

        private void VisitMap(IDictionary<object, object> map)
        {
            var isEmpty = true;

            foreach (var entry in map)
            {
                isEmpty = false;
                EnterContext(entry.Key.ToString()!);
                Visit(entry.Value);
                ExitContext();
            }

            SetNullIfElementIsEmpty(isEmpty);
        }

        private void Visit(object value)
        {
            switch (value)
            {
                case IDictionary<object, object> map:
                    VisitMap(map);
                    break;
                case IList<object> sequence:
                    VisitSequence(sequence);
                    break;
                default:
                    VisitScalar(value);
                    break;
            }
        }

        private void VisitScalar(object scalar)
        {
            var currentKey = _context.Peek();
            if (_data.ContainsKey(currentKey))
            {
                throw new FormatException(Resources.FormatError_KeyIsDuplicated(currentKey));
            }

            _data[currentKey] = scalar switch
            {
                null => "",
                bool boolean => boolean ? "true" : "false",
                string str => str,
                // the only remaining type is a number, but there is not a common base class "Number". the best thing to use is IFormattable, which is also used to ensure numbers are formatted correctly
                IFormattable formattable => formattable.ToString("G", CultureInfo.InvariantCulture),
                _ => throw new FormatException($"Unsupported scalar type: {scalar.GetType().Name}")
            };
        }

        private void VisitSequence(IList<object> sequence)
        {
            var i = 0;

            for (; i < sequence.Count; i++)
            {
                EnterContext(i.ToString(CultureInfo.InvariantCulture));
                Visit(sequence[i]);
                ExitContext();
            }

            SetNullIfElementIsEmpty(i == 0);
        }

        private void EnterContext(string context)
        {
            _context.Push(_context.Count > 0 ?
                _context.Peek() + ConfigurationPath.KeyDelimiter + context :
                context);
        }

        private void ExitContext()
        {
            _context.Pop();
        }

        private void SetNullIfElementIsEmpty(bool isEmpty)
        {
            if (isEmpty && _context.Count > 0)
            {
                _data[_context.Peek()] = null;
            }
        }
    }
}
