using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

[CreateAssetMenu()]
public class TextureData : UpdatableData
{
	private const int textureSize = 512;

	// Create a 16-bit colour texture format
	private const TextureFormat textureFormat = TextureFormat.RGB565;

	public Layer[] layers;

	float savedMinHeight;
	float savedMaxHeight;

	public void ApplyToMaterial(Material material)
	{
		UpdateMeshHeights(material, savedMinHeight, savedMaxHeight);
	}

	public void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
	{
		material.SetInt("_layerCount", layers.Length);
        material.SetColorArray("baseColours", layers.Select(x => x.baseColour).ToArray());
        material.SetFloatArray("baseStartHeights", layers.Select(x => x.startHeight).ToArray());
        material.SetFloatArray("baseBlends", layers.Select(x => x.blendStrength).ToArray());
        material.SetFloatArray("baseColourStrengths", layers.Select(x => x.colourStrength).ToArray());
        material.SetFloatArray("baseTextureScales", layers.Select(x => x.textureScale).ToArray());
        Texture2DArray texturesArray = GenerateTextureArray(layers.Select(x => x.texture).ToArray());
		material.SetTexture("_textures", texturesArray);

		if(material.GetTexture("_textures") != null)
        {
			Debug.Log("TEXTURE ARRAY SET");
			Debug.Log(texturesArray.depth);
        }


		savedMinHeight = minHeight;
		savedMaxHeight = maxHeight;

		material.SetFloat("_minHeight", minHeight);
		material.SetFloat("_maxHeight", maxHeight);
	}

	private Texture2DArray GenerateTextureArray(Texture2D[] textures)
    {
		Texture2DArray textureArray = new Texture2DArray(textureSize, textureSize, textures.Length, textureFormat, true);
		for(int i = 0; i < textures.Length; i++)
        {
			Debug.Log(textures[i].name);
			textureArray.SetPixels(textures[i].GetPixels(), i);
        }
		textureArray.Apply();
		return textureArray;
    }


	/// <summary>
	/// Allows for multiple textures to be used
	/// </summary>
	[System.Serializable]
	public class Layer
    {
		public Texture2D texture;
		public Color baseColour;
		[Range(0, 1)]
		public float colourStrength;
		[Range(0, 1)]
		public float startHeight;
		[Range(0, 1)]
		public float blendStrength;
		public float textureScale;
    }
}
