Shader "Standard DoubleSide 2Vertex"
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

        [Space(40)]
        _wind_dir ("Wind Direction", Vector) = (1,0.25,0,0)
        _wind_size ("Wind Wave Size", range(-50,50)) = 5
        _tree_sway_stutter_influence("Tree Sway Stutter Influence", range(0,1)) = 0.2
        _tree_sway_stutter ("Tree Sway Stutter", range(0,10)) = 1.5
        _tree_sway_speed ("Tree Sway Speed", range(0,10)) = 1
        _tree_sway_disp ("Tree Sway Displacement", range(0,1)) = 0.3
        //_branches_disp ("Branches Displacement", range(0,0.5)) = 0
        _leaves_wiggle_disp ("Leaves Wiggle Displacement", float) = 0
        _leaves_wiggle_speed ("Leaves Wiggle Speed", float) = 0
        //_r_influence ("Red Vertex Influence", range(0,1)) = 0
        _b_influence ("Blue Vertex Influence", range(0,1)) = 0
    }

    SubShader
    {
        //Tags { "RenderType" = "Opaque" "Queue" = "Geometry+1" "ForceNoShadowCasting" = "True" }
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry+1" }
        LOD 300

        Cull Back
        CGPROGRAM
        #pragma surface surf Standard addshadow fullforwardshadows vertex:vert
        #pragma target 4.6

        #include "UnityPBSLighting.cginc"

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalMap;
            float2 uv_NormalMap2;
        };

        //Declared Variables
        float4 _wind_dir;
        float _wind_size;
        float _tree_sway_speed;
        float _tree_sway_disp;
        float _leaves_wiggle_disp;
        float _leaves_wiggle_speed;
        //float _branches_disp;
        float _tree_sway_stutter;
        float _tree_sway_stutter_influence;
        //float _r_influence;
        float _b_influence;

        // Vertex Manipulation Function
        void vert (inout appdata_full i)
        {

            //Gets the vertex's World Position 
            float3 worldPos = mul (unity_ObjectToWorld, i.vertex).xyz;

            //Tree Movement and Wiggle
            i.vertex.x += (cos(_Time.z * _tree_sway_speed + (worldPos.x/_wind_size) + (sin(_Time.z * _tree_sway_stutter * _tree_sway_speed + (worldPos.x/_wind_size)) * _tree_sway_stutter_influence) ) + 1)/2 * _tree_sway_disp * _wind_dir.x * (i.vertex.y / 10) + 
            cos(_Time.w * i.vertex.x * _leaves_wiggle_speed + (worldPos.x/_wind_size)) * _leaves_wiggle_disp * _wind_dir.x * i.color.b * _b_influence;

            i.vertex.z += (cos(_Time.z * _tree_sway_speed + (worldPos.z/_wind_size) + (sin(_Time.z * _tree_sway_stutter * _tree_sway_speed + (worldPos.z/_wind_size)) * _tree_sway_stutter_influence) ) + 1)/2 * _tree_sway_disp * _wind_dir.z * (i.vertex.y / 10) + 
            cos(_Time.w * i.vertex.z * _leaves_wiggle_speed + (worldPos.x/_wind_size)) * _leaves_wiggle_disp * _wind_dir.z * i.color.b * _b_influence;

            i.vertex.y += cos(_Time.z * _tree_sway_speed + (worldPos.z/_wind_size)) * _tree_sway_disp * _wind_dir.y * (i.vertex.y / 10);

            //Branches Movement
            //i.vertex.y += sin(_Time.w * _tree_sway_speed + _wind_dir.x + (worldPos.z/_wind_size)) * _branches_disp  * i.color.r * _r_influence;
        }

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
        #pragma surface surf Standard addshadow fullforwardshadows vertex:vert
        #pragma target 4.6

        #include "UnityPBSLighting.cginc"

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalMap;
            float2 uv_NormalMap2;
        };

        //Declared Variables
        float4 _wind_dir;
        float _wind_size;
        float _tree_sway_speed;
        float _tree_sway_disp;
        float _leaves_wiggle_disp;
        float _leaves_wiggle_speed;
        //float _branches_disp;
        float _tree_sway_stutter;
        float _tree_sway_stutter_influence;
        //float _r_influence;
        float _b_influence;

        // Vertex Manipulation Function
        void vert (inout appdata_full i)
        {

            //Gets the vertex's World Position 
            float3 worldPos = mul (unity_ObjectToWorld, i.vertex).xyz;

            //Tree Movement and Wiggle
            i.vertex.x += (cos(_Time.z * _tree_sway_speed + (worldPos.x/_wind_size) + (sin(_Time.z * _tree_sway_stutter * _tree_sway_speed + (worldPos.x/_wind_size)) * _tree_sway_stutter_influence) ) + 1)/2 * _tree_sway_disp * _wind_dir.x * (i.vertex.y / 10) + 
            cos(_Time.w * i.vertex.x * _leaves_wiggle_speed + (worldPos.x/_wind_size)) * _leaves_wiggle_disp * _wind_dir.x * i.color.b * _b_influence;

            i.vertex.z += (cos(_Time.z * _tree_sway_speed + (worldPos.z/_wind_size) + (sin(_Time.z * _tree_sway_stutter * _tree_sway_speed + (worldPos.z/_wind_size)) * _tree_sway_stutter_influence) ) + 1)/2 * _tree_sway_disp * _wind_dir.z * (i.vertex.y / 10) + 
            cos(_Time.w * i.vertex.z * _leaves_wiggle_speed + (worldPos.x/_wind_size)) * _leaves_wiggle_disp * _wind_dir.z * i.color.b * _b_influence;

            i.vertex.y += cos(_Time.z * _tree_sway_speed + (worldPos.z/_wind_size)) * _tree_sway_disp * _wind_dir.y * (i.vertex.y / 10);

            //Branches Movement
            //i.vertex.y += sin(_Time.w * _tree_sway_speed + _wind_dir.x + (worldPos.z/_wind_size)) * _branches_disp  * i.color.r * _r_influence;
        }

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