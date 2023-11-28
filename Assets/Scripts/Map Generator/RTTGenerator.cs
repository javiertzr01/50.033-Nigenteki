using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.Netcode;
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

    public ObstacleInfo redSpawnInfo;
    public ObstacleInfo blueSpawnInfo;
    public ObstacleInfo capturePointInfo;

    // Information to be passed to Network
    public Vector2Int[] floorPositionsArray = null;
    public Sprites[] spritesArray = null;
    public Vector2Int[] obstaclePositionsArray = null;
    public string[] obstacleNamesArray = null;
    public Vector2Int redSpawnPosition;
    public Vector2Int blueSpawnPosition;
    public Vector2Int capturePointPosition;

    // Prefabs
    private List<GameObject> instantiatedPrefabs = new List<GameObject>();
    private List<GameObject> flowersPrefabs = new List<GameObject>();

    public override void RunProceduralGeneration()
    {
        (nodePositions, nodeDictionary, pathPositions) = RunRTT();
        (floorPositions, nodePositionZone, biomeZone) = RunRandomWalk(nodePositions);
        floorPositions.UnionWith(pathPositions);                // Make sure that there is a walkable path to all places
        spriteMap = GenerateSpriteMap();
        (floorPositionsArray, spritesArray) = MapDictToArray(spriteMap);
        endPoints = FindEndPoints(nodePositions, pathPositions);
        (redSpawnPosition, blueSpawnPosition, capturePointPosition) = DeterminePOI();
        ProcessPOI(redSpawnPosition, redSpawnInfo);
        ProcessPOI(blueSpawnPosition, blueSpawnInfo);
        ProcessPOI(capturePointPosition, capturePointInfo);
        prefabMap = PlaceItems();
        (obstaclePositionsArray, obstacleNamesArray) = MapDictToArray(prefabMap);
        SpawnPOI(redSpawnPosition, blueSpawnPosition, capturePointPosition);
        SpawnItems(obstaclePositionsArray, obstacleNamesArray);
        if(IsServer)
        {
            foreach(GameObject flower in flowersPrefabs)
            {
                flower.GetComponent<NetworkObject>().Spawn(true);
            }
        }
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
            FloorPositionsArray = floorPositionsArray,
            SpritesArray = spritesArray,
            ObstaclePositionsArray = obstaclePositionsArray,
            ObstacleNamesArray = obstacleNamesArray,
            RedSpawnPosition = redSpawnPosition,
            BlueSpawnPosition = blueSpawnPosition,
            CapturePointPosition = capturePointPosition
        };

        Logger.Instance.LogInfo("Saved map info, floor position length = " + netStore.generatedMapData.Value.FloorPositionsArray.Length);
    }

    public override void LoadMap()
    {
        Vector2Int[] floorPositionsArray = netStore.generatedMapData.Value.FloorPositionsArray;
        Sprites[] spritesArray = netStore.generatedMapData.Value.SpritesArray;
        Vector2Int[] obstaclePositionsArray = netStore.generatedMapData.Value.ObstaclePositionsArray;
        string[] obstacleNamesArray = netStore.generatedMapData.Value.ObstacleNamesArray;
        Vector2Int redSpawnPosition = netStore.generatedMapData.Value.RedSpawnPosition;
        Vector2Int blueSpawnPosition = netStore.generatedMapData.Value.BlueSpawnPosition;
        Vector2Int capturePointPosition = netStore.generatedMapData.Value.CapturePointPosition;

        Logger.Instance.LogInfo($"Loading map info for client, floor position length = {obstacleNamesArray.Length}");

        tilemapVisualizer.PaintBiomeTiles(floorPositionsArray, spritesArray);
        WallGenerator.CreateWalls(floorPositionsArray.ToHashSet(), tilemapVisualizer);
        SpawnPOI(redSpawnPosition, blueSpawnPosition, capturePointPosition);
        // SpawnItems(obstaclePositionsArray, obstacleNamesArray);
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



    private (Vector2Int, Vector2Int, Vector2Int) DeterminePOI()
    {
        Dijkstra dijkstra = new Dijkstra();
        foreach(var kvp in nodeDictionary)
        {
            ProceduralGenerationAlgorithms.RTTNode node = kvp.Value;
            dijkstra.AddToGraph(node, node.pathWeight);
        }
        int endPointsCount = endPoints.Count;
        Vector2Int redSpawnPosition = endPoints[Random.Range(0, endPointsCount-1)];
        ProceduralGenerationAlgorithms.RTTNode redSpawnNode = nodeDictionary[redSpawnPosition];
        ProceduralGenerationAlgorithms.RTTNode blueSpawnNode = dijkstra.FindFurthestNode(redSpawnNode);
        ProceduralGenerationAlgorithms.RTTNode capturePointNode = dijkstra.FindCommonFurthestNode(redSpawnNode, blueSpawnNode);

        return (redSpawnNode.pos, blueSpawnNode.pos, capturePointNode.pos);
    }



    private void ProcessPOI(Vector2Int position, ObstacleInfo info)
    {
        HashSet<Vector2Int> zonePositions = biomeZone[nodeDictionary[position].biomeRoot.pos];

        List<Vector2Int> positions = new List<Vector2Int>() { position };
        Vector2Int size = info.size;
        int maxX = (size.x - 1)/2;
        int maxY = (size.y - 1)/2;
        int minX = -(size.x - 1)/2;
        int minY = -(size.y - 1)/2;

        for (int row = minX; row <= maxX; row++)
        {
            for (int col = minY; col <= maxY; col++)
            {
                if (col == 0 && row == 0)
                    continue;
                Vector2Int newPosToCheck = new Vector2Int(position.x + row, position.y + col);
                if (!floorPositions.Contains(newPosToCheck))
                {
                    Debug.Log("Error: Hole present, Map Regeneration Required");
                    // regenerate = true;
                }
                positions.Add(newPosToCheck);
            }
        }
        zonePositions.ExceptWith(positions);
        biomeZone[nodeDictionary[position].biomeRoot.pos] = zonePositions;
    }



    public void SpawnPOI(Vector2Int red, Vector2Int blue, Vector2Int cp)
    {
        PrefabLoader.LoadAndInstantiatePrefab("RedSpawn", new Vector3(red.x, red.y, 0));
        PrefabLoader.LoadAndInstantiatePrefab("BlueSpawn", new Vector3(blue.x, blue.y, 0));
        PrefabLoader.LoadAndInstantiatePrefab("CapturePoint", new Vector3(cp.x, cp.y, 0));
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
        // foreach (Vector2Int position in pathPositions)
        //     map[position] = Sprites.Path;                  // Set the sprite for path
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
            // floorPositions.ExceptWith(pathPositions);
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


    public void SpawnItems(Vector2Int[] positionsArray, string[] namesArray)
    {
        (flowersPrefabs, instantiatedPrefabs) = PrefabLoader.LoadAndInstantiatePrefabs(namesArray, positionsArray);
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
