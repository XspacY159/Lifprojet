using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(TimerManager))]
public class TimerManagerDrawer : Editor
{
    private bool showTimers = false;

    private Dictionary<string, Timer> timers = new Dictionary<string, Timer>();

    private void OnEnable()
    {
        EditorApplication.update += Update;
    }

    private void OnDisable()
    {
        EditorApplication.update -= Update;
    }

    private void Update()
    {
        if (showTimers)
            Repaint();
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        showTimers = EditorGUILayout.BeginFoldoutHeaderGroup(showTimers, "Timers : ");

        if(showTimers)
        {
            timers = TimerManager.GetTimers();
            foreach (string key in timers.Keys)
            {
                string text = key;
                if (text.Length > 20) text = key.Substring(0, 20) + "...";
                EditorGUILayout.LabelField(text + " : " + timers[key].counter, EditorStyles.label);
            }
        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        serializedObject.ApplyModifiedProperties();
    }
}
