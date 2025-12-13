using System;
using System.Collections.Generic;
using System.Linq;

namespace Proiect_IA.Core

{
    public class Node
    {
        public string Name { get; set; }
        public List<string> Values { get; set; }
        public List<Node> Parents { get; set; }
        public Dictionary<string, Dictionary<string, double>> CPT { get; set; }

        public Node(string name)
        {
            Name = name;
            Values = new List<string>();
            Parents = new List<Node>();
            CPT = new Dictionary<string, Dictionary<string, double>>();
        }

        /*
         Calculeaza probabilitatea:
         P(this = value | parintii )
        */
        public double Probability(string value, Dictionary<string, string> evidence)
        {
            // Daca nodul nu are parinti, folosim cheia vida ""
            if (Parents.Count == 0)
            {
                if (!CPT.ContainsKey(""))
                {
                    throw new Exception("CPT missing root entry for node " + Name);
                }

                return CPT[""][value];
            }

            // Construim cheia pentru CPT pe baza parintilor
            var keyParts = new List<string>();

            foreach (var parent in Parents)
            {
                if (!evidence.ContainsKey(parent.Name))
                {
                    throw new Exception(
                        "Missing evidence for parent " + parent.Name + " of node " + Name
                    );
                }

                keyParts.Add(parent.Name + "=" + evidence[parent.Name]);
            }

            string key = string.Join(",", keyParts);

            if (!CPT.ContainsKey(key))
            {
                throw new Exception(
                    "CPT entry not found for key " + key +
                    " in node " + Name
                );
            }

            return CPT[key][value];
        }
    }
}
