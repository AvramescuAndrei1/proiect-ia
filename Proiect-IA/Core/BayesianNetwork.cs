using System;
using System.Collections.Generic;
using System.Linq;
using Proiect_IA.Core;

namespace Proiect_IA.Core
{
    public class BayesianNetwork
    {
        public List<Node> Nodes { get; set; }
        public BayesianNetwork()
        {
            Nodes = new List<Node>();
        }
        public Node GetNode(string name)
        {
            var node = Nodes.FirstOrDefault(n => n.Name == name);
            if (node == null)
            {
                throw new Exception("Node not found: " + name);
            }
            return node;
        }
        public void AddNode(Node node)
        {
            if (Nodes.Any(n => n.Name == node.Name))
            {
                throw new Exception("Duplicate node: " + node.Name);
            }

            Nodes.Add(node);
        }
        public List<Node> GetTopologicalOrder()
        {
            var result = new List<Node>();
            var visited = new HashSet<Node>();

            foreach (var node in Nodes)
            {
                Visit(node, visited, result);
            }

            return result;
        }
        private void Visit(Node node, HashSet<Node> visited, List<Node> result)
        {
            if (visited.Contains(node))
                return;

            visited.Add(node);

            foreach (var parent in node.Parents)
            {
                Visit(parent, visited, result);
            }

            if (!result.Contains(node))
            {
                result.Add(node);
            }
        }
        public void PrintPrettyTopology()
        {
            Console.WriteLine();
            Console.WriteLine("Network Topology (Layers View):");
            Console.WriteLine("-------------------------------");

            var nodeDepths = new Dictionary<string, int>();
            foreach (var node in Nodes)
            {
                CalculateDepth(node, nodeDepths);
            }

            var levels = nodeDepths.GroupBy(x => x.Value)
                                   .OrderBy(k => k.Key)
                                   .ToDictionary(g => g.Key, g => g.Select(x => x.Key).ToList());

            int maxLevel = levels.Keys.Max();
            int consoleWidth = 80;

            for (int i = 0; i <= maxLevel; i++)
            {
                if (!levels.ContainsKey(i)) continue;

                var nodesInLevel = levels[i];

                string line = "";
                foreach (var nodeName in nodesInLevel)
                {
                    line += $"[{nodeName}]   ";
                }
                Console.WriteLine(CenterText(line.TrimEnd(), consoleWidth));

                if (i < maxLevel && levels.ContainsKey(i + 1))
                {
                    var nextLevelCount = levels[i + 1].Count;
                    var currentLevelCount = nodesInLevel.Count;

                    if (currentLevelCount > 1 && nextLevelCount == 1)
                    {
                        Console.WriteLine(CenterText("      \\       /      ", consoleWidth));
                        Console.WriteLine(CenterText("       \\     /       ", consoleWidth));
                        Console.WriteLine(CenterText("          V          ", consoleWidth));
                    }
                    else if (currentLevelCount == 1 && nextLevelCount > 1)
                    {
                        Console.WriteLine(CenterText("          |          ", consoleWidth));
                        Console.WriteLine(CenterText("       /     \\       ", consoleWidth));
                        Console.WriteLine(CenterText("      v       v      ", consoleWidth));
                    }
                    else
                    {
                        Console.WriteLine(CenterText("          |          ", consoleWidth));
                        Console.WriteLine(CenterText("          V          ", consoleWidth));
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine("Detailed Connections:");
            foreach (var node in Nodes)
            {
                foreach (var parent in node.Parents)
                {
                    Console.WriteLine($" {parent.Name} -> {node.Name}");
                }
            }
            Console.WriteLine();
        }

        private int CalculateDepth(Node node, Dictionary<string, int> depths)
        {
            if (depths.ContainsKey(node.Name)) return depths[node.Name];

            if (node.Parents.Count == 0)
            {
                depths[node.Name] = 0;
                return 0;
            }

            int maxParentDepth = 0;
            foreach (var parent in node.Parents)
            {
                int pDepth = CalculateDepth(parent, depths);
                if (pDepth > maxParentDepth) maxParentDepth = pDepth;
            }

            depths[node.Name] = maxParentDepth + 1;
            return maxParentDepth + 1;
        }

        private string CenterText(string text, int width)
        {
            if (text.Length >= width) return text;
            int leftPadding = (width - text.Length) / 2;
            return new string(' ', leftPadding) + text;
        }


    }
}
