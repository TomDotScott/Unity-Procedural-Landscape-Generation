using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int _width, int _height, float _scale)
    {
        float[,] noiseMap = new float[_width, _height];

        if(_scale <= 0)
        {
            _scale = 0.0001f;
        }

        for(int y = 0; y < _height; y++)
        {
            for(int x = 0; x < _width; x++)
            {
                float sampleX = x / _scale;
                float sampleY = y / _scale;

                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                noiseMap[x, y] = perlinValue;
            }
        }

        return noiseMap;
    }
}
