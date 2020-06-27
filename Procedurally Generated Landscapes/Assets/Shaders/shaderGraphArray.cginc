#define SAMPLE_TEXTURE2D_ARRAY(textureName, samplerName, coord2, index)                  textureName.Sample(samplerName, float3(coord2, index))

const static int maxLayerCount = 8;
const static float epsilon = 1E-4;

int layerCount;
float3 baseColours[maxLayerCount];
float baseStartHeights[maxLayerCount];
float baseBlends[maxLayerCount];
float baseColourStrengths[maxLayerCount];
float baseTextureScales[maxLayerCount];

float invLerp(float xx, float yy, float value)
{
    return saturate((value - xx) / (yy - xx));
}


float3 triplanar(float3 worldPos, float scale, float3 blendAxes, Texture2DArray textures, SamplerState ss, int textureIndex) {
    float3 scaledWorldPos = worldPos / scale;
    float3 xProjection = SAMPLE_TEXTURE2D_ARRAY(textures, ss, float2(scaledWorldPos.y, scaledWorldPos.z), textureIndex) * blendAxes.x;
    float3 yProjection = SAMPLE_TEXTURE2D_ARRAY(textures, ss, float2(scaledWorldPos.x, scaledWorldPos.z), textureIndex) * blendAxes.y;
    float3 zProjection = SAMPLE_TEXTURE2D_ARRAY(textures, ss, float2(scaledWorldPos.x, scaledWorldPos.y), textureIndex) * blendAxes.z;
    return xProjection + yProjection + zProjection;
}

void layer_terrain_float(float3 worldPos, float heightPercent, float3 worldNormal, Texture2DArray textures, SamplerState ss, int layerCount, out float3 albedo) {
    float3 blendAxes = abs(worldNormal);
    blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;
    float3 debugResult;
    albedo = 0;

    for (int i = 0; i < layerCount; i ++) {
        float drawStrength = invLerp(-baseBlends[i] / 2 - epsilon, baseBlends[i] / 2, heightPercent - baseStartHeights[i]);
        float3 baseColour = baseColours[i] * baseColourStrengths[i];
        float3 textureColour = triplanar(worldPos, baseTextureScales[i], blendAxes, textures, ss, i) * (1-baseColourStrengths[i]);
        // drawStrength = 0.5;
        albedo = albedo * (1 - drawStrength) + (baseColour + textureColour) * drawStrength;
    }
    //albedo = debugResult;
}