using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class RandomUtility
{
    public static bool OneInChance(int chance)
    {
        int rand = Random.Range(0, chance);
        return rand == 0;
    }

    public static bool PercentageChance(float percentage)
    {
        float rand = Random.value;
        return rand <= percentage / 100;
    }

    public static int GetRandomWeightedIndex(List<float> weights)
    {
        int res = weights.Count - 1;

        float randomWeight = Random.Range(0, weights.Sum());
        for (int i = 0; i < weights.Count; ++i)
        {
            randomWeight -= weights[i];
            if (randomWeight <= 0)
            {
                res = i;
                break;
            }
        }

        return res;
    }
}
