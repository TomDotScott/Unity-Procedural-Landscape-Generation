Shader "Custom/TerrainShader"
{
	Properties
	{

	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		float minHeight;
	float maxHeight;

		struct Input {
			float3 worldPos;
		};

		float inverseLerp(float _a, float _b, float _value) {
			return saturate((_value - _a) / (_b - _a));
		}

		// blend between different textures depending on the height of the surface
		void surf(Input IN, inout SurfaceOutputStandard o) {
			float heightPercent = inverseLerp(minHeight, maxHeight, IN.worldPos.y);
			o.Albedo = heightPercent;
		}
		ENDCG
	}
		FallBack "Diffuse"
}
