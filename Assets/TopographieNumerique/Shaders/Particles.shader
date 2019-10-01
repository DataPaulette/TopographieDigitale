Shader "Leon/Particles"
{
	Properties
	{
		_Emissive ("Emissive", Float) = 0
		_Metallic ("Metallic", Range(0,1)) = 0
		_Glossiness ("Glossiness", Range(0,1)) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		Cull Off

		CGPROGRAM
		#pragma surface surf Standard vertex:vert addshadow
		#pragma target 3.0
		#include "Common.cginc"

		struct Input
		{
			float2 texcoord : TEXCOORD0;
			float2 texcoord1 : TEXCOORD1;
			float4 color : TEXCOORD2;
		};

		sampler2D _Velocity, _Position, _Trail, _HeightMap;
		float _Radius, _Emissive, _TrailSegment, _TimeElapsed, _Metallic, _Glossiness, _Dimension, _Height, _HeightVariation, _WindNoiseScale, _WindStrength, _WindSpeed;
		float4x4 _MatrixWorld;

		void vert (inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			float3 p = v.vertex.xyz;
			float3 velocity = tex2Dlod(_Velocity, float4(v.texcoord1.x,0,0,0)).xyz;
			float moving = smoothstep(0.0, 0.1, length(velocity));
			float notMoving = 1.-moving;

			float yy = v.texcoord.y * 0.5 + 0.5;
			p = tex2Dlod(_Trail, float4(v.texcoord1.x,yy,0,0)).xyz;
			float e = 0.01;
			float index = v.texcoord1.y;
      float4 uv = clamp(float4(fmod(index, _Dimension)/_Dimension,floor(index/_Dimension)/_Dimension,0,0),0.,1.);
			float3 north = float3(uv.x,tex2Dlod(_HeightMap, uv).r,uv.y+e);
			float3 south = float3(uv.x,tex2Dlod(_HeightMap, uv).r,uv.y-e);
			float3 east = float3(uv.x+e,tex2Dlod(_HeightMap, uv).r,uv.y);
			float3 west = float3(uv.x-e,tex2Dlod(_HeightMap, uv).r,uv.y);
			v.normal = cross(normalize(north-south), normalize(east-west));

			p.y += yy * (_Height + _HeightVariation * hash(uv.xy)) * notMoving;
			float a = noise(p * _WindNoiseScale) * PI * 2 +  _WindSpeed * _TimeElapsed;
			p.xz += float2(cos(a),sin(a))*yy*_WindStrength * notMoving;

			// billboard
			float3 z = -normalize(_WorldSpaceCameraPos - p);
			float3 x = normalize(cross(z, float3(0,1,0)));
			float3 y = normalize(cross(x, z));
			p += (x * v.texcoord.x * (1.-yy) + y * v.texcoord.y) * (_Radius + moving*.01);
			v.vertex.xyz = p;
			o.texcoord = v.texcoord;
			o.color = float4(1,1,1,1)*yy;
		}

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = IN.color.rgb;
			o.Emission = o.Albedo * _Emissive;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = 1.0;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
