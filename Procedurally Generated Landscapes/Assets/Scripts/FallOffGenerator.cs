using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class FallOffGenerator
{
    public static float[,] GenerateFallOffMap(int _size)
    {
        float[,] map = new float[_size, _size];
        for (int i = 0; i < _size; i++)
        {
            for (int j = 0; j < _size; j++)
            {
                float x = i / (float)_size * 2 - 1;
                float y = j / (float)_size * 2 - 1;

                // find which value is closer to the edge of the map
                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));

                map[i, j] = value;
            }
        }
        return map;
    }
}
