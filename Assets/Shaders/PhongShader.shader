Shader "Standard Tessellate Phong"
{
    Properties
    {
        //Tessellation
        _EdgeLength("Edge length", Range(2,50)) = 12.5
        _Phong("Phong Strengh", Range(-1,1)) = 0.5
        [MaterialToggle] _MaxTess("No Cull", float) = 0

        [Space(20)]
        //Texture properties
        _MainTex("Base (RGB)", 2D) = "white" {}
        _Color("Color", color) = (1,1,1,1)
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Glossiness("Smoothness", Range(0,1)) = 0.5

        [Space(20)]
        [Normal]_NormalMap("Normalmap", 2D) = "bump" {}
        _NormalPow("Normal Power", Range(-4, 4)) = 1

        [Space(20)]
        [Normal]_NormalMap2("Normalmap2", 2D) = "bump" {}
        _NormalPow2("Normal2 Power", Range(-4, 4)) = 1

        [Space(20)]
        _ParallaxMap("HeightMap", 2D) = "white" {}
        _Parallax("Height", Range(0, 1)) = 0

        [Space(20)]
        _OccMap("Occlusion Map",2D) = "white"{}
        _OcclusionStrength("Occlusion Strenght", Range(0, 1)) = 0

        [Space(20)]
        _EmissionMap("Emission Map",2D) = "black"{}
        [HDR] _EmissionColor("EmissionColor", color) = (0,0,0)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 300

        CGPROGRAM
        #pragma surface surf Standard addshadow fullforwardshadows vertex:disp tessellate:tessEdge tessphong:_Phong
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

        float _Phong;
        float _EdgeLength;
        float _MaxTess;

        float4 tessEdge(appdata v0, appdata v1, appdata v2)
        {
            return UnityEdgeLengthBasedTessCull(v0.vertex, v1.vertex, v2.vertex, _EdgeLength, _MaxTess);
        }

        void disp(inout appdata v)
        {
            //Do nothing
        }

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalMap;
            float2 uv_NormalMap2;
            float3 viewDir;
        };

        half _Glossiness;
        half _Metallic;

        sampler2D _MainTex;
        sampler2D _NormalMap;
        sampler2D _NormalMap2;
        sampler2D _OccMap;
        sampler2D _EmissionMap;
        sampler2D _ParallaxMap;

        fixed4 _Color;
        fixed4 _EmissionColor;

        float _NormalPow;
        float _NormalPow2;
        float _Parallax;
        float _OcclusionStrength;

        inline fixed3 combineNormalMaps(fixed3 base, fixed3 detail)
        {
            base += fixed3(0, 0, 1);
            detail *= fixed3(-1, -1, 1);
            return base * dot(base, detail) / base.z - detail;
        }

        fixed3 baseNormal;
        fixed3 detailNormal;
        fixed3 combinedNormal;

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            half h = tex2D(_ParallaxMap, IN.uv_MainTex).g;
            float2 offset = ParallaxOffset1Step(h, _Parallax, IN.viewDir);
            IN.uv_MainTex += offset;
            IN.uv_NormalMap += offset;

            half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            half4 O = tex2D(_OccMap, IN.uv_MainTex);

            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Occlusion = LerpOneTo(O.g, _OcclusionStrength);
            o.Emission = tex2D(_EmissionMap, IN.uv_MainTex).rgb * _EmissionColor.rgb;

            baseNormal = UnpackScaleNormal(tex2D(_NormalMap, IN.uv_NormalMap), _NormalPow);
            detailNormal = UnpackScaleNormal(tex2D(_NormalMap2, IN.uv_NormalMap2), _NormalPow2);
            combinedNormal = combineNormalMaps(baseNormal, detailNormal);

            o.Normal = combinedNormal;
        }
        ENDCG
    }
    FallBack "Standard"
}