Shader "Standard Scroll Transparent"
{
    Properties
    {
        [HDR]_Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo", 2D) = "white" {}
        [Enum(Cull Back,1, Front,2, Off,0)] _Cull("Cull", Int) = 2
        _ScrollXSpeed("X scroll speed", Range(-100, 100)) = 0
        _ScrollYSpeed("Y scroll speed", Range(-100, 100)) = 0
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        [Space(20)]

        [Normal]_NormalMap("Normalmap", 2D) = "bump" {}
        _NormalPow("Normal Power", Range(-4, 4)) = 1
        _ScrollXSpeed2("X scroll speed 2", Range(-100, 100)) = 0
        _ScrollYSpeed2("Y scroll speed 2", Range(-100, 100)) = 0
        [Space(20)]

        [Normal]_NormalMap2("Normalmap2", 2D) = "bump" {}
        _NormalPow2("Normal2 Power", Range(-4, 4)) = 1
        _ScrollXSpeed3("X scroll speed 3", Range(-100, 100)) = 0
        _ScrollYSpeed3("Y scroll speed 3", Range(-100, 100)) = 0

    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent"}

        Cull[_Cull]
        LOD 200
        //Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM

        #pragma surface surf Standard fullforwardshadows alpha:fade

        #pragma target 3.0
        #include "UnityPBSLighting.cginc"

        sampler2D _MainTex;
        sampler2D _NormalMap;
        sampler2D _NormalMap2;
        fixed _ScrollXSpeed;
        fixed _ScrollYSpeed;
        fixed _ScrollXSpeed2;
        fixed _ScrollYSpeed2;
        fixed _ScrollXSpeed3;
        fixed _ScrollYSpeed3;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalMap;
            float2 uv_NormalMap2;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _NormalPow;
        float _NormalPow2;

        inline fixed3 combineNormalMaps(fixed3 base, fixed3 detail) {
            base += fixed3(0, 0, 1);
            detail *= fixed3(-1, -1, 1);
            return base * dot(base, detail) / base.z - detail;
        }

        fixed3 baseNormal;
        fixed3 detailNormal;
        fixed3 combinedNormal;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)

        UNITY_INSTANCING_BUFFER_END(Props)

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            fixed offsetX = _ScrollXSpeed * _Time;
            fixed offsetY = _ScrollYSpeed * _Time;
            fixed2 offsetUV = fixed2(offsetX, offsetY);

            fixed offsetX2 = _ScrollXSpeed2 * _Time;
            fixed offsetY2 = _ScrollYSpeed2 * _Time;
            fixed2 offsetUV2 = fixed2(offsetX2, offsetY2);

            fixed offsetX3 = _ScrollXSpeed3 * _Time;
            fixed offsetY3 = _ScrollYSpeed3 * _Time;
            fixed2 offsetUV3 = fixed2(offsetX3, offsetY3);

            fixed2 mainUV = IN.uv_MainTex + offsetUV;
            fixed2 normalUV = IN.uv_NormalMap + offsetUV2;
            fixed2 normalUV2 = IN.uv_NormalMap2 + offsetUV3;

            fixed4 c = tex2D (_MainTex, mainUV) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;

            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;

            baseNormal = UnpackScaleNormal(tex2D(_NormalMap, normalUV), _NormalPow);
            detailNormal = UnpackScaleNormal(tex2D(_NormalMap2, normalUV2), _NormalPow2);
            combinedNormal = combineNormalMaps(baseNormal, detailNormal);
            
            o.Normal = combinedNormal;

        }
        ENDCG
    }
    FallBack "Standard"
}
