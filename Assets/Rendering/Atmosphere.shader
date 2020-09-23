// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Hidden/Atmosphere"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                half3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                half3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = mul(unity_ObjectToWorld, UnityObjectToClipPos(v.vertex));
                o.uv = v.uv;
                o.worldNormal = UnityObjectToViewPos(v.vertex);  
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = v.vertex - _WorldSpaceCameraPos; //- ComputeScreenPos(o.vertex); // v.viewDir; //normalize(_WorldSpaceCameraPos - v.viewDir);
                return o;
            }

            sampler2D _MainTex;
            UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

            float2 raySphere (float3 planetCenter, float atmosphereRadius, float3 rayOrigin, float3 rayDir) 
            {
                float3 offset = rayOrigin - planetCenter;
                float a = 1;
                float b = 2 * dot(offset, rayDir);
                float c = dot(offset, offset) - atmosphereRadius * atmosphereRadius;
                float d = b*b - 4*a*c;

                // Number of intersections: 0 when d<0; 1 when d=0, 2 when d>0
                if (d > 0) {
                    float s = sqrt(d);
                    float dstToAtmosphereNear = max(0, (-b-s) / (2*a));
                    float dstToAtmosphereFar = (-b+s) / (2*a);

                    // ignore intersections that occur behind the ray
                    if (dstToAtmosphereFar >=0) {
                        return float2(dstToAtmosphereNear, dstToAtmosphereFar-dstToAtmosphereNear);
                    }
                }

                // If ray did not intersect sphere 
                return float2(10000, 0);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float sceneDepthNonlinear = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                float sceneDepth = LinearEyeDepth(sceneDepthNonlinear) * length(i.viewDir);

                float3 rayOrigin = _WorldSpaceCameraPos;
                float3 rayDir = normalize(i.viewDir);
                
                float atmosphereRadius = 150;
                float2 hitInfo = raySphere((0,0,0), atmosphereRadius, rayOrigin, rayDir);
                float dstToAtmosphere = hitInfo.x;
                float dstThroughAtmosphere = hitInfo.y;

                // just invert the colors
                col.rgb = i.worldNormal; ///(col.rgb) * (1 - sceneDepth) + (1, 1, 1) * sceneDepth - i.viewDir/100;
                return col; //dstToAtmosphere / (atmosphereRadius * 2);
            }
            ENDCG
        }
    }
}
