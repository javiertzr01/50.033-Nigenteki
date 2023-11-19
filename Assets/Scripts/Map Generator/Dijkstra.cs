using System;
using System.Collections.Generic;
using UnityEngine;

using RTTNode = ProceduralGenerationAlgorithms.RTTNode;

public class Dijkstra 
{
    // Represents a graph as an adjacency list
    private Dictionary<RTTNode, Dictionary<RTTNode, int>> graph; 

    public Dijkstra()
    {
        graph = new Dictionary<RTTNode, Dictionary<RTTNode, int>>();
    }

    // Dijkstra's algorithm to find the furthest node from a given source
    public RTTNode FindFurthestNode(RTTNode source)
    {
        Dictionary<RTTNode, int> distances = new Dictionary<RTTNode, int>();
        foreach (RTTNode vertex in graph.Keys)
        {
            distances[vertex] = int.MaxValue;
        }

        distances[source] = 0;

        PriorityQueue<RTTNode> priorityQueue = new PriorityQueue<RTTNode>();
        priorityQueue.Enqueue(source, 0);

        while (priorityQueue.Count > 0)
        {
            RTTNode currentVertex = priorityQueue.Dequeue();

            foreach (var neighbor in graph[currentVertex])
            {
                int distance = distances[currentVertex] + neighbor.Value;
                if (distance < distances[neighbor.Key])
                {
                    distances[neighbor.Key] = distance;
                    priorityQueue.Enqueue(neighbor.Key, distance);
                }
            }
        }

        // Find the vertex with the maximum distance
        int maxDistance = int.MinValue;
        RTTNode furthestNode = null;

        foreach (var kvp in distances)
        {
            if (kvp.Value > maxDistance)
            {
                maxDistance = kvp.Value;
                furthestNode = kvp.Key;
            }
        }

        return furthestNode;
    }

    public RTTNode FindCommonFurthestNode(RTTNode source, RTTNode furthestNodeFromSource)
    {
        Dictionary<RTTNode, int> distancesFromSource = new Dictionary<RTTNode, int>();
        Dictionary<RTTNode, int> distancesFromFurthestNode = new Dictionary<RTTNode, int>();

        foreach (RTTNode vertex in graph.Keys)
        {
            distancesFromSource[vertex] = int.MaxValue;
            distancesFromFurthestNode[vertex] = int.MaxValue;
        }

        distancesFromSource[source] = 0;
        distancesFromFurthestNode[furthestNodeFromSource] = 0;

        PriorityQueue<RTTNode> priorityQueueFromSource = new PriorityQueue<RTTNode>();
        priorityQueueFromSource.Enqueue(source, 0);

        PriorityQueue<RTTNode> priorityQueueFromFurthestNode = new PriorityQueue<RTTNode>();
        priorityQueueFromFurthestNode.Enqueue(furthestNodeFromSource, 0);

        while (priorityQueueFromSource.Count > 0)
        {
            RTTNode currentVertexFromSource = priorityQueueFromSource.Dequeue();

            foreach (var neighbor in graph[currentVertexFromSource])
            {
                int distance = distancesFromSource[currentVertexFromSource] + neighbor.Value;
                if (distance < distancesFromSource[neighbor.Key])
                {
                    distancesFromSource[neighbor.Key] = distance;
                    priorityQueueFromSource.Enqueue(neighbor.Key, distance);
                }
            }
        }

        while (priorityQueueFromFurthestNode.Count > 0)
        {
            RTTNode currentVertexFromFurthestNode = priorityQueueFromFurthestNode.Dequeue();

            foreach (var neighbor in graph[currentVertexFromFurthestNode])
            {
                int distance = distancesFromFurthestNode[currentVertexFromFurthestNode] + neighbor.Value;
                if (distance < distancesFromFurthestNode[neighbor.Key])
                {
                    distancesFromFurthestNode[neighbor.Key] = distance;
                    priorityQueueFromFurthestNode.Enqueue(neighbor.Key, distance);
                }
            }
        }

        // Find the common node
        int minDifference = int.MaxValue;
        RTTNode equidistantNode = null;

        foreach (var vertex in graph.Keys)
        {
            int difference = Mathf.Abs(distancesFromSource[vertex] - distancesFromFurthestNode[vertex]);

            if (difference < minDifference)
            {
                minDifference = difference;
                equidistantNode = vertex;
            }
        }

        return equidistantNode;
    }

    public void AddToGraph(RTTNode node, Dictionary<RTTNode,int> pathWeights)
    {
        graph.Add(node, pathWeights);
    }
}

// PriorityQueue implementation to efficiently handle vertices with their distances
public class PriorityQueue<T>
{
    private List<Tuple<T, int>> elements = new List<Tuple<T, int>>();

    public int Count => elements.Count;

    public void Enqueue(T item, int priority)
    {
        elements.Add(Tuple.Create(item, priority));
        int index = elements.Count - 1;

        while (index > 0)
        {
            int parentIndex = (index - 1) / 2;

            if (elements[index].Item2 >= elements[parentIndex].Item2)
                break;

            Swap(index, parentIndex);
            index = parentIndex;
        }
    }

    public T Dequeue()
    {
        T result = elements[0].Item1;
        elements[0] = elements[elements.Count - 1];
        elements.RemoveAt(elements.Count - 1);

        int index = 0;
        while (true)
        {
            int leftChildIndex = 2 * index + 1;
            int rightChildIndex = 2 * index + 2;

            if (leftChildIndex >= elements.Count)
                break;

            int minIndex = index;

            if (elements[leftChildIndex].Item2 < elements[minIndex].Item2)
                minIndex = leftChildIndex;

            if (rightChildIndex < elements.Count && elements[rightChildIndex].Item2 < elements[minIndex].Item2)
                minIndex = rightChildIndex;

            if (minIndex == index)
                break;

            Swap(index, minIndex);
            index = minIndex;
        }

        return result;
    }

    private void Swap(int i, int j)
    {
        Tuple<T, int> temp = elements[i];
        elements[i] = elements[j];
        elements[j] = temp;
    }
}
