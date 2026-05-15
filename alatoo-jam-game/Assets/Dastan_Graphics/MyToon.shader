Shader "Custom/PBR_Toon_Advanced"
{
    Properties
    {
        [Header(Base)]
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
        
        [Header(Normals)]
        [Normal] _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalStrength ("Normal Strength", Range(0, 5)) = 1
        
        [Header(PBR Settings)]
        _MetallicMap ("Metallic Map (R)", 2D) = "white" {}
        _Metallic ("Metallic Intensity", Range(0, 1)) = 0
        _SpecMap ("Roughness Map (R)", 2D) = "white" {}
        _Roughness ("Roughness Intensity", Range(0, 2)) = 1
        _Glossiness ("Specular Sharpness", Range(2, 256)) = 32
        
        [Header(Toon Shadow)]
        _ShadowColor ("Shadow Color", Color) = (0.2, 0.2, 0.2, 1)
        _Step ("Shadow Threshold", Range(0, 1)) = 0.5
        _Smoothness ("Shadow Smoothing", Range(0.01, 0.5)) = 0.02
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }
        
        // ОСНОВНОЙ ПРОХОД
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; float3 normalOS : NORMAL; float4 tangentOS : TANGENT; };
            struct Varyings { float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; float3 normalWS : TEXCOORD3; float3 tangentWS : TEXCOORD4; float3 bitangentWS : TEXCOORD5; float3 viewDirWS : TEXCOORD6; };

            sampler2D _MainTex, _NormalMap, _SpecMap, _MetallicMap;
            float4 _Color, _ShadowColor;
            float _Step, _Smoothness, _NormalStrength, _Glossiness, _Roughness, _Metallic;

            Varyings vert (Attributes IN) {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                VertexNormalInputs normalInputs = GetVertexNormalInputs(IN.normalOS, IN.tangentOS);
                OUT.normalWS = normalInputs.normalWS;
                OUT.tangentWS = normalInputs.tangentWS;
                OUT.bitangentWS = normalInputs.bitangentWS;
                OUT.viewDirWS = GetWorldSpaceViewDir(TransformObjectToWorld(IN.positionOS.xyz));
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target {
                half3 normalTS = UnpackNormalScale(tex2D(_NormalMap, IN.uv), _NormalStrength);
                float3x3 TBN = float3x3(IN.tangentWS, IN.bitangentWS, IN.normalWS);
                float3 worldNormal = normalize(mul(normalTS, TBN));
                
                Light light = GetMainLight();
                float NdotL = dot(worldNormal, light.direction);
                float toon = smoothstep(_Step - _Smoothness, _Step + _Smoothness, NdotL);
                
                float3 viewDir = normalize(IN.viewDirWS);
                float3 halfVec = normalize(light.direction + viewDir);
                float NdotH = saturate(dot(worldNormal, halfVec));
                
                float roughnessMap = tex2D(_SpecMap, IN.uv).r * _Roughness;
                float metallicMap = tex2D(_MetallicMap, IN.uv).r * _Metallic;
                
                float specPower = _Glossiness * (1.1 - saturate(roughnessMap));
                float spec = pow(NdotH, specPower);
                
                half4 albedo = tex2D(_MainTex, IN.uv) * _Color;
                float3 specColor = lerp(float3(1,1,1), albedo.rgb, metallicMap);
                float3 finalSpec = spec * specColor * toon;

                half3 diffuse = lerp(_ShadowColor.rgb * albedo.rgb, albedo.rgb, toon);
                diffuse *= (1.0 - metallicMap);
                
                return half4(diffuse + finalSpec, albedo.a);
            }
            ENDHLSL
        }

        // ДОБАВЬ ЭТОТ PASS: Он нужен Unity 6 для записи глубины (важно для декалей)
        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0

            HLSLPROGRAM
            #pragma vertex DepthVert
            #pragma fragment DepthFrag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings { float4 positionCS : SV_POSITION; };

            Varyings DepthVert(Attributes IN) {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }
            half4 DepthFrag(Varyings IN) : SV_TARGET { return 0; }
            ENDHLSL
        }
    }
}
