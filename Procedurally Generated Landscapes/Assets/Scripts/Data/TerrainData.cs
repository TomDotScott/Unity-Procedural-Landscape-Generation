using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu()]
public class TerrainData : UpdatableData
{
    public float uniformScale = 5f;

    public bool useFlatShading;
    public bool applyFallOffMap;

    public float heightMultiplier;
    public AnimationCurve meshHeightCurve;

    public float minHeight
    {
        get
        {
            return uniformScale * heightMultiplier * meshHeightCurve.Evaluate(0);
        }
    }

    public float maxHeight
    {
        get
        {
            return uniformScale * heightMultiplier * meshHeightCurve.Evaluate(1);
        }
    }

}
