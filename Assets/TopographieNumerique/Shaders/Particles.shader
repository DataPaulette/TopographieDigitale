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

		float _Radius, _Emissive, _TrailSegment, _TimeElapsed, _Metallic, _Glossiness;
		float4x4 _MatrixWorld;

		#ifdef SHADER_API_D3D11
		struct PointData { float3 position, velocity, info; };
		struct TrailData { float3 position, info; };
		StructuredBuffer<PointData> _Particles;
		StructuredBuffer<TrailData> _Trails;
		#endif

		void vert (inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			float3 p = v.vertex.xyz;
			o.color = float4(1,1,1,1);

			#ifdef SHADER_API_D3D11
			float index = v.texcoord1.y*_TrailSegment;
			index += floor(clamp(v.texcoord.y*0.5+0.5,0,1) * (_TrailSegment-1));
			PointData particle = _Particles[v.texcoord1.y];
			p = _Trails[index].position;
			// o.color = float4(particle.info, 1);
			o.color = float4(1,1,1,1);
			float moving = length(_Particles[v.texcoord1.y].velocity);
			float yes = smoothstep(0.5,0.1,moving);
			v.normal = normalize(cross(float3(0,1,0),_Trails[index+1].position-_Trails[index].position));
			// v.normal = lerp(v.normal, normalize(cross(float3(0,1,0),_Particles[v.texcoord1.y].velocity)), 1.-yes);
			float height = 0.4 + 1.4 * noise(p*20.);
			float y = v.texcoord.y * 0.5 + 0.5;
			p += float3(0,1,0) * yes * y * height;
			float a = noise(p*10.) + _TimeElapsed;
			p.xz += float2(cos(a),sin(a)) * sin(v.texcoord.y) * .1 * yes * y;
			p += float3(1,0,0) * yes * v.texcoord.x * 0.1*(1.-y);
			o.color *= lerp(1,y,yes);
			p -= float3(0,1,0) * _Radius * v.texcoord.x * (1.-yes);
			#endif

			// billboard
			// float3 z = normalize(_WorldSpaceCameraPos - p);
			// float3 x = normalize(cross(z, float3(0,1,0)));
			// float3 y = normalize(cross(x, z));
			// float3 normal = x * v.texcoord.x - y * v.texcoord.y;
			v.vertex.xyz = p;//mul(unity_WorldToObject, float4(p,1)).xyz;
			o.texcoord = v.texcoord;
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
