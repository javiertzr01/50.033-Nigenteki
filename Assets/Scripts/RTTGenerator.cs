using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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

    // RandomWalkBased
    [SerializeField]
    private int iterations = 10;
    [SerializeField]
    public int walkLength = 50;

    // Information about the map
    public HashSet<Vector2Int> nodePositions = null;
    public Dictionary<Vector2Int, ProceduralGenerationAlgorithms.RTTNode> nodeDictionary = null;
    public HashSet<Vector2Int> pathPositions = null;
    public HashSet<Vector2Int> floorPositions = null;
    public List<Vector2Int> endPoints = null;
    public Dictionary<Vector2Int, HashSet<Vector2Int>> zone = null;
    private Dictionary<Vector2Int, Sprites> spriteMap = null;
    private Dictionary<Vector2Int, string> prefabMap = null; 

    // Information to be passed to Network
    public Vector2Int[] floorPositionsArray = null;
    public Sprites[] spritesArray = null;

    // Testing
    [SerializeField]
    GameObject obstacle;


    protected override void RunProceduralGeneration()
    {
        (nodePositions, nodeDictionary, pathPositions) = RunRTT();
        (floorPositions, zone) = RunRandomWalk(nodePositions);
        floorPositions.UnionWith(pathPositions);                // Make sure that there is a walkable path to all places
        spriteMap = GenerateSpriteMap();
        (floorPositionsArray, spritesArray) = MapDictToArray(spriteMap);
        endPoints = FindEndPoints(nodePositions, pathPositions);
        prefabMap = PlaceItems();
        foreach (var kvp in prefabMap)
        {
            GameObject obs = GameObject.Instantiate(obstacle, new Vector3(kvp.Key.x, kvp.Key.y, 0), Quaternion.identity);
            Debug.Log(kvp.Key + ":" + kvp.Value);
        }
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
            // tilemapVisualizer.PaintFloorTiles(floorPositions);
            tilemapVisualizer.PaintBiomeTiles(floorPositionsArray, spritesArray);
            WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
        }
    }



    protected (HashSet<Vector2Int>, Dictionary<Vector2Int, ProceduralGenerationAlgorithms.RTTNode>, HashSet<Vector2Int>) RunRTT()
    {
        // root = new ProceduralGenerationAlgorithms.RTTNode(RandomSample());
        root = new ProceduralGenerationAlgorithms.RTTNode(Vector2Int.zero);
        root.biome = Biome.GetRandomBiome();
        // startPosition = root.pos;

        for (int i = 0; i < numNodes; i++)
        {
            Vector2Int newPosition = RandomSample();
            (ProceduralGenerationAlgorithms.RTTNode closest, int closestDist) = root.GetClosest(newPosition);
            closest.Grow(newPosition, maxLength);
        }
        return (root.GetAllNodePositions(), root.GetNodeDictionary(), root.GetAllPathPositions());
    }



    private List<Vector2Int> FindEndPoints(HashSet<Vector2Int> nodes, HashSet<Vector2Int> path)
    {
        List<Vector2Int> endPoints = new List<Vector2Int>();
        foreach(var node in nodes)
        {
            int neighboursCount = 0;
            foreach(var direction in Direction2D.cardinalDirectionsList)
            {
                if(path.Contains(node + direction))
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



    protected (HashSet<Vector2Int>, Dictionary<Vector2Int, HashSet<Vector2Int>>) RunRandomWalk(HashSet<Vector2Int> nodePositions)
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        Dictionary<Vector2Int, HashSet<Vector2Int>> zone = new Dictionary<Vector2Int, HashSet<Vector2Int>>();
        foreach(Vector2Int startPosition in nodePositions)
        {
            HashSet<Vector2Int> nodeFloorPositions = new HashSet<Vector2Int>();  // To hold the zone around one node
            for (int i = 0; i < iterations; i++)
            {
                var path = ProceduralGenerationAlgorithms.SimpleRandomWalk(startPosition, walkLength);
                // Add to the hashset containing all the floorpositions
                floorPositions.UnionWith(path);

                // Add to the hashset containing the floorpositions specific to the node
                // Each iteration hold 1 path around zone, we want all iterations but don't want overlapping tiles
                nodeFloorPositions.UnionWith(path);

                // Uncomment the code below if "more spread out" RandomWalk is desired                        
                // if(startRandomlyEachIteration)
                // {
                //     currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
                // }
            }
            zone.Add(startPosition, nodeFloorPositions);
        }
        return (floorPositions, zone);
    }



    protected Dictionary<Vector2Int, Sprites> GenerateSpriteMap()
    {
        Dictionary<Vector2Int, Sprites> map = new Dictionary<Vector2Int, Sprites>();
        foreach (Vector2Int position in pathPositions)
            map[position] = Sprites.Path;                  // Set the sprite for path
        foreach (var kvp in zone)
        {
            Sprites biome = GetBiome(kvp.Key);                              // Get Biome
            foreach (Vector2Int position in kvp.Value)
            {
                map[position] = biome;
            }
        }
        
        return map;
    }



    protected Dictionary<Vector2Int, string> PlaceItems()
    {
        Vector2Int size = new Vector2Int(2,2);
        PlacementType placementType = PlacementType.ColliderActive;   
        int minCount = 3;
        int maxCount = 8;
        string name = "tree";

        Dictionary<Vector2Int, string> PrefabMap = new Dictionary<Vector2Int, string>();
        foreach(var kvp in zone)
        {
            HashSet<Vector2Int> nodeFloorPositions = kvp.Value;
            ItemPlacementHelper itemPlacementHelper = new ItemPlacementHelper(nodeFloorPositions, pathPositions);
            int itemCount = Random.Range(minCount, maxCount + 1);
            for (int i = 0; i < itemCount; i++)
            {
                Vector2Int? position = itemPlacementHelper.GetItemPlacementPosition(placementType, 5, size, true);
                if (position != null)
                    PrefabMap[(Vector2Int)position] = name;
            }
        }
        return PrefabMap;
    }

    private Sprites GetBiome(Vector2Int nodePosition)
    {
        return nodeDictionary[nodePosition].biome;
    }

    private (Vector2Int[], Sprites[]) MapDictToArray(Dictionary<Vector2Int, Sprites> map)
    {
        Vector2Int[] keys = (new List<Vector2Int>(map.Keys)).ToArray();
        Sprites[] values = (new List<Sprites>(map.Values)).ToArray();

        return (keys, values);
    }
}
