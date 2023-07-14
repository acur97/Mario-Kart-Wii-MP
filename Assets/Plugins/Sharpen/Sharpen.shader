Shader "Shaders/Sharpen"
{
	Properties
	{
		_MainTex("Base", 2D) = "" {}
	}

	CGINCLUDE

#include "UnityCG.cginc"

	UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
	half _CentralFactor;
	half _SideFactor;
	half4 _MainTex_TexelSize;

	struct appdata
	{
		half4 pos : POSITION;
		half2 uv  : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f
	{
		half4 pos : SV_POSITION;
		half2 uv  : TEXCOORD0;
		half4 uv1  : TEXCOORD1;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	v2f vert(appdata v)
	{
		v2f o;
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_OUTPUT(v2f, o);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		o.pos = UnityObjectToClipPos(v.pos);
		o.uv = UnityStereoTransformScreenSpaceTex(v.uv);
		o.uv1 = half4(o.uv - _MainTex_TexelSize.xy, o.uv + _MainTex_TexelSize.xy);
		return o;
	}

	half4 frag(v2f i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
		half4 c = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv) * _CentralFactor;
		c -= UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv1.xy) * _SideFactor;
		c -= UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv1.xw) * _SideFactor;
		c -= UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv1.zy) * _SideFactor;
		c -= UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv1.zw) * _SideFactor;
		return c;
	}

	ENDCG

	Subshader
	{
		Pass
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			ENDCG
		}
	}

	Fallback off
}