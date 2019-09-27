Shader "Leon/ComputePosition" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex, _Velocity;
            float _Speed, _TimeDelta;

            float4 frag (v2f_img i) : SV_Target {
                float3 position = tex2D(_MainTex, i.uv).xyz;
                float3 velocity = tex2D(_Velocity, i.uv).xyz;
                position += velocity * _Speed * _TimeDelta;
                return float4(position,1);
            }
            ENDCG
        }
    }
}
