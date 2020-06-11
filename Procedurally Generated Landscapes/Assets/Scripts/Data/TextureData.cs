using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TextureData : UpdatableData
{
    float savedMinHeight;
    float savedMaxHeight;

    public void ApplyToMaterial(Material _mat)
    {
        UpdateMeshHeights(_mat, savedMinHeight, savedMaxHeight);
    }

    public void UpdateMeshHeights(Material _mat, float _min, float _max)
    {
        savedMaxHeight = _max;
        savedMinHeight = _min;

        _mat.SetFloat("minHeight", _min);
        _mat.SetFloat("maxHeight", _max);
    }

}
