Shader "Custom/Ripple"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _RippleStrength ("Ripple Strength", Range(0, 1)) = 0.5
        _RippleSpeed ("Ripple Speed", Range(0, 10)) = 5
        _RippleSize ("Ripple Size", Range(0, 10)) = 10
        _DarknessAmount ("Darkness Amount", Range(0, 1)) = 0
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        
        Blend SrcAlpha OneMinusSrcAlpha
        
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
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 distortedUV : TEXCOORD1;
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD2;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _RippleStrength;
            float _RippleSpeed;
            float _RippleSize;
            float _DarknessAmount;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                o.screenPos = ComputeScreenPos(o.vertex);
                
                float2 center = float2(0.5, 0.5);
                float dist = distance(v.uv, center);
                float ripple = sin(dist * 10 + _Time.y * 5) * _RippleStrength * 0.01;
                o.distortedUV = o.uv + float2(ripple, ripple);
                
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // 如果强度为0，直接返回原始纹理
                if (_RippleStrength <= 0.001)
                {
                    fixed4 col = tex2D(_MainTex, i.uv);
                    //应用黑色混合
                    col.rgb = lerp(col.rgb, float3(0, 0, 0), _DarknessAmount);
                    return col;
                }
                
                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                
                float2 center = float2(0.5, 0.5);
                float dist = distance(screenUV, center);
                
                float timeEffect = _Time.y * _RippleSpeed;
                float ripple = sin(dist * _RippleSize - timeEffect) * 0.5 + 0.5;
                ripple *= _RippleStrength;
                
                float2 offset = normalize(screenUV - center) * ripple * 0.02;
                
                float2 uv = i.uv + offset;
                
                #if UNITY_EDITOR
                if (unity_Batching < 0)
                {
                    fixed4 col = tex2D(_MainTex, i.distortedUV);
                    col.rgb = lerp(col.rgb, float3(0, 0, 0), _DarknessAmount);
                    return col;
                }
                #endif
                
                fixed4 col = tex2D(_MainTex, uv);
                
                col.rgb += ripple * 0.05;

                col.rgb = lerp(col.rgb, float3(0, 0, 0), _DarknessAmount);
                
                return col;
            }
            ENDCG
        }
    }
}