using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

namespace InputProvider
{
    public class InputProviderBase<StateType> : MonoBehaviour
    {
        [HideInInspector]
        public List<InputMiddleware<StateType>> inputMiddlewares = new List<InputMiddleware<StateType>>();

        public SerializedDictionary<string, bool> triggerkeys = new SerializedDictionary<string, bool>();

        public virtual void Initialize(GameObject Agent)
        {

        }

        public virtual void SetKeyState(string key, bool value)
        {
            foreach (InputMiddleware<StateType> inputMiddleware in inputMiddlewares)
            {
                if (inputMiddleware.statekeys.ContainsKey(key))
                    inputMiddleware.statekeys[key] = value;
            }
        }

        public virtual bool GetKeyState(string key)
        {
            foreach (InputMiddleware<StateType> inputMiddleware in inputMiddlewares)
            {
                if (inputMiddleware.statekeys.ContainsKey(key))
                    return inputMiddleware.statekeys[key];
            }
            Debug.LogWarning("The state key " + key + " have not been found");
            return false;
        }
    }

    public class InputMiddleware<StateType> : MonoBehaviour
    {
        public InputProviderBase<StateType> inputProvider;

        public SerializedDictionary<string, bool> statekeys = new SerializedDictionary<string, bool>();
        public virtual StateType InputProcess(StateType state)
        {
            return state;
        }
    }
}
