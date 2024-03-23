using System;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace InputProvider.Graph
{
    public class IPTreeReader : MonoBehaviour
    {
        [SerializeField] private InputProviderGraph ipGraph;

        private Dictionary<string, Dictionary<string, bool>> stateKeysList = 
            new Dictionary<string, Dictionary<string, bool>>();

        private bool startNodeExists = false;
        private BaseNode startNode;
        public event Action<string> onStateKeyChanged;

        private void OnEnable()
        {
            foreach (BaseNode node in ipGraph.nodes)
            {
                if (node.GetNodeType() != NodeType.InputMiddleware) continue;

                InputMiddlewareNode inputMiddleware = (InputMiddlewareNode)node;
                if (stateKeysList.ContainsKey(inputMiddleware.MWName)) continue;

                Dictionary<string, bool> stateKeySwitches = new Dictionary<string, bool>();
                foreach (string key in ((InputMiddlewareNode)node).stateKeys)
                {
                    if (stateKeySwitches.Count == 0)
                        stateKeySwitches.Add(key, true);
                    else
                        stateKeySwitches.Add(key, false);
                }
                stateKeysList.Add(((InputMiddlewareNode)node).MWName, stateKeySwitches);
            }

            startNodeExists = false;
            foreach (BaseNode node in ipGraph.nodes)
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
                foreach (string key in stateKeysList[currentIMNode.MWName].Keys)
                {
                    if (stateKeysList[currentIMNode.MWName][key] == true)
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
            if (!stateKeysList.ContainsKey(MWName))
            {
                Debug.LogWarning("InputMiddleware " + MWName + " does not exist in the current input provider of " + gameObject.name);
                return;
            }
            if (!stateKeysList[MWName].ContainsKey(stateKey))
            {
                Debug.LogWarning("State key " + stateKey + " of " + MWName + "wasn't found");
                return;
            }

            List<string> keys = new List<string>();
            foreach (string key in stateKeysList[MWName].Keys)
            {
                keys.Add(key);
            }
            foreach (string key in keys)
            {
                stateKeysList[MWName][key] = false;
            }

            stateKeysList[MWName][stateKey] = true;
            onStateKeyChanged?.Invoke(stateKey);
        }

        public string GetIMSwitchesState(string MWName) //Return Input Middleware Active switch/statekey
        {
            if (!stateKeysList.ContainsKey(MWName))
            {
                Debug.LogWarning("InputMiddleware " + MWName + " does not exist in the current input provider of " + gameObject.name);
                return "";
            }

            string res = "";

            foreach (string key in stateKeysList[MWName].Keys)
            {
                if (stateKeysList[MWName][key])
                    res = key;
            }

            return res;
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

            Debug.LogWarning("Next node of " + node.name + " : " + output + " wasn't found.");
            return null;
        }
    }
}
