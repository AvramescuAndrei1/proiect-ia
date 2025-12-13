using System;
using System.Collections.Generic;
using Proiect_IA.Core;
using Proiect_IA.IO;
using Proiect_IA.Core;
using Project_IA.Inference;
using Proiect_IA.IO;


namespace Proiect_IA
{
    class Program
    {
        static void ExplainQuery(string nodeName, Evidence evidence, BayesianNetwork network)
        {
            Console.WriteLine();
            Console.WriteLine("Explanation:");
            Console.WriteLine("------------");

            var node = network.GetNode(nodeName);

            // Afisam evidenta curenta
            if (evidence.ToDictionary().Count > 0)
            {
                Console.WriteLine("Known evidence:");
                foreach (var kv in evidence.ToDictionary())
                {
                    Console.WriteLine("- " + kv.Key + " = " + kv.Value);
                }
            }
            else
            {
                Console.WriteLine("No evidence is set.");
            }

            Console.WriteLine();

            // Explicatie in functie de parinti
            if (node.Parents.Count == 0)
            {
                Console.WriteLine(
                    "The variable '" + nodeName + "' has no parents."
                );
                Console.WriteLine(
                    "The probabilities are taken directly from its prior distribution."
                );
            }
            else
            {
                Console.WriteLine(
                    "The variable '" + nodeName + "' depends on:"
                );

                foreach (var parent in node.Parents)
                {
                    Console.WriteLine("- " + parent.Name);
                }

                Console.WriteLine();

                bool allParentsKnown = node.Parents.All(p =>
                    evidence.Contains(p.Name)
                );

                if (allParentsKnown)
                {
                    Console.WriteLine(
                        "All parent variables are known."
                    );
                    Console.WriteLine(
                        "The result is taken directly from the conditional probability table."
                    );
                }
                else
                {
                    Console.WriteLine(
                        "Some parent variables are unknown."
                    );
                    Console.WriteLine(
                        "The algorithm uses enumeration to sum over the unknown variables."
                    );
                }
            }

            Console.WriteLine();
        }

        static void PrintAvailableVariables(BayesianNetwork network)
        {
            Console.WriteLine("Available variables and allowed values:");
            Console.WriteLine("---------------------------------------");

            foreach (var node in network.Nodes)
            {
                Console.WriteLine(
                    "- " + node.Name + " {" + string.Join(", ", node.Values) + "}"
                );
            }

            Console.WriteLine();
            Console.WriteLine("Use: set Variable=Value");
            Console.WriteLine("Example: set Studiaza=DA");
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Bayesian Inference - Enumeration");
            Console.WriteLine("--------------------------------");

            // Incarcam reteaua
            Console.Write("Enter network file path: ");
            string path = Console.ReadLine();

            BayesianNetwork network;
            try
            {
                var parser = new NetworkParser();
                network = parser.Parse(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading network: " + ex.Message);
                return;
            }

            var evidence = new Evidence();
            var inference = new EnumerationInference();

            Console.WriteLine("Network loaded successfully.");
            network.PrintPrettyTopology();
            PrintAvailableVariables(network);
            Console.WriteLine("Commands:");
            Console.WriteLine("  set Node=Value");
            Console.WriteLine("  query Node");
            Console.WriteLine("  clear");
            Console.WriteLine("  exit");

            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;

                if (input.Equals("clear", StringComparison.OrdinalIgnoreCase))
                {
                    evidence = new Evidence();
                    Console.WriteLine("Evidence cleared.");
                    continue;
                }

                // Comanda SET
                if (input.StartsWith("set "))
                {
                    try
                    {
                        string assignment = input.Substring(4);
                        var parts = assignment.Split('=');

                        string nodeName = parts[0].Trim();
                        string value = parts[1].Trim();

                        // Verificam daca nodul exista
                        var node = network.GetNode(nodeName);

                        if (!node.Values.Contains(value))
                        {
                            Console.WriteLine(
                                "Invalid value for " + nodeName +
                                ". Allowed values are: " +
                                string.Join(", ", node.Values)
                            );
                            continue;
                        }


                        evidence.Set(nodeName, value);
                        Console.WriteLine("Evidence set: " + nodeName + " = " + value);
                    }
                    catch
                    {
                        Console.WriteLine("Invalid SET command format.");
                    }

                    continue;
                }

                // Comanda QUERY
                if (input.StartsWith("query "))
                {
                    try
                    {
                        string nodeName = input.Substring(6).Trim();
                        var node = network.GetNode(nodeName);

                        var result = inference.EnumerationAsk(
                            nodeName,
                            evidence,
                            network
                        );

                        ExplainQuery(nodeName, evidence, network);
                        Console.WriteLine("Result:");

                        foreach (var kv in result)
                        {
                            Console.WriteLine(
                                "P(" + nodeName + "=" + kv.Key + ") = " +
                                kv.Value.ToString("0.000")
                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Query error: " + ex.Message);
                    }

                    continue;
                }

                Console.WriteLine("Unknown command.");
            }

            Console.WriteLine("Program terminated.");
        }
    }
}

