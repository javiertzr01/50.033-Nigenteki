using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

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

    public HashSet<Vector2Int> nodePositions = null;

    // RandomWalkBased
    [SerializeField]
    private int iterations = 10;
    [SerializeField]
    public int walkLength = 50;

    public HashSet<Vector2Int> floorPositions = null;

    public override void RunProceduralGeneration()
    {
        nodePositions = RunRTT();
        floorPositions = RunRandomWalk(nodePositions);
    }

    public override void ViewMap()
    {
        if (viewNodes && nodePositions != null)
        {
            tilemapVisualizer.PaintFloorTiles(nodePositions);
        }
        else if (floorPositions != null)
        {
            tilemapVisualizer.PaintFloorTiles(floorPositions);
        }
    }

    public override void SaveMap()
    {
        netStore.generatedMapData.Value = new GeneratedMapData
        {
            Size = floorPositions.Count(),
            FloorPositions = floorPositions.ToArray(),
        };

        Logger.Instance.LogInfo("Saved map info: " + netStore.generatedMapData.Value.FloorPositions.Count().ToString());
    }


    public override void LoadMap()
    {
        HashSet<Vector2Int> currentFloorPositions = netStore.generatedMapData.Value.FloorPositions.ToHashSet();
        Logger.Instance.LogInfo("Loading map info: " + currentFloorPositions.Count().ToString());

        tilemapVisualizer.PaintFloorTiles(currentFloorPositions);
    }

    public HashSet<Vector2Int> RunRTT()
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
        return root.GetAllNodePositions();
    }

    public Vector2Int RandomSample()
    {
        return new Vector2Int(Random.Range(-worldSize.x / 2, worldSize.x / 2), Random.Range(-worldSize.y / 2, worldSize.y / 2));
    }

    public HashSet<Vector2Int> RunRandomWalk(HashSet<Vector2Int> nodePosition)
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
