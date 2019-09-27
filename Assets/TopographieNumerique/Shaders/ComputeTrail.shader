Shader "Leon/ComputeTrail" {
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

            sampler2D _MainTex, _Position;
            float _TrailSegment, _TrailDamping;

            fixed4 frag (v2f_img i) : SV_Target {
                float3 trail = tex2D(_MainTex, i.uv).xyz;
                float3 previous = tex2D(_MainTex, i.uv-float2(0,1./_TrailSegment)).xyz;
                float3 position = tex2D(_Position, i.uv).xyz;
                trail = position;
                trail = lerp(trail, previous, _TrailDamping * step(0.1,i.uv.y));
                return float4(trail,0);
            }
            ENDCG
        }
    }
}
