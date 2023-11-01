using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractProceduralGenerator : MonoBehaviour
{
    [SerializeField]
    protected TilemapVisualizer tilemapVisualizer = null;
    [SerializeField]
    protected Vector2Int startPosition = Vector2Int.zero;
    [SerializeField]
    public bool viewNodes = false;

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

    protected abstract void RunProceduralGeneration();
    protected abstract void ViewMap();
}
