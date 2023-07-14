Shader "Tessellation/Bumped Specular (displacement)"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}

		[Space(20)]
		_SpecMap ("Base (RGB)(A)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.2

		[Space(20)]
		_BumpMap ("Normalmap", 2D) = "bump" {}
        _NormalPow ("Normal Power", Range(-4, 4)) = 1
		_BumpMap2 ("_DetailAlbedoMap", 2D) = "bump" {}
        _NormalPow2 ("Normal Power2", Range(-4, 4)) = 1
		
		[Space(20)]
		_ParallaxMap ("Heightmap (A)", 2D) = "black" {}
		_Parallax ("Height", Range (0.0, 1.0)) = 0.5
		_EdgeLength ("Edge length", Range(3,50)) = 10
		
		[Space(20)]
		_OccMap ("Occlusion Map (G)",2D) = "white"{}
        _OcclusionStrength ("Occlusion Strenght", Range(0, 1)) = 0
	}

	SubShader
	{ 
		Tags { "RenderType"="Opaque" }
		LOD 800
	
		CGPROGRAM
		#pragma surface surf StandardSpecular addshadow fullforwardshadows vertex:disp tessellate:tessEdge
		#pragma require tessellation tessHW
		#pragma target 4.6

		#include "UnityPBSLighting.cginc"
		#include "Tessellation.cginc"

		struct appdata
		{
			float4 vertex : POSITION;
			float4 tangent : TANGENT;
			float3 normal : NORMAL;
			float2 texcoord : TEXCOORD0;
			float2 texcoord1 : TEXCOORD1;
			float2 texcoord2 : TEXCOORD2;
		};

		float _EdgeLength;
		float _Parallax;
        half _Glossiness;
        float _OcclusionStrength;
        float _NormalPow;
        float _NormalPow2;

		float4 tessEdge (appdata v0, appdata v1, appdata v2)
		{
			return UnityEdgeLengthBasedTessCull (v0.vertex, v1.vertex, v2.vertex, _EdgeLength, _Parallax * 1.5f);
		}

		sampler2D _ParallaxMap;
		float4 _ParallaxMap_ST;

		void disp (inout appdata v)
		{
			float d = tex2Dlod(_ParallaxMap, float4(_ParallaxMap_ST.xy * v.texcoord.xy + _ParallaxMap_ST.zw,0,0)).a * _Parallax;
			v.vertex.xyz += v.normal * d;
		}
		
		inline fixed3 combineNormalMaps(fixed3 base, fixed3 detail)
		{
            base += fixed3(0, 0, 1);
            detail *= fixed3(-1, -1, 1);
            return base * dot(base, detail) / base.z - detail;
        }

		sampler2D _MainTex;
		sampler2D _SpecMap;
		sampler2D _BumpMap;
		sampler2D _BumpMap2;
        sampler2D _OccMap;
		fixed4 _Color;

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_SpecMap;
			float2 uv_BumpMap;
			float2 uv_BumpMap2;
		};

		void surf (Input IN, inout SurfaceOutputStandardSpecular o)
		{
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 specTex = tex2D(_SpecMap, IN.uv_SpecMap);
            half4 O = tex2D(_OccMap, IN.uv_MainTex);

			o.Albedo = tex.rgb * _Color.rgb;
			o.Specular = specTex.rgba;
            o.Smoothness = _Glossiness;

            o.Occlusion = LerpOneTo(O.g, _OcclusionStrength);

			fixed3 baseNormal = UnpackScaleNormal(tex2D(_BumpMap, IN.uv_BumpMap), _NormalPow);
            fixed3 detailNormal = UnpackScaleNormal(tex2D(_BumpMap2, IN.uv_BumpMap2), _NormalPow2);
			
			o.Normal = combineNormalMaps(baseNormal, detailNormal);
		}
		ENDCG
	}

	FallBack "Standard"
}