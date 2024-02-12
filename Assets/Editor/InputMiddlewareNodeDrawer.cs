using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace InputProvider.Graph
{
    [CustomNodeEditor(typeof(InputMiddlewareNode))]
    public class InputMiddlewareNodeDrawer : NodeEditor
    {
        private InputMiddlewareNode node;

        private string newStateKey = "";

        private bool showInputProp = false;

        SerializedProperty outputs;
        ReorderableList outputList;

        public override void OnCreate()
        {
            base.OnCreate();
            if (node == null) node = target as InputMiddlewareNode;

            outputs = serializedObject.FindProperty("stateKeys");
            outputList = new ReorderableList(serializedObject, outputs, false, true, false, false);

            outputList.drawHeaderCallback = DrawOptionHeader;
            outputList.drawElementCallback = DrawOption;
        }

        public override void OnBodyGUI()
        {
            serializedObject.Update();

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("entry"));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("exit"));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("MWName"));
            node.name = node.MWName;

            showInputProp = EditorGUILayout.BeginFoldoutHeaderGroup(showInputProp, "Middleware Properties");

            if (showInputProp)
            {
                EditorGUILayout.PrefixLabel("StateKey");
                newStateKey = EditorGUILayout.TextField(newStateKey);

                if (GUILayout.Button("Create new StateKey"))
                {
                    bool noInput = (newStateKey.Length == 0);

                    if (noInput)
                    {
                        EditorUtility.DisplayDialog("Error creating port", "No StateKey was specified ", "OK");
                        return;
                    }

                    bool matchesExistingOutput = false;

                    foreach (NodePort port in node.DynamicOutputs)
                    {
                        if (port.fieldName == newStateKey)
                        {
                            matchesExistingOutput = true;
                            break;
                        }
                    }
                    if (matchesExistingOutput)
                    {
                        EditorUtility.DisplayDialog("Error creating port", "Requesting port is already in use", "OK");
                        return;
                    }

                    node.AddDynamicOutput(typeof(int), Node.ConnectionType.Multiple,
                        Node.TypeConstraint.None, newStateKey);
                    node.stateKeys.Add(newStateKey);
                }

                outputList.DoLayoutList();
            }
            else
            {
                foreach (NodePort port in node.DynamicOutputs)
                {
                    NodeEditorGUILayout.PortField(port);
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawOptionHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "StateKeys", EditorStyles.boldLabel);
        }

        private void DrawOption(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty output = outputList.serializedProperty.GetArrayElementAtIndex(index);
            NodePort port = node.DynamicOutputs.ToArray()[index];

            EditorGUI.LabelField(new Rect(rect.x + 55, rect.y, 50, EditorGUIUtility.singleLineHeight),
                output.stringValue);

            int occurence = 0;
            for (int i = 0; i < node.stateKeys.Count; i++)
            {
                if (i == index) continue;
                if(node.stateKeys[index] == node.stateKeys[i])
                {
                    Debug.LogWarning("Error renaming port, Requesting port is already in use");
                    occurence++;
                    node.stateKeys[index] = node.stateKeys[index] + " (" + occurence + ")";

                    List<NodePort> prevPorts = new List<NodePort>();
                    for (int j = 0; j < port.ConnectionCount; j++)
                    {
                        prevPorts.Add(port.GetConnection(j));
                    }
                    node.RemoveDynamicPort(port);
                    port = node.AddDynamicOutput(typeof(int), Node.ConnectionType.Multiple,
                        Node.TypeConstraint.None, node.stateKeys[index]);
                    for (int j = 0; j < prevPorts.Count; j++)
                    {
                        port.Connect(prevPorts[j]);
                    }
                    return;
                }
            }

            NodeEditorGUILayout.PortField(new Vector2(rect.width + 10, rect.y), port);

            if (port.fieldName != node.stateKeys[index])
            {
                List<NodePort> prevPorts = new List<NodePort>();
                for (int i = 0; i < port.ConnectionCount; i++)
                {
                    prevPorts.Add(port.GetConnection(i));
                }
                node.RemoveDynamicPort(port);
                port = node.AddDynamicOutput(typeof(int), Node.ConnectionType.Multiple,
                    Node.TypeConstraint.None, node.stateKeys[index]);
                for (int i = 0; i < prevPorts.Count; i++)
                {
                    port.Connect(prevPorts[i]);
                }
            }

            if (GUI.Button(new Rect(rect.x, rect.y, 50,
                EditorGUIUtility.singleLineHeight), "Delete"))
            {
                node.RemoveDynamicPort(port);
                foreach (string statekey in node.stateKeys)
                {
                    if (port.fieldName == statekey)
                    {
                        node.stateKeys.Remove(statekey);
                        break;
                    }
                }
            }
        }
    }
}
