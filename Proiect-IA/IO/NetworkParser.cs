using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Proiect_IA.Core;

namespace Proiect_IA.IO
{
    // Citeste o retea bayesiana din fisier text
    public class NetworkParser
    {
        public BayesianNetwork Parse(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new Exception("Network file not found: " + filePath);
            }

            var network = new BayesianNetwork();
            var nodes = new Dictionary<string, Node>();

            var lines = File.ReadAllLines(filePath)
                            .Select(l => l.Trim())
                            .Where(l => l.Length > 0)
                            .ToList();

            int index = 0;

            // Nodes
            if (lines[index] != "NODES:")
                throw new Exception("Expected NODES section");
            index++;

            while (!lines[index].StartsWith("EDGES:"))
            {
                var parts = lines[index].Split(' ', 2);
                string nodeName = parts[0];
                string valuesPart = parts[1]
                    .Replace("{", "")
                    .Replace("}", "");

                var values = valuesPart.Split(',')
                                        .Select(v => v.Trim())
                                        .ToList();

                var node = new Node(nodeName);
                node.Values.AddRange(values);

                nodes[nodeName] = node;
                network.AddNode(node);

                index++;
            }

            // Edges
            index++;

            while (!lines[index].StartsWith("CPT:"))
            {
                var parts = lines[index].Split("->");
                string parentName = parts[0].Trim();
                string childName = parts[1].Trim();

                Node parent = nodes[parentName];
                Node child = nodes[childName];

                child.Parents.Add(parent);

                index++;
            }

            // CPT
            index++;

            while (index < lines.Count)
            {
                string line = lines[index];
                if (line.EndsWith(":") && !line.Contains("|"))
                {
                    string nodeName = line.Replace(":", "");
                    Node node = nodes[nodeName];
                    node.CPT[""] = new Dictionary<string, double>();

                    index++;
                    while (index < lines.Count && lines[index].Contains("="))
                    {
                        var parts = lines[index].Split('=');
                        string value = parts[0].Trim();
                        double prob = double.Parse(parts[1].Trim());

                        node.CPT[""][value] = prob;
                        index++;
                    }
                }
                else if (line.Contains("|"))
                {
                    var headerParts = line.Split('|');
                    string nodeName = headerParts[0].Trim();
                    Node node = nodes[nodeName];

                    index++;
                    while (index < lines.Count && lines[index].Contains("->"))
                    {
                        var parts = lines[index].Split("->");
                        string parentConfig = parts[0]
                             .Replace(" ", "")
                             .Trim();


                        var probParts = parts[1].Split(',');
                        var dist = new Dictionary<string, double>();

                        foreach (var p in probParts)
                        {
                            var pv = p.Split(':');
                            string value = pv[0].Trim();
                            double prob = double.Parse(pv[1].Trim());
                            dist[value] = prob;
                        }

                        node.CPT[parentConfig] = dist;
                        index++;
                    }
                }
                else
                {
                    index++;
                }
            }

            return network;
        }
    }
}
