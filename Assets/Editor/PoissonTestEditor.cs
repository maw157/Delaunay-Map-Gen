using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PoissonTest))]
public class PoissonTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PoissonTest poissonTest = (PoissonTest) target;
        
        GUILayout.Space(10f);

        GUILayout.Label("Poisson Disk Sampler", EditorStyles.boldLabel);
        if (GUILayout.Button("Regenerate Points", GUILayout.Width(150)))
            poissonTest.GeneratePoints();
    }
}