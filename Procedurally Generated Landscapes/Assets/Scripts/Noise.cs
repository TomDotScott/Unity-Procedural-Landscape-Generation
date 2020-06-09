using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public enum NormaliseMode
    {
        Local, Global
    }

    public static float[,] GenerateNoiseMap(int _width, int _height, int _seed, float _scale, int _octaves, float _persistence, float _lacunarity, Vector2 _offset, NormaliseMode _normaliseMode)
    {
        float[,] noiseMap = new float[_width, _height];

        System.Random prng = new System.Random(_seed);
        Vector2[] octaveOffsets = new Vector2[_octaves];

        float maxGlobalNoiseHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < _octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + _offset.x;
            float offsetY = prng.Next(-100000, 100000) - _offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxGlobalNoiseHeight += amplitude;
            amplitude *= _persistence;
        }

        if (_scale <= 0)
        {
            _scale = 0.0001f;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = _width / 2;
        float halfHeight = _height / 2;


        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                frequency = 1;
                amplitude = 1;
                float noiseHeight = 0;

                for (int i = 0; i < _octaves; i++)
                {
                    // the higher the freuency, the further away the sample points,
                    // this means the height changes more rapidly
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / _scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / _scale * frequency;

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
                if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
            }
        }
        

        // normalise the noise map
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                if (_normaliseMode == NormaliseMode.Local)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                }
                else
                {
                    // make the height values consistent across the entire map
                    float normalizedHeight = (noiseMap[x, y] + 1) / (maxGlobalNoiseHeight / 1.75f);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        } 
        return noiseMap;
    }
}
