using System;
using UnityEngine;
using XNode;

namespace InputProvider.Graph
{

    public class IPGraphReader : MonoBehaviour
    {
        [SerializeField] private InputProviderGraph inputProvider;

        public event Action OnStateChanged;

        public BaseNode currentNode { get; private set; }

        private bool startNodeExists = false;
        private bool stopReading = false;

        private string currentState = "NONE";

        private Guid timerId = Guid.NewGuid();

        private void OnEnable()
        {
            startNodeExists = false;
            foreach (BaseNode node in inputProvider.nodes)
            {
                if (node.GetNodeType() == NodeType.start)
                {
                    startNodeExists = true;
                    currentNode = node;
                    break;
                }
            }
        }

        private void Update()
        {
            if (!startNodeExists)
            {
                Debug.LogError("Start Node does not exists, graph reading is impossible");
                return;
            }

            if (stopReading) return;

            ReadCurrentNode();

            if (currentState == "NONE") currentState = currentNode.name;
            if(currentState != currentNode.name)
            {
                currentState = currentNode.name;
                OnStateChanged?.Invoke();
            }
        }

        private void ReadCurrentNode()
        {
            switch (currentNode.GetNodeType())
            {
                case NodeType.start:
                    {
                        FindNextNode("exit");
                        break;
                    }
                case NodeType.wait:
                    {
                        TimerManager.StartTimer(currentNode.GetWaitTime(), "Graph Reader" + timerId,
                            () => FindNextNode("exit"));
                        break;
                    }
                case NodeType.InputMiddleware:
                    {
                        InputMiddlewareNode node = (InputMiddlewareNode)currentNode;
                        if(node.GetOutputPort("exit").ConnectionCount != 0)
                            FindNextNode("exit");
                        break;
                    }
                case NodeType.chance:
                    {
                        ReadChanceNode(); 
                        break;
                    }
                default:
                    {
                        FindNextNode("exit");
                        break;
                    }
            }
        }

        private void FindNextNode(string output)
        {
            bool foudNode = false;
            foreach (NodePort port in currentNode.Ports)
            {
                if (port.fieldName == output && port.Connection != null)
                {
                    currentNode = port.Connection.node as BaseNode;
                    foudNode = true;
                    break;
                }
            }

            if (!foudNode)
            {
                Debug.LogWarning("Next node of " + currentNode.name + " : " + output + " wasn't found. Graph reading stopped");
                stopReading = true;
                return;
            }
        }

        private void ReadChanceNode()
        {
            ChanceNode node = (ChanceNode)currentNode;

            int randOption = RandomUtility.GetRandomWeightedIndex(node.weights);

            FindNextNode(randOption.ToString());
        }

        public void SetStateKey(string key)
        {
            if (currentNode.GetNodeType() != NodeType.InputMiddleware) return;

            InputMiddlewareNode node = (InputMiddlewareNode)currentNode;

            if (!node.stateKeys.Contains(key))
            {
                Debug.LogWarning("current node : "+ node.name +" couldn't find stateKey : " + key);
                return;
            }

            FindNextNode(key);
        }

        public void SetState(string key)
        {
            foreach (InputMiddlewareNode node in inputProvider.nodes)
            {
                if(node.MWName == key)
                {
                    currentNode = node;
                    return;
                }
            }

            Debug.LogWarning("State " + key + " does not exist in the current input provider of " + gameObject.name);
        }

        public string GetCurrentState()
        {
            return currentState;
        }
    }
}