using InputProvider.Graph;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(IPGraphReader))]
public class IPGraphDrawer : Editor
{
     private IPGraphReader ipGraphReader;
    private void OnEnable()
    {
        ipGraphReader = (IPGraphReader)target;
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("inputProvider"));

        EditorGUILayout.PrefixLabel("Current State");
        if(ipGraphReader.currentNode != null)
            EditorGUILayout.LabelField(ipGraphReader.currentNode.name, EditorStyles.label);

        serializedObject.ApplyModifiedProperties();
    }
}
