// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Paryi_camera"
{
	Properties
	{
		_Vector0("Vector 0", Vector) = (-2.76,3,0,0)
		_noiseScale("noiseScale", Float) = 1.73
		_fade("fade", Float) = -0.61
		_bokasi("bokasi", Float) = 1.81
		_noiseAngle("noiseAngle", Float) = 0
		[HDR]_MainColor("MainColor", Color) = (1,1,1,1)
		[HDR]_lineColor("lineColor", Color) = (1,0.4481132,0.4481132,1)
		_TextureMain("TextureMain", 2D) = "white" {}
		_line("line", Float) = 1.2
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha , SrcAlpha OneMinusSrcAlpha
		
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform float4 _MainColor;
		uniform sampler2D _TextureMain;
		uniform float4 _TextureMain_ST;
		uniform float _fade;
		uniform float _bokasi;
		uniform float _noiseScale;
		uniform float _noiseAngle;
		uniform float2 _Vector0;
		uniform float _line;
		uniform float4 _lineColor;


		float2 voronoihash10( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi10( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
		{
			float2 n = floor( v );
			float2 f = frac( v );
			float F1 = 8.0;
			float F2 = 8.0; float2 mg = 0;
			for ( int j = -1; j <= 1; j++ )
			{
				for ( int i = -1; i <= 1; i++ )
			 	{
			 		float2 g = float2( i, j );
			 		float2 o = voronoihash10( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
					float d = 0.5 * dot( r, r );
			 		if( d<F1 ) {
			 			F2 = F1;
			 			F1 = d; mg = g; mr = r; id = o;
			 		} else if( d<F2 ) {
			 			F2 = d;
			
			 		}
			 	}
			}
			return F1;
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_TextureMain = i.uv_texcoord * _TextureMain_ST.xy + _TextureMain_ST.zw;
			float4 tex2DNode20 = tex2D( _TextureMain, uv_TextureMain );
			float time10 = _noiseAngle;
			float2 voronoiSmoothId10 = 0;
			float2 coords10 = i.uv_texcoord * _noiseScale;
			float2 id10 = 0;
			float2 uv10 = 0;
			float fade10 = 0.5;
			float voroi10 = 0;
			float rest10 = 0;
			for( int it10 = 0; it10 <4; it10++ ){
			voroi10 += fade10 * voronoi10( coords10, time10, id10, uv10, 0,voronoiSmoothId10 );
			rest10 += fade10;
			coords10 *= 2;
			fade10 *= 0.5;
			}//Voronoi10
			voroi10 /= rest10;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float temp_output_13_0 = ( ( voroi10 * 1.0 ) + -(_Vector0.x + (ase_vertex3Pos.y - -1.0) * (_Vector0.y - _Vector0.x) / (1.0 - -1.0)) );
			float smoothstepResult15 = smoothstep( _fade , ( _fade + _bokasi ) , temp_output_13_0);
			float smoothstepResult26 = smoothstep( _fade , ( _bokasi + ( _fade * _line ) ) , temp_output_13_0);
			o.Emission = ( ( _MainColor * tex2DNode20 ) + ( ( smoothstepResult15 - smoothstepResult26 ) * _lineColor ) ).rgb;
			o.Alpha = ( _MainColor.a * tex2DNode20.a * smoothstepResult15 );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Unlit keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.Vector2Node;4;-1249.279,1098.46;Inherit;False;Property;_Vector0;Vector 0;0;0;Create;True;0;0;0;False;0;False;-2.76,3;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PosVertexDataNode;6;-1304.279,699.4597;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NegateNode;7;-759.1975,1045.769;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-1675.573,280.6906;Inherit;False;Property;_noiseAngle;noiseAngle;5;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;10;-1465.573,382.6905;Inherit;False;0;0;1;0;4;False;1;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1.26;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-1117.981,471.2291;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-1679.573,500.6906;Inherit;False;Property;_noiseScale;noiseScale;2;0;Create;True;0;0;0;False;0;False;1.73;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-264.9637,-196.9416;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;20;-710.9637,-116.9416;Inherit;True;Property;_TextureMain;TextureMain;8;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;188.1874,-103.7882;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Paryi_camera;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;True;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;2;5;False;;10;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.TransformPositionNode;1;-1298.521,879.8693;Inherit;False;Object;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TFHCRemapNode;3;-1042.279,936.4597;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-155.8204,111.1133;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-1049.033,374.5945;Inherit;False;Property;_bokasi;bokasi;4;0;Create;True;0;0;0;False;0;False;1.81;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-1078.877,23.75487;Inherit;False;Property;_fade;fade;3;0;Create;True;0;0;0;False;0;False;-0.61;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;13;-789.6042,732.6357;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;15;-468.4048,255.1884;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;18;-860.2524,212.7722;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;33;-14.56392,-120.8541;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;19;-625.9637,-295.9416;Inherit;False;Property;_MainColor;MainColor;6;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;35;-167.2109,680.2027;Inherit;False;Property;_lineColor;lineColor;7;1;[HDR];Create;True;0;0;0;False;0;False;1,0.4481132,0.4481132,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;77.11841,479.186;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;30;-151.8663,401.9729;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;26;-407.7948,501.4804;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;29;-654.7165,364.5318;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-792.3223,574.7087;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-983.2776,598.4811;Inherit;False;Property;_line;line;9;0;Create;True;0;0;0;False;0;False;1.2;0;0;0;0;1;FLOAT;0
WireConnection;7;0;3;0
WireConnection;10;1;12;0
WireConnection;10;2;11;0
WireConnection;14;0;10;0
WireConnection;21;0;19;0
WireConnection;21;1;20;0
WireConnection;0;2;33;0
WireConnection;0;9;22;0
WireConnection;3;0;6;2
WireConnection;3;3;4;1
WireConnection;3;4;4;2
WireConnection;22;0;19;4
WireConnection;22;1;20;4
WireConnection;22;2;15;0
WireConnection;13;0;14;0
WireConnection;13;1;7;0
WireConnection;15;0;13;0
WireConnection;15;1;16;0
WireConnection;15;2;18;0
WireConnection;18;0;16;0
WireConnection;18;1;17;0
WireConnection;33;0;21;0
WireConnection;33;1;34;0
WireConnection;34;0;30;0
WireConnection;34;1;35;0
WireConnection;30;0;15;0
WireConnection;30;1;26;0
WireConnection;26;0;13;0
WireConnection;26;1;16;0
WireConnection;26;2;29;0
WireConnection;29;0;17;0
WireConnection;29;1;28;0
WireConnection;28;0;16;0
WireConnection;28;1;31;0
ASEEND*/
//CHKSM=3FD2C0FA25A16971A6C6197475AB97F3A3C42C6B