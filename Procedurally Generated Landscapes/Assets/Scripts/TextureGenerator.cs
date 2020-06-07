using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Creates a texture from a 1-D colour map
/// </summary>
public static class TextureGenerator
{
    public static Texture2D TextureFromColourMap(Color[] _colourMap, int _width, int _height)
    {
        Texture2D texture = new Texture2D(_width, _height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(_colourMap);
        texture.Apply();
        return texture;
    }

    public static Texture2D TextureFromHeightMap(float[,] _heightMap)
    {
        int width = _heightMap.GetLength(0);
        int height = _heightMap.GetLength(1);

        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colourMap[(y * width) + x] = Color.Lerp(Color.black, Color.white, _heightMap[x, y]);
            }
        }

        return TextureFromColourMap(colourMap, width, height);
    }
}
