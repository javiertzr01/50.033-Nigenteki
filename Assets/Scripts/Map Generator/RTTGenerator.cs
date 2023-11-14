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
    public Dictionary<Vector2Int, HashSet<Vector2Int>> nodePositionZone = null;
    public Dictionary<Vector2Int, HashSet<Vector2Int>> biomeZone = null;
    private Dictionary<Vector2Int, Sprites> spriteMap = null;
    private Dictionary<Vector2Int, string> prefabMap = null; 

    public BiomeInfo green1;
    public BiomeInfo green2;
    public BiomeInfo green3;
    public BiomeInfo autumn;
    public BiomeInfo sakura;

    // Information to be passed to Network
    public Vector2Int[] floorPositionsArray = null;
    public Sprites[] spritesArray = null;
    public Vector2Int[] obstaclePositionsArray = null;
    public string[] obstacleNamesArray = null;

    // Testing
    GameObject obstacle;
    [SerializeField]
    public GameObject AutumnTree2x2;
    public GameObject Green1Tree2x2;
    public GameObject Green2Tree2x2;
    public GameObject Green3Tree2x2;
    public GameObject SakuraTree2x2;
    public GameObject AutumnTree5x5;
    public GameObject Green1Tree5x5;
    public GameObject Green2Tree5x5;
    public GameObject Green3Tree5x5;
    public GameObject SakuraTree5x5;
    

    public override void RunProceduralGeneration()
    {
        (nodePositions, nodeDictionary, pathPositions) = RunRTT();
        (floorPositions, nodePositionZone, biomeZone) = RunRandomWalk(nodePositions);
        floorPositions.UnionWith(pathPositions);                // Make sure that there is a walkable path to all places
        spriteMap = GenerateSpriteMap();
        (floorPositionsArray, spritesArray) = MapDictToArray(spriteMap);
        endPoints = FindEndPoints(nodePositions, pathPositions);
        prefabMap = PlaceItems();
        (obstaclePositionsArray, obstacleNamesArray) = MapDictToArray(prefabMap);
        SpawnItems(obstaclePositionsArray, obstacleNamesArray);
    }



    public override void ViewMap()
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



    protected (HashSet<Vector2Int>, Dictionary<Vector2Int, HashSet<Vector2Int>>, Dictionary<Vector2Int, HashSet<Vector2Int>>) RunRandomWalk(HashSet<Vector2Int> nodePositions)
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        Dictionary<Vector2Int, HashSet<Vector2Int>> nodePositionZone = new Dictionary<Vector2Int, HashSet<Vector2Int>>();
        Dictionary<Vector2Int, HashSet<Vector2Int>> biomeZone = new Dictionary<Vector2Int, HashSet<Vector2Int>>();
        foreach(Vector2Int startPosition in nodePositions)
        {
            HashSet<Vector2Int> nodeFloorPositions = new HashSet<Vector2Int>();  // To hold the zone around one node
            for (int i = 0; i < iterations; i++)
            {
                var path = ProceduralGenerationAlgorithms.SimpleRandomWalk(startPosition, walkLength);

                // Make sure no overlaps
                path.ExceptWith(floorPositions);

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
            ProceduralGenerationAlgorithms.RTTNode node = nodeDictionary[startPosition];
            if (biomeZone.ContainsKey(node.biomeRoot.pos))
            {
                biomeZone[node.biomeRoot.pos].UnionWith(nodeFloorPositions);
            }
            else
            {
                biomeZone.Add(node.biomeRoot.pos, nodeFloorPositions);
            }
            nodePositionZone.Add(startPosition, nodeFloorPositions);
        }
        return (floorPositions, nodePositionZone, biomeZone);
    }



    protected Dictionary<Vector2Int, Sprites> GenerateSpriteMap()
    {
        Dictionary<Vector2Int, Sprites> map = new Dictionary<Vector2Int, Sprites>();
        foreach (Vector2Int position in pathPositions)
            map[position] = Sprites.Path;                  // Set the sprite for path
        foreach (var kvp in nodePositionZone)
        {
            Sprites biome = GetBiome(kvp.Key);                              // Get Biome
            foreach (Vector2Int position in kvp.Value)
            {
                if (!map.ContainsKey(position))
                    map[position] = biome;
            }
        }
        
        return map;
    }



    protected Dictionary<Vector2Int, string> PlaceItems()
    {
        // Object placement
        Dictionary<Vector2Int, string> PrefabMap = new Dictionary<Vector2Int, string>();
        foreach(var kvp in biomeZone)
        {
            HashSet<Vector2Int> floorPositions = kvp.Value;
            floorPositions.ExceptWith(pathPositions);
            Sprites biome = spriteMap[floorPositions.First()];
            BiomeInfo biomeInfo;
            switch (biome)
            {
                case Sprites.Green1:
                    biomeInfo = green1;
                    break;
                case Sprites.Green2:
                    biomeInfo = green2;
                    break;
                case Sprites.Green3:
                    biomeInfo = green3;
                    break;
                case Sprites.Autumn:
                    biomeInfo = autumn;
                    break;
                case Sprites.Sakura:
                    biomeInfo = sakura;
                    break;
                default:
                    biomeInfo = null;
                    break;
            }
            if (biomeInfo == null)
            {
                Debug.Log("Error: Null Biome Info");
            }
            ItemPlacementHelper itemPlacementHelper = new ItemPlacementHelper(floorPositions, pathPositions);
            foreach(var item in biomeInfo.items)
            {
                int itemCount = Random.Range(item.minCount, item.maxCount + 1);
                for (int i = 0; i < itemCount; i++)
                {
                    Vector2Int? position = itemPlacementHelper.GetItemPlacementPosition(item.itemData.placementType, 500, item.itemData.size, true);
                    if (position != null)
                        PrefabMap[(Vector2Int)position] = biome.ToString()+item.itemData.name;
                }
            }
        }
        return PrefabMap;
    }


    public void SpawnItems(Vector2Int[] obstaclePositionsArray, string[] obstacleNamesArray)
    {
        for(int i = 0; i < obstaclePositionsArray.Count(); i++)
        {
            switch(obstacleNamesArray[i])
            {
                case "Autumntree2x2":
                    obstacle = AutumnTree2x2;
                    break;
                case "Green1tree2x2":
                    obstacle = Green1Tree2x2;
                    break;
                case "Green2tree2x2":
                    obstacle = Green2Tree2x2;
                    break;
                case "Green3tree2x2":
                    obstacle = Green3Tree2x2;
                    break;
                case "Sakuratree2x2":
                    obstacle = SakuraTree2x2;
                    break;
                case "Autumntree5x5":
                    obstacle = AutumnTree5x5;
                    break;
                case "Green1tree5x5":
                    obstacle = Green1Tree5x5;
                    break;
                case "Green2tree5x5":
                    obstacle = Green2Tree5x5;
                    break;
                case "Green3tree5x5":
                    obstacle = Green3Tree5x5;
                    break;
                case "Sakuratree5x5":
                    obstacle = SakuraTree5x5;
                    break;
                default:
                    Debug.Log("No Prefab");
                    break;
            }
            GameObject obs = GameObject.Instantiate(obstacle, new Vector3(obstaclePositionsArray[i].x, obstaclePositionsArray[i].y, 0), Quaternion.identity);
        }
            // Debug.Log(kvp.Key + ":" + kvp.Value);
    }


    private Sprites GetBiome(Vector2Int nodePosition)
    {
        return nodeDictionary[nodePosition].biome;
    }

    private (TKey[], TValue[]) MapDictToArray<TKey, TValue>(Dictionary<TKey, TValue> map)
    {
        TKey[] keys = (new List<TKey>(map.Keys)).ToArray();
        TValue[] values = (new List<TValue>(map.Values)).ToArray();

        return (keys, values);
    }
}
