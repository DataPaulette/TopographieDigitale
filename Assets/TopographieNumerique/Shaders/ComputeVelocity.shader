﻿Shader "Leon/ComputeVelocity" {
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

            sampler2D _MainTex, _Position, _HeightMap;
            float _NoiseScale, _TimeElapsed, _Curl, _Twirl, _Attract, _Gravity, _Expand, _Grain, _TimeDelta, _Friction, _Dimension;

            fixed4 frag (v2f_img i) : SV_Target {
                float3 velocity = tex2D(_MainTex, i.uv).xyz;
                float3 position = tex2D(_Position, i.uv).xyz;

                float3 seed = position * _NoiseScale;
                float3 curl = float3(noise(seed), noise(seed+float3(98.64,76.24,43.692)), noise(seed+float3(145.54,945.57,654.7485)))* 2.0 - 1.0;
                float3 grain = float3(hash(seed.xz),hash(seed.yz),hash(seed.xy)) * 2.0 - 1.0;
                float angle = atan2(position.z, position.x);
                float3 twirl = float3(-sin(angle),0.0,cos(angle));
                float3 gravity = float3(0,1,0);
                float3 expand = normalize(position);
                float index = i.uv.x * _Dimension * _Dimension;
                float2 uv = clamp(float2(fmod(index, _Dimension)/_Dimension,floor(index/_Dimension)/_Dimension),0.,1.)*2.0-1.0;
                float3 p = float3(uv.x,tex2D(_HeightMap, uv*0.5+0.5).r,uv.y)*float3(4,2,4);

                float3 attract = normalize(p-position) * smoothstep(0.1, 0.5, length(p-position));
                float should = smoothstep(0.28,0.4,fmod(noise(seed)+_TimeElapsed*.1, 1.0));
                float a = _TimeElapsed * .3;
                float2 pp = float2(cos(a),sin(a))*.7;
                should = smoothstep(0.4,0.1,length(uv-pp));
                // should = pow(should, 5.);

                velocity += (curl * _Curl + twirl * _Twirl + gravity * _Gravity + expand * _Expand + grain * _Grain) * should;
                velocity += attract * _Attract * (1.-should);

                velocity *= clamp(1.0 - _TimeDelta * _Friction, 0.0, 1.0);
                // velocity += hash(uv)*0.001;

                return float4(velocity,0);
            }
            ENDCG
        }
    }
}
