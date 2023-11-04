using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTTGenerator : AbstractProceduralGenerator
{
    // RTT based
    protected ProceduralGenerationAlgorithms.RTTNode root;

    [SerializeField]
    protected Vector2Int worldSize = new Vector2Int(512, 512);
    [SerializeField]
    protected int numNodes = 50;
    [SerializeField]
    protected int maxLength = 10;

    private HashSet<Vector2Int> nodePositions = null;
    private HashSet<Vector2Int> pathPositions = null;

    // RandomWalkBased
    [SerializeField]
    private int iterations = 10;
    [SerializeField]
    public int walkLength = 50;

    private HashSet<Vector2Int> floorPositions = null;
    private List<Vector2Int> endPoints = null;


    protected override void RunProceduralGeneration()
    {
        (nodePositions, pathPositions) = RunRTT();
        floorPositions = RunRandomWalk(nodePositions);
        endPoints = FindEndPoints(nodePositions, pathPositions);
    }

    protected override void ViewMap()
    {
        if (viewNodes && nodePositions != null)
        {
            tilemapVisualizer.PaintFloorTiles(nodePositions);
        }
        else if (viewPaths && pathPositions != null)
        {
            tilemapVisualizer.PaintFloorTiles(pathPositions);
        }
        else if (floorPositions != null)
        {
            tilemapVisualizer.PaintFloorTiles(floorPositions);
            WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
        }
    }

    protected (HashSet<Vector2Int>, HashSet<Vector2Int>) RunRTT()
    {
        // root = new ProceduralGenerationAlgorithms.RTTNode(RandomSample());
        root = new ProceduralGenerationAlgorithms.RTTNode(Vector2Int.zero);
        // startPosition = root.pos;

        for (int i = 0; i < numNodes; i++)
        {
            Vector2Int newPosition = RandomSample();
            (ProceduralGenerationAlgorithms.RTTNode closest, int closestDist) = root.GetClosest(newPosition);
            closest.Grow(newPosition, maxLength);
        }
        return (root.GetAllNodePositions(), root.GetAllPathPositions());
    }

    private List<Vector2Int> FindEndPoints(HashSet<Vector2Int> nodePositions, HashSet<Vector2Int> pathPositions)
    {
        List<Vector2Int> endPoints = new List<Vector2Int>();
        foreach(var node in nodePositions)
        {
            int neighboursCount = 0;
            foreach(var direction in Direction2D.cardinalDirectionsList)
            {
                if(pathPositions.Contains(node + direction))
                    neighboursCount++;
            }
            if (neighboursCount == 1)
            {
                endPoints.Add(node);
            }
        }
        return endPoints;
    }

    private Vector2Int RandomSample()
    {
        return new Vector2Int(Random.Range(-worldSize.x / 2, worldSize.x / 2), Random.Range(-worldSize.y / 2, worldSize.y / 2));
    }

    protected HashSet<Vector2Int> RunRandomWalk(HashSet<Vector2Int> nodePosition)
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        foreach(Vector2Int startPosition in nodePosition)
        {
            for (int i = 0; i < iterations; i++)
            {
                var path = ProceduralGenerationAlgorithms.SimpleRandomWalk(startPosition, walkLength);
                floorPositions.UnionWith(path);
                // if(startRandomlyEachIteration)
                // {
                //     currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
                // }
            }
        }
        return floorPositions;
    }
}
