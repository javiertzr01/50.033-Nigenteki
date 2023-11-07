using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class ProceduralGenerationAlgorithms
{

    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int walkLength)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();

        path.Add(startPosition);
        var previousPosition = startPosition;

        for (int i = 0; i < walkLength; i++)
        {
            var newPosition = previousPosition + Direction2D.GetRandomCardinalDirection();
            path.Add(newPosition);
            previousPosition = newPosition;
        }
        return path;
    }

    public class RTTNode
    {
        public Vector2Int pos;
        public List<RTTNode> children;

        public RTTNode(Vector2Int position)
        {
            pos = position;
            children = new List<RTTNode>();
        }

        public int Dist(Vector2Int sample)
        {
            int xDiff = pos.x - sample.x;
            int yDiff = pos.y - sample.y;
            return xDiff * xDiff + yDiff * yDiff;
        }

        public (RTTNode, int) GetClosest(Vector2Int sample)
        {
            int closestDist = Dist(sample);
            RTTNode closest = this;

            foreach (RTTNode child in children)
            {
                var (closestChild, dist) = child.GetClosest(sample);

                if (dist < closestDist)
                {
                    closest = closestChild;
                    closestDist = dist;
                }
            }

            return (closest, closestDist);
        }

        public void Grow(Vector2Int sample, int max_length)
        {
            int d = (int)Mathf.Sqrt(Dist(sample));          // Distance between this node and sample
            int length = Mathf.Min(d, max_length);          // Length to move before placing child node
            float diffX = (sample.x - pos.x) / (float)d;    
            float diffY = (sample.y - pos.y) / (float)d;    
            int stepX = pos.x + (int)(diffX * length);      // Child node x position
            int stepY = pos.y + (int)(diffY * length);      // Child node y position
            Vector2Int step = new Vector2Int(stepX, stepY);
            RTTNode child = new RTTNode(step);

            children.Add(child);
        }

        public HashSet<Vector2Int> GetAllNodePositions()
        {
            HashSet<Vector2Int> positions = new HashSet<Vector2Int>();
            GetAllNodePositionsRecursive(this, positions);
            // foreach (RTTNode child in children)
            // {
            //     GetAllNodePositionsRecursive(child, positions);
            // }
            return positions;
        }
    
        private static void GetAllNodePositionsRecursive(RTTNode node, HashSet<Vector2Int> positions)
        {
            positions.Add(node.pos);
    
            foreach (RTTNode child in node.children)
            {
                GetAllNodePositionsRecursive(child, positions);
            }
        }

    }

}



public static class Direction2D
{
    public static List<Vector2Int> cardinalDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0,1), // UP
        new Vector2Int(1,0), // RIGHT
        new Vector2Int(0,-1), // DOWN
        new Vector2Int(-1,0) // LEFT
    };

    public static Vector2Int GetRandomCardinalDirection()
    {
        return cardinalDirectionsList[Random.Range(0, cardinalDirectionsList.Count)];
    }
}
