using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UnitTests))]
public class UnitTestsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UnitTests unitTests = (UnitTests) target;


        GUILayout.Space(10f);
        GUILayout.Label("Helper Methods", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Test Vector2ToPoint", GUILayout.Width(200)))
            unitTests.Test_Vector2ToPoint();

        if (GUILayout.Button("Test Quicksort", GUILayout.Width(200)))
            unitTests.Test_Quicksort();

        if (GUILayout.Button("Test isOnRightSide", GUILayout.Width(200)))
            unitTests.Test_isOnRightSide();

        if (GUILayout.Button("Test pointIsRightOfEdge", GUILayout.Width(200)))
            unitTests.Test_pointIsRightOfEdge();

        if (GUILayout.Button("Test ReverseEdge", GUILayout.Width(200)))
            unitTests.Test_ReverseEdge();

        if (GUILayout.Button("Test ConstructTriangle", GUILayout.Width(200)))
            unitTests.Test_ConstructTriangle();

        if (GUILayout.Button("Test pointInCircumcircle", GUILayout.Width(200)))
            unitTests.Test_pointInCircumcircle();

        if (GUILayout.Button("Test GetCircumcenter", GUILayout.Width(200)))
            unitTests.Test_GetCircumcenter();

        if (GUILayout.Button("Test GetCircumradius", GUILayout.Width(200)))
            unitTests.Test_GetCircumradius();

        if (GUILayout.Button("Test GetTrianglesFromEdge", GUILayout.Width(200)))
            unitTests.Test_GetTrianglesFromEdge();

        if (GUILayout.Button("Test edgeExistsInDT", GUILayout.Width(200)))
            unitTests.Test_edgeExistsInDT();

        
        GUILayout.Space(10f);
        GUILayout.Label("Core Algorithm Tests", EditorStyles.boldLabel);

        if (GUILayout.Button("Test notLegal", GUILayout.Width(200)))
            unitTests.Test_notLegal();
        
        if (GUILayout.Button("Test Flip", GUILayout.Width(200)))
            unitTests.Test_Flip();

        if (GUILayout.Button("Test Legalize", GUILayout.Width(200)))
            unitTests.Test_Legalize();

        if (GUILayout.Button("Test Triangulate2DVerbose", GUILayout.Width(200)))
            unitTests.Test_Triangulate2DVerbose();

        if (GUILayout.Button("Test Triangulate2D", GUILayout.Width(200)))
            unitTests.Test_Triangulate2D();

        
        GUILayout.Space(10f);
        GUILayout.Label("Data Structure Methods", EditorStyles.boldLabel);

        if (GUILayout.Button("Test TriangleEquals", GUILayout.Width(200)))
            unitTests.Test_TriangleEquals();

        if (GUILayout.Button("Test EdgeEquals", GUILayout.Width(200)))
            unitTests.Test_EdgeEquals();

        if (GUILayout.Button("Test PointEquals", GUILayout.Width(200)))
            unitTests.Test_PointEquals();
    }
}