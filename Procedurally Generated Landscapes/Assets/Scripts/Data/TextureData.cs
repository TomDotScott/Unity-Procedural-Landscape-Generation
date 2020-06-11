using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TextureData : UpdatableData
{
    public void ApplyToMaterial(Material _mat)
    {
        // Do some Stuff
    }

    public void UpdateMeshHeights(Material _mat, float _min, float _max)
    {
        _mat.SetFloat("minHeight", _min);
        _mat.SetFloat("maxHeight", _max);
    }

}
