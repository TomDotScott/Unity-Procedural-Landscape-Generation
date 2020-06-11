using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu()]
public class TerrainData : ScriptableObject
{
    public float uniformScale = 5f;

    public bool useFlatShading;
    public bool applyFallOffMap;

    public float heightMultiplier;
    public AnimationCurve meshHeightCurve;

}
