using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRenderer;

    public void DrawTexture(Texture2D _texture)
    {
        textureRenderer.sharedMaterial.mainTexture = _texture;
        textureRenderer.transform.localScale = new Vector3(_texture.width, 1, _texture.height);
    }
}
