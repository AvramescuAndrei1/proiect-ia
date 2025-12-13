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
            Console.WriteLine("Bayesian Network Topology");
            Console.WriteLine("-------------------------");
            Console.WriteLine();

            Console.WriteLine("Studiaza        Dificultate");
            Console.WriteLine("     \\              /");
            Console.WriteLine("      \\            /");
            Console.WriteLine("           Nota");
            Console.WriteLine("             |");
            Console.WriteLine("        Recomandare");
            Console.WriteLine();
        }


    }
}
