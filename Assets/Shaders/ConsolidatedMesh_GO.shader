Shader "ConsolidatedMesh_GO"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_MainTexAlpha("MainTexAlpha", 2D) = "white" {}
		_LightmapTex("LightmapTex", 2D) = "white" {}
		_TransDataTex("TransData", 2D) = "white" {}
		_AtlasDataTex("AtlasData", 2D) = "white" {}
		[Enum(UnityEngine.Rendering.BlendMode)] _BlendSrc("Blend mode Source", Int) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _BlendDst("Blend mode Destination", Int) = 0
		_AlphaCutoff("Alpha Cutoff", Float) = 0.0
		_FinalAlpha("Final Alpha", Float) = 1.0
		_ShadowDepthOffset("Shadow Depth Offset", Float) = 0.0
		_ContentDepthOffset("Content Depth Offset", Float) = 0.0
	}

	SubShader
	{
		Blend[_BlendSrc][_BlendDst]

		//Shadow Pass
		Pass
		{
			Stencil
			{
				Ref 0
				Comp Equal
				Pass IncrSat
			}

			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing

			#include "UnityCG.cginc"
			#include "ShaderGlobals.cginc"

			float _AlphaCutoff;
			float _FinalAlpha;
			float _ShadowDepthOffset;

			OutVertexInPixel vert(InVertex i)
			{
				OutVertexInPixel o;

				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_INITIALIZE_OUTPUT(OutVertexInPixel, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				ProcessShadowVertex(i, o);

				return o;
			}

			struct FragOut
			{
				float4 Color : SV_Target;
				float  Depth : SV_Depth;
			};

			FragOut frag(OutVertexInPixel i)
			{
				FragOut output;
				output.Color = i.color;
				output.Color.a *= _FinalAlpha;
				output.Color.rgb = _ShadowColor.rgb;
				output.Color.a *= _ShadowColor.a;

				output.Depth = i.vertex.z;
				// modify depth here
				output.Depth += _ShadowDepthOffset;
				return output;

			}

			ENDCG
		}

		Pass
		{
			Stencil
			{
				Ref 0
				Comp Always
				Pass IncrSat
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing

			#include "UnityCG.cginc"
			#include "ShaderGlobals.cginc"

			float _AlphaCutoff;
			float _FinalAlpha;
			float _ContentDepthOffset;

			OutVertexInPixel vert(InVertex i)
			{
				OutVertexInPixel o;

				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_INITIALIZE_OUTPUT(OutVertexInPixel, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				ProcessVertex(i, o);

				return o;
			}

			struct FragOut
			{
				float4 Color : SV_Target;
				float  Depth : SV_Depth;
			};

			FragOut frag(OutVertexInPixel i) : SV_Target
			{

				FragOut output;
				output.Color = i.color;


				ApplyAtlasTextureAndAlphaCutoff(output.Color, i.atlas, i.uv.xy, _AlphaCutoff);

				CalculateReflections(output.Color, i.worldNorm, i.params);

				output.Color.a *= _FinalAlpha;

				CalculateLighting(output.Color, i.worldNorm);

				output.Depth = i.vertex.z;
				// modify depth here
				output.Depth += _ContentDepthOffset;

				return output;
			}
			ENDCG
		}
	}
}
