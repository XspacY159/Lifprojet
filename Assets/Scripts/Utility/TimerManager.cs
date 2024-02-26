using System;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    public float counter;
    public Action action;
    public bool unscaledTimed;
}

public class TimerManager : MonoBehaviour
{
    private static Dictionary<string, Timer> timers = new Dictionary<string, Timer>();
    private List<string> timersToRemove = new List<string>();

    public static bool StartTimer(float time, string key, Action _action = null, bool _unscaleTime = false)
    {
        if (time == 0) return false;

        if(timers.ContainsKey(key))
            return true;

        timers.Add(key, new Timer { 
            action = _action, counter = time, unscaledTimed = _unscaleTime
        });
        return false;
    }

    private void Update()
    {
        foreach(string key in timers.Keys)
        {
            Timer timer = timers[key];
            if(timer.counter > 0)
            {
                timer.counter -= timer.unscaledTimed ? Time.unscaledDeltaTime : Time.deltaTime;
                continue;
            }

            timersToRemove.Add(key);
        }
        while(timersToRemove.Count > 0)
        {
            string key = timersToRemove[0];
            if(timers.ContainsKey(key))
            {
                timers[key].action?.Invoke();
                timers.Remove(key);
            }
            timersToRemove.Remove(key);
        }
    }

    public static void AddToTimer(string key, float value)
    {
        timers[key].counter += value;
    }

    public static Timer GetTimer(string key)
    { 
        return timers[key]; 
    }

    public static Dictionary<string, Timer> GetTimers()
    {
        Dictionary<string, Timer> res = new Dictionary<string, Timer>();

        foreach (string key in timers.Keys)
        {
            res.Add(key, timers[key]);
        }

        return res;
    }

    public static bool IsCounting(string key)
    {
        return timers.ContainsKey(key);
    }

    public static void Cancel(string key)
    {
        if( timers.ContainsKey(key))
        {
            timers.Remove(key);
        }
    }
}
