Shader "Leon/ComputeInit" {
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
            #include "Common.cginc"

            sampler2D _MainTex;

            float4 frag (v2f_img i) : SV_Target {
                return float4(hash(i.uv),hash(i.uv+float2(0.123,5.102)),hash(i.uv-float2(5.876,1.57)),0);
            }
            ENDCG
        }
    }
}
