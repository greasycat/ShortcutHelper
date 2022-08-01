using System.Collections.Generic;
using System.Linq;
using Priority_Queue;
using UnityEngine;

public class Shortcut
{
    public static void Calculate()
    {
        var selected = Interaction.Instance.SelectedSquareNames;
        if (selected.Count < 2) return;

        Interaction.Instance.ShortestDistance = new Dictionary<string, float>();

        for (var i = 0; i != selected.Count-1; ++i)
        {
            var start = selected[i];
            var end = selected[i+1];
            
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
                    var distance = currentDistance + weight;

                    if (distance < distances[neighbor])
                    {
                        distances[neighbor] = distance;
                        priorityQueue.Enqueue(new KeyValuePair<string, float>(neighbor, distance), distance);
                    }
                }

            }

            Debug.Log(distances[end]);
            Interaction.Instance.ShortestDistance[end] = distances[end];
        }

        Interaction.Instance.ShortestDistance[selected[0]] = 0f;

    }
}