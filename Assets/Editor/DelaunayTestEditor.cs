using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DelaunayTest))]
public class DelaunayTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DelaunayTest delaunayTest = (DelaunayTest) target;
        
        GUILayout.Space(10f);

        GUILayout.Label("Delaunay Triangulation", EditorStyles.boldLabel);
        if (GUILayout.Button("New Triangulation", GUILayout.Width(150)))
            delaunayTest.GenerateTriangulation();
    }
}