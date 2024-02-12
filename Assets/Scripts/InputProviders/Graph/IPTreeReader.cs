using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using XNode;

namespace InputProvider.Graph
{
    public class IPTreeReader : MonoBehaviour
    {
        [SerializeField] private InputProviderGraph inputProvider;

        private Dictionary<string, Dictionary<string, bool>> stateKeys = 
            new Dictionary<string, Dictionary<string, bool>>();

        private bool startNodeExists = false;
        private BaseNode startNode;

        private void OnEnable()
        {
            foreach (BaseNode node in inputProvider.nodes)
            {
                if (node.GetNodeType() != NodeType.InputMiddleware) continue;

                InputMiddlewareNode inputMiddleware = (InputMiddlewareNode)node;
                if (stateKeys.ContainsKey(inputMiddleware.MWName)) continue;

                Dictionary<string, bool> stateKeySwitches = new Dictionary<string, bool>();
                foreach (string key in ((InputMiddlewareNode)node).stateKeys)
                {
                    if (stateKeySwitches.Count == 0)
                        stateKeySwitches.Add(key, true);
                    else
                        stateKeySwitches.Add(key, false);
                }
                stateKeys.Add(((InputMiddlewareNode)node).MWName, stateKeySwitches);
            }

            startNodeExists = false;
            foreach (BaseNode node in inputProvider.nodes)
            {
                if (node.GetNodeType() == NodeType.start)
                {
                    startNodeExists = true;
                    startNode = node;
                    break;
                }
            }
        }

        public string ResolveGraph()
        {
            if (!startNodeExists)
            {
                Debug.LogError("Start Node does not exists, graph reading is impossible");
                return "-1";
            }

            BaseNode currentNode = FindNextNode(startNode, "exit");
            if (currentNode.GetNodeType() != NodeType.InputMiddleware) return "-1";

            InputMiddlewareNode currentIMNode = (InputMiddlewareNode)currentNode;
            while (currentIMNode.stateKeys.Count != 0)
            {
                string nextNode = "";
                foreach (string key in stateKeys[currentIMNode.MWName].Keys)
                {
                    if (stateKeys[currentIMNode.MWName][key] == true)
                    {
                        nextNode = key;
                        break;
                    }
                }

                currentNode = FindNextNode(currentIMNode, nextNode);
                if (currentNode.GetNodeType() != NodeType.InputMiddleware) return "-1";
                currentIMNode = (InputMiddlewareNode)currentNode;
            }

            return currentIMNode.MWName;
        }

        public void SwitchStateKey(string MWName, string stateKey)
        {
            if (!stateKeys.ContainsKey(MWName)) return;
            if (!stateKeys[MWName].ContainsKey(stateKey)) return;

            List<string> keys = new List<string>();
            foreach (string key in stateKeys[MWName].Keys)
            {
                keys.Add(key);
            }
            foreach (string key in keys)
            {
                stateKeys[MWName][key] = false;
            }

            stateKeys[MWName][stateKey] = true;
        }

        private BaseNode FindNextNode(BaseNode node, string output)
        {
            foreach (NodePort port in node.Ports)
            {
                if (port.fieldName == output && port.Connection != null)
                {
                    node = port.Connection.node as BaseNode;
                    return node;
                }
            }

            Debug.LogWarning("Next node of " + node.name + " : '" + output + "' wasn't found.");
            return null;
        }
    }
}
