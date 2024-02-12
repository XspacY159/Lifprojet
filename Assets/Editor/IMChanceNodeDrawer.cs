using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace InputProvider.Graph
{
    [CustomNodeEditor(typeof(ChanceNode))]
    public class IMChanceNodeDrawer : NodeEditor
    {
        private ChanceNode node;

        private bool showInputProp = false;

        SerializedProperty outputs;
        ReorderableList outputList;

        public override void OnCreate()
        {
            base.OnCreate();
            if (node == null) node = target as ChanceNode;

            outputs = serializedObject.FindProperty("weights");
            outputList = new ReorderableList(serializedObject, outputs, false, true, false, false);

            outputList.drawHeaderCallback = DrawOptionHeader;
            outputList.drawElementCallback = DrawOption;
        }

        public override void OnBodyGUI()
        {
            serializedObject.Update();

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("entry"));

            showInputProp = EditorGUILayout.BeginFoldoutHeaderGroup(showInputProp, "Properties");

            if (showInputProp)
            {
                EditorGUILayout.PrefixLabel("Options");

                float totalPercentage = 0;
                for (int i = 0; i < node.weights.Count; i++)
                {
                    totalPercentage += node.weights[i];

                    if (totalPercentage > 100)
                        node.weights[i] = Mathf.Clamp(node.weights[i], 0, 
                            node.weights[i] - (totalPercentage - 100));

                    if (node.weights[i] < 0)
                        node.weights[i] = 0;
                }                

                if (GUILayout.Button("Create new Option"))
                {
                    node.AddDynamicOutput(typeof(int), Node.ConnectionType.Multiple,
                        Node.TypeConstraint.None, node.DynamicOutputs.Count().ToString());
                    node.weights.Add(0);
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
            EditorGUI.LabelField(rect, "Options", EditorStyles.boldLabel);

            EditorGUI.LabelField(new Rect(rect.x + 60, rect.y, rect.width, rect.height), "Weights", EditorStyles.label);
        }

        private void DrawOption(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty output = outputList.serializedProperty.GetArrayElementAtIndex(index);
            NodePort port = node.DynamicOutputs.ToArray()[index];

            EditorGUI.PropertyField(new Rect(rect.x + 60, rect.y, 30, EditorGUIUtility.singleLineHeight),
                 output, GUIContent.none);
            NodeEditorGUILayout.PortField(new Vector2(rect.width + 10, rect.y), port);

            if (port.fieldName != index.ToString())
            {
                List<NodePort> prevPorts = new List<NodePort>();
                for (int i = 0; i < port.ConnectionCount; i++)
                {
                    prevPorts.Add(port.GetConnection(i));
                }
                node.RemoveDynamicPort(port);
                port = node.AddDynamicOutput(typeof(int), Node.ConnectionType.Multiple,
                    Node.TypeConstraint.None, index.ToString());
                for (int i = 0; i < prevPorts.Count; i++)
                {
                    port.Connect(prevPorts[i]);
                }
            }

            if (GUI.Button(new Rect(rect.x, rect.y, 50,
                EditorGUIUtility.singleLineHeight), "Delete"))
            {
                node.RemoveDynamicPort(port);
                for (int i = 0; i < node.weights.Count; i++)
                {
                    if (port.fieldName == i.ToString())
                    {
                        node.weights.RemoveAt(i);
                        break;
                    }
                }
            }
        }
    }
}
