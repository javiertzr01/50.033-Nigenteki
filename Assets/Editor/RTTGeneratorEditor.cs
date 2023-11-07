using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AbstractProceduralGenerator), true)]

public class RTTGeneratorEditor : Editor
{
    AbstractProceduralGenerator generator;

    private void Awake()
    {
        generator = (AbstractProceduralGenerator) target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Generate Map"))
        {
            generator.GenerateMap();
        }
        if (GUILayout.Toggle(generator.viewPaths, "Paths"))
        {
            generator.viewPaths = true;
            generator.viewNodes = false;
            generator.VisualizeMap();
        }
        else if(GUILayout.Toggle(generator.viewNodes, "Nodes Only"))
        {
            generator.viewNodes = true;
            generator.viewPaths =false;
            generator.VisualizeMap();
        }
        else
        {
            generator.viewNodes = false;
            generator.viewPaths =false;
            generator.VisualizeMap();
        }
    }
}
