using System;
using System.Collections.Generic;
using Core;
using Priority_Queue;
using UnityEngine;

namespace Utils
{
    public static class Shortcut
    {
        public static void CalculateAll()
        {
            var selected = Interaction.Instance.SelectedSquareNames;
            if (selected.Count < 1) throw new Exception("Please select more than 1 blocks you want to calculate");

            // Interaction.Instance.ShortestDistance = new Dictionary<string, float>();
            Interaction.Instance.ShortestDistance = new Dictionary<string, Dictionary<string, float>>();

            for (var i = 0; i != selected.Count-1; ++i)
            {
                var start = selected[i];
                Interaction.Instance.ShortestDistance[start] = new Dictionary<string, float>();
                for (var j = i+1; j != selected.Count; ++j)
                {
                    var end = selected[j];
                    Interaction.Instance.ShortestDistance[start][end] = Shortest(start, selected[j]);
                }
            }

            // for (var j = selected.Count-1; j <= 0; --j)
            // {
            //     var shortest = new Dictionary<string, float>();
            //     for (var i = 0; i != j; ++i)
            //     {
            //         shortest[selected[i]] = Interaction.Instance.ShortestDistance[selected[i]][selected[j]];
            //     }
            //
            //     Interaction.Instance.ShortestDistance[selected[j]] = shortest;
            // }
        }

        private static float Shortest(string start, string end)
        {
            var distances = new Dictionary<string, float>();
            foreach (var pair in Interaction.Instance.Squares)
            {
                distances[pair.Key] = float.MaxValue;
            }

            distances[start] = 0;
            var priorityQueue = new SimplePriorityQueue<KeyValuePair<string, float>>();
            priorityQueue.Enqueue(new KeyValuePair<string, float>(start,0), 0);
            while (priorityQueue.Count > 0)
            {
                var (currentVertex, currentDistance) = priorityQueue.Dequeue();

                if (currentDistance > distances[currentVertex]) continue;

                foreach (var (neighbor, weight) in Interaction.Instance.Squares[currentVertex].GetAdjacentSquares())
                {
                    var w = weight;
                    if (Interaction.Instance.SelectedSquareNames.Contains(neighbor) && neighbor != end) 
                        w = float.MaxValue;
                    
                    var distance = currentDistance + w;

                    if (distance < distances[neighbor])
                    {
                        distances[neighbor] = distance;
                        priorityQueue.Enqueue(new KeyValuePair<string, float>(neighbor, distance), distance);
                    }
                }

            }
            return distances[end];
        }
            
    }
}