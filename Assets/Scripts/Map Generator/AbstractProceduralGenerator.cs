using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class AbstractProceduralGenerator : NetworkBehaviour
{
    public NetworkStore netStore;

    [SerializeField]
    protected TilemapVisualizer tilemapVisualizer = null;
    [SerializeField]
    protected Vector2Int startPosition = Vector2Int.zero;
    [SerializeField]
    public bool viewNodes = false;
    public bool viewPaths = false;

    public void GenerateMap()
    {
        tilemapVisualizer.Clear();
        RunProceduralGeneration();
        ViewMap();
    }

    public void VisualizeMap()
    {
        tilemapVisualizer.Clear();
        ViewMap();
    }

    public void SaveMapInstance()
    {
        SaveMap();
    }

    public void LoadMapForClient()
    {
        tilemapVisualizer.Clear();
        LoadMap();
        ViewMap();
    }

    public abstract void RunProceduralGeneration();
    public abstract void ViewMap();
    public abstract void SaveMap();
    public abstract void LoadMap();
}
