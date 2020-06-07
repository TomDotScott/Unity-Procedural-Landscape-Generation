using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int _width, int _height, float _scale, int _octaves, float _persistence, float _lacunarity)
    {
        float[,] noiseMap = new float[_width, _height];

        if (_scale <= 0)
        {
            _scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < _octaves; i++)
                {
                    // the higher the freuency, the further away the sample points,
                    // this means the height changes more rapidly
                    float sampleX = x / _scale * frequency;
                    float sampleY = y / _scale * frequency;

                    // generate values between -1 and 1 so that there can be dips in the 
                    // terrain when it is later generated
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= _persistence;

                    // frequency increases per octave since lacunarity is greater 
                    // than 1.
                    frequency *= _lacunarity;
                }

                // work out the range of the noisemap values
                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
            }
        }
        

        // normalise the noise map
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        } 
        return noiseMap;
    }
}
