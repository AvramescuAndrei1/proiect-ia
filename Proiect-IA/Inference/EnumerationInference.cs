using System;
using System.Collections.Generic;
using System.Linq;
using Proiect_IA.Core;

namespace Project_IA.Inference
{
    // Implementarea inferentei prin enumerare
    public class EnumerationInference
    {
         //Algoritmul ENUMERATION-ASK
        public Dictionary<string, double> EnumerationAsk(
            string queryVariable,
            Evidence evidence,
            BayesianNetwork network)
        {
            var result = new Dictionary<string, double>();

            Node queryNode = network.GetNode(queryVariable);
            var orderedNodes = network.GetTopologicalOrder();

            foreach (var value in queryNode.Values)
            {
                var extendedEvidence = new Evidence();
                foreach (var kv in evidence.ToDictionary())
                {
                    extendedEvidence.Set(kv.Key, kv.Value);
                }

                extendedEvidence.Set(queryVariable, value);

                double prob = EnumerateAll(
                    orderedNodes,
                    extendedEvidence.ToDictionary()
                );

                result[value] = prob;
            }

            Normalize(result);

            return result;
        }

         //Algoritmul ENUMERATE-ALL recursiv
        private double EnumerateAll(
            List<Node> vars,
            Dictionary<string, string> evidence)
        {
            if (vars.Count == 0)
            {
                return 1.0;
            }
            Node Y = vars[0];
            var rest = vars.Skip(1).ToList();

            if (evidence.ContainsKey(Y.Name))
            {
                double prob = Y.Probability(
                    evidence[Y.Name],
                    evidence
                );

                return prob * EnumerateAll(rest, evidence);
            }
            else
            {
                double sum = 0.0;

                foreach (var y in Y.Values)
                {
                    var extendedEvidence = new Dictionary<string, string>(evidence);
                    extendedEvidence[Y.Name] = y;

                    double prob = Y.Probability(y, extendedEvidence);

                    sum += prob * EnumerateAll(rest, extendedEvidence);
                }

                return sum;
            }
        }
        private void Normalize(Dictionary<string, double> distribution)
        {
            double sum = distribution.Values.Sum();

            if (sum == 0)
            {
                throw new Exception("Cannot normalize distribution with sum = 0");
            }

            var keys = distribution.Keys.ToList();
            foreach (var key in keys)
            {
                distribution[key] = distribution[key] / sum;
            }
        }
    }
}
