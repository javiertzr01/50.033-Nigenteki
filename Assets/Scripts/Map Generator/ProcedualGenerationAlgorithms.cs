using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public HashSet<Vector2Int> pathsToChildren = new HashSet<Vector2Int>();
        public Dictionary<RTTNode, int> pathWeight = new Dictionary<RTTNode, int>();
        private int pathCount;
        private int biomeCount;
        [SerializeField]
        private int consecutiveBiomeDistance = 12;
        private int maxConsecutiveBiomeCount = 5;
        public Sprites biome = Sprites.None;

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
            pathCount = 0;
            biomeCount = 0;
            int d = (int)Mathf.Sqrt(Dist(sample));          // Distance between this node and sample
            int length = Mathf.Min(d, max_length);          // Length to move before placing child node
            float diffX = (sample.x - pos.x) / (float)d;    
            float diffY = (sample.y - pos.y) / (float)d;    
            int stepX = pos.x + (int)(diffX * length);      // Child node x position
            int stepY = pos.y + (int)(diffY * length);      // Child node y position
            Vector2Int step = new Vector2Int(stepX, stepY);
            RTTNode child = new RTTNode(step);
            this.GeneratePath(this.pos, child.pos);
            pathWeight.Add(child, pathCount);
            if (this.biome == Sprites.None)
            {
                Debug.Log("Empty Biome at" + this.pos);
            }
            if(pathCount <= consecutiveBiomeDistance && biomeCount < maxConsecutiveBiomeCount)
            {
                child.biome = this.biome;
                biomeCount++;
            }
            else
            {
                child.biome = Biome.GetRandomBiome();
                biomeCount = 0;
            }
            children.Add(child);
        }

        // Recursive Algorithm that generates a path between current node and sample node
        private void GeneratePath(Vector2Int current, Vector2Int sample)
        {
            Vector2Int Xdirection;
            Vector2Int Ydirection;
            Vector2Int newPos = Vector2Int.zero;
            bool moveX = true;
            bool moveY = true;
            pathsToChildren.Add(current);

            if (current != sample)
            {
                int diffX = sample.x - current.x;
                int diffY = sample.y - current.y;
                int totalWeight = Mathf.Abs(diffX) + Mathf.Abs(diffY);

                switch(diffX)
                {
                    case int n when diffX > 0:
                        Xdirection = Vector2Int.right;
                        break;
                    case int n when diffX < 0:
                        Xdirection = Vector2Int.left;
                        break;
                    default:
                        Xdirection = Vector2Int.zero;
                        moveX = false;
                        break;
                }

                switch(diffY)
                {
                    case int n when diffY > 0:
                        Ydirection = Vector2Int.up;
                        break;
                    case int n when diffY < 0:
                        Ydirection = Vector2Int.down;
                        break;
                    default:
                        Ydirection = Vector2Int.zero;
                        moveY = false;
                        break;
                }
                
                if (moveX && !moveY)
                    newPos = current + Xdirection;
                else if (!moveX && moveY)
                    newPos = current + Ydirection;
                else
                {
                    if (Random.Range(0, totalWeight) < Mathf.Abs(diffX))
                        newPos = current + Xdirection;
                    else
                        newPos = current + Ydirection;
                }
                GeneratePath(newPos, sample);
                pathCount++;
            }
            else
            {
                return;
            }
        }

        public HashSet<Vector2Int> GetAllNodePositions()
        {
            HashSet<Vector2Int> positions = new HashSet<Vector2Int>();
            GetAllNodePositionsRecursive(this, positions);
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

        public Dictionary<Vector2Int, RTTNode> GetNodeDictionary()
        {
            Dictionary<Vector2Int, RTTNode> nodeDictionary = new Dictionary<Vector2Int, RTTNode>();
            GetNodeDictionaryRecursive(this, nodeDictionary);
            return nodeDictionary;
        }

        private static void GetNodeDictionaryRecursive(RTTNode node, Dictionary<Vector2Int, RTTNode> dictionary)
        {
            dictionary.Add(node.pos, node);
    
            foreach (RTTNode child in node.children)
            {
                GetNodeDictionaryRecursive(child, dictionary);
            }
        }

        public HashSet<Vector2Int> GetAllPathPositions()
        {
            HashSet<Vector2Int> positions = new HashSet<Vector2Int>();
            GetAllPathPositionsRecursive(this, positions);
            return positions;
        }

        private static void GetAllPathPositionsRecursive(RTTNode node, HashSet<Vector2Int> positions)
        {
            positions.UnionWith(node.pathsToChildren);

            foreach (RTTNode child in node.children)
            {
                GetAllPathPositionsRecursive(child, positions);
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



public enum Sprites
{
    None,
    Path,
    RedSpawn,
    BlueSpawn,
    One,
    Two,
    Three,
    Four,
    Five
}

public static class Biome
{
    public static Sprites GetRandomBiome()
    {
        return (Sprites)Random.Range((int)Sprites.BlueSpawn + 1, Enum.GetValues(typeof(Sprites)).Length);
    }
}