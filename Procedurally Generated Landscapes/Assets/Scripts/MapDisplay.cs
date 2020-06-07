using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRenderer;

    public void DrawNoiseMap(float[,] _noiseMap)
    {
        int width = _noiseMap.GetLength(0);
        int height = _noiseMap.GetLength(1);

        Texture2D texture2D = new Texture2D(width, height);

        Color[] colourMap = new Color[width * height];
        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                colourMap[(y * width) + x] = Color.Lerp(Color.black, Color.white, _noiseMap[x, y]);
            }
        }
        texture2D.SetPixels(colourMap);
        texture2D.Apply();

        textureRenderer.sharedMaterial.mainTexture = texture2D;
        textureRenderer.transform.localScale = new Vector3(width, 1, height);
    }
}
