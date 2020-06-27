const static int maxLayerCount = 8;
const static float epsilon = 1E-4;

int layerCount;
float3 baseColours[maxLayerCount];
float baseStartHeights[maxLayerCount];
float baseBlends[maxLayerCount];
float baseColourStrength[maxLayerCount];
float baseTextureScales[maxLayerCount];

float invLerp(float a, float b, float t)
{
    return a + t * (b - a);
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

    albedo = 0;

    for (int i = 0; i < layerCount; i ++) {
        float drawStrength = invLerp(-baseBlends[i]/2 - epsilon, baseBlends[i]/2, heightPercent - baseStartHeights[i]);

        float3 baseColour = baseColours[i] * baseColourStrength[i];
        float3 textureColour = triplanar(worldPos, baseTextureScales[i], blendAxes, textures, ss, i) * (1-baseColourStrength[i]);

        albedo = albedo * (1-drawStrength) + textureColour * drawStrength;
    }
}