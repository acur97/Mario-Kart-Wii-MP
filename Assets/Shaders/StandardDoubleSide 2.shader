Shader "Standard DoubleSide 2"
{
    Properties
    {
        //Texture properties
        _MainTex("Base (RGB)", 2D) = "white" {}
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        [HDR] _Color("Color", color) = (1,1,1,1)
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Glossiness("Smoothness", Range(0,1)) = 0.5

        [Space(20)]
        [Normal]_NormalMap("Normalmap", 2D) = "bump" {}
        _NormalPow("Normal Power", Range(-4, 4)) = 1

        [Space(20)]
        [Normal]_NormalMap2("Normalmap2", 2D) = "bump" {}
        _NormalPow2("Normal2 Power", Range(-4, 4)) = 1

        [Space(20)]
        _OccMap("Occlusion Map",2D) = "white"{}
        _OcclusionStrength("Occlusion Strenght", Range(0, 1)) = 0

        [Space(20)]
        _EmissionMap("Emission Map",2D) = "black"{}
        [HDR] _EmissionColor("EmissionColor", color) = (0,0,0)
    }

    SubShader
    {
        //Tags { "RenderType" = "Opaque" "Queue" = "Geometry+1" "ForceNoShadowCasting" = "True" }
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry+1" }
        LOD 300

        Cull Back
        CGPROGRAM
        #pragma surface surf Standard addshadow fullforwardshadows
        #pragma target 4.6

        #include "UnityPBSLighting.cginc"

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalMap;
            float2 uv_NormalMap2;
        };

        half _Glossiness;
        half _Metallic;
        half _Cutoff;

        sampler2D _MainTex;
        sampler2D _NormalMap;
        sampler2D _NormalMap2;
        sampler2D _OccMap;
        sampler2D _EmissionMap;

        fixed4 _Color;
        fixed4 _EmissionColor;

        float _NormalPow;
        float _NormalPow2;
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
            clip(c.a - _Cutoff);
        }
        ENDCG

        Cull Front
        CGPROGRAM
        #pragma surface surf Standard addshadow fullforwardshadows
        #pragma target 4.6

        #include "UnityPBSLighting.cginc"

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalMap;
            float2 uv_NormalMap2;
        };

        half _Glossiness;
        half _Metallic;
        half _Cutoff;

        sampler2D _MainTex;
        sampler2D _NormalMap;
        sampler2D _NormalMap2;
        sampler2D _OccMap;
        sampler2D _EmissionMap;

        fixed4 _Color;
        fixed4 _EmissionColor;

        float _NormalPow;
        float _NormalPow2;
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
            
            o.Normal = -combinedNormal;
            clip(c.a - _Cutoff);
        }
        ENDCG
    }
    FallBack "Standard"
}