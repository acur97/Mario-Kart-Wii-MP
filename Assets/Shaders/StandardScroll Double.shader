Shader "Standard Scroll Double"
{
    Properties
    {
        [HDR] _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo 1", 2D) = "white" {}
        _ScrollXSpeed("X scroll speed", Range(-100, 100)) = 0
        _ScrollYSpeed("Y scroll speed", Range(-100, 100)) = -10
        [HDR] _Color2 ("Color", Color) = (1,1,1,1)
        _SecondTex ("Albedo 2", 2D) = "white" {}
        _ScrollXSpeed2("X scroll speed 2", Range(-100, 100)) = 0
        _ScrollYSpeed2("Y scroll speed 2", Range(-100, 100)) = -40
        _Blend ("Texture Blend", Range(0,1)) = 1

        [Space(20)]
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Glossiness("Smoothness", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent"}
        LOD 200

        CGPROGRAM

        #pragma surface surf Standard fullforwardshadows alpha:fade

        #pragma target 3.0
        #include "UnityPBSLighting.cginc"
        
        sampler2D _MainTex;
        sampler2D _SecondTex;
        fixed _ScrollXSpeed;
        fixed _ScrollYSpeed;
        fixed _ScrollXSpeed2;
        fixed _ScrollYSpeed2;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_SecondTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _Color2;
        half _Blend;

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            fixed offsetX = _ScrollXSpeed * _Time;
            fixed offsetY = _ScrollYSpeed * _Time;
            fixed2 offsetUV = fixed2(offsetX, offsetY);
            fixed2 mainUV = IN.uv_MainTex + offsetUV;

            fixed offsetX2 = _ScrollXSpeed2 * _Time;
            fixed offsetY2 = _ScrollYSpeed2 * _Time;
            fixed2 offsetUV2 = fixed2(offsetX2, offsetY2);
            fixed2 mainUV2 = IN.uv_SecondTex + offsetUV2;

            fixed4 mainCol = tex2D(_MainTex, mainUV) * _Color;
            fixed4 texTwoCol = tex2D(_SecondTex, mainUV2) * _Color2;                           
       
            fixed4 mainOutput = mainCol.rgba * (1.0 - (texTwoCol.a * _Blend));
            fixed4 blendOutput = texTwoCol.rgba * texTwoCol.a * _Blend;         
       
            o.Albedo = mainOutput.rgb + blendOutput.rgb;
            o.Alpha = mainOutput.a + blendOutput.a;

            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
        }
        ENDCG
    }
    FallBack "Standard"
}