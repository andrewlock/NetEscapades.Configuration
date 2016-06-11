using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using YamlDotNet.RepresentationModel;

namespace NetEscapades.Extensions.Configuration.Yaml
{
    internal class YamlConfigurationFileParser
    {
        private readonly IDictionary<string, string> _data = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly Stack<string> _context = new Stack<string>();
        private string _currentPath;

        public IDictionary<string, string> Parse(Stream input)
        {
            _data.Clear();

            // https://dotnetfiddle.net/rrR2Bb
            var yaml = new YamlStream();
            yaml.Load(new StreamReader(input));

            // Examine the stream
            var mapping = (YamlMappingNode)yaml.Documents[0].RootNode;

            // The document node is a mapping node
            VisitYamlMappingNode(mapping);

            return _data;
        }

        private void VisitYamlNodePair(KeyValuePair<YamlNode, YamlNode> yamlNodePair)
        {
            if (yamlNodePair.Value is YamlScalarNode)
            {
                VisitYamlScalarNode((YamlScalarNode)yamlNodePair.Key, (YamlScalarNode)yamlNodePair.Value);
            }
            if (yamlNodePair.Value is YamlMappingNode)
            {
                VisitYamlMappingNode((YamlScalarNode)yamlNodePair.Key, (YamlMappingNode)yamlNodePair.Value);
            }
            if (yamlNodePair.Value is YamlSequenceNode)
            {
                VisitYamlSequenceNode((YamlScalarNode)yamlNodePair.Key, (YamlSequenceNode)yamlNodePair.Value);
            }
        }

        private void VisitYamlScalarNode(YamlScalarNode yamlKey, YamlScalarNode yamlValue)
        {
            //a node with a single 1-1 mapping 
            EnterContext(yamlKey.Value);
            var currentKey = _currentPath;

            if (_data.ContainsKey(currentKey))
            {
                throw new FormatException(string.Format(Resources.Error_KeyIsDuplicated, currentKey));
            }

            _data[currentKey] = yamlValue.Value;
            ExitContext();
        }

        private void VisitYamlMappingNode(YamlMappingNode node)
        {
            foreach (var yamlNodePair in node.Children)
            {
                VisitYamlNodePair(yamlNodePair);
            }
        }

        private void VisitYamlMappingNode(YamlScalarNode yamlKey, YamlMappingNode yamlValue)
        {
            //a node with an associated sub-document
            EnterContext(yamlKey.Value);

            VisitYamlMappingNode(yamlValue);

            ExitContext();
        }

        private void VisitYamlSequenceNode(YamlScalarNode yamlKey, YamlSequenceNode yamlValue)
        {
            //a node with an associated list
            EnterContext(yamlKey.Value);

            VisitYamlSequenceNode(yamlValue);

            ExitContext();
        }


        private void VisitYamlSequenceNode(YamlSequenceNode node)
        {
            for (int i = 0; i < node.Children.Count; i++)
            {
                var entry = node.Children[i];
                //create a dummy scalar node for providing the context
                var dummyNode = new YamlScalarNode(i.ToString());
                var nodePair = new KeyValuePair<YamlNode, YamlNode>(dummyNode, entry);

                VisitYamlNodePair(nodePair);
            }
        }


        private void EnterContext(string context)
        {
            _context.Push(context);
            _currentPath = ConfigurationPath.Combine(_context.Reverse());
        }

        private void ExitContext()
        {
            _context.Pop();
            _currentPath = ConfigurationPath.Combine(_context.Reverse());
        }
    }
}
