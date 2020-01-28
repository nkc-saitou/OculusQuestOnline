Shader "Custom/Dissolve"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Value("Value", Range(0, 1)) = 0.0
		_ColorInside("Color inside", Color) = (1, 1, 1, 1)
		[HDR]_EmissionColorInside("Color Emission inside", Color) = (0, 0, 0, 1)
		_ColorOutside("Color outside", Color) = (1, 1, 1, 1)
		[HDR]_EmissionColorOutSide("Color Emission outside", Color) = (0, 0, 0, 1)
		_DivideX("DivideX", Int) = 128
		_DivideY("DivideY", Int) = 128
		_UpdatePer("UpdatePer", Float) = 0
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
		CGINCLUDE
		#include "UnityCG.cginc"
		uniform half _Value;
		uniform half4 _ColorOutside;
		uniform half4 _ColorInside;
		uniform	half4 _EmissionColorOutSide;
		uniform	half4 _EmissionColorInside;
		uniform sampler2D _MainTex;
		uniform int _DivideX;
		uniform int _DivideY;
		uniform half _UpdatePer;
		uniform half _Glossiness;
		uniform half _Metallic;

		#pragma target 3.0

		ENDCG

		SubShader
		{
				Tags { "RenderType" = "Opaque" }

				CGPROGRAM
				#pragma surface surf Standard fullforwardshadows

				struct Input {
					float2 uv_MainTex;
				};



				float random(fixed2 p) {

					fixed t = 0;
					if (_UpdatePer <= 0) {
						t = _Time.y;
					}
					else {
						t = floor(_Time.y / _UpdatePer) * _UpdatePer;
					}

					return frac(sin(dot(p + t, fixed2(12.9898, 78.233))) * 43758.5453);
				}

				float noise(fixed2 st) {
					fixed2 p = floor(st);
					return random(p);
				}

				void surf(Input IN, inout SurfaceOutputStandard o) {
					fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

					if (noise(IN.uv_MainTex * fixed2(_DivideX, _DivideY)) >= _Value) {
						discard;
					}

					o.Albedo = c * _ColorOutside;
					o.Emission = _EmissionColorOutSide;
					o.Metallic = _Metallic;
					o.Smoothness = _Glossiness;
					o.Alpha = c.a;
				}

				ENDCG

				Cull Front

				CGPROGRAM
				#pragma surface surf Standard fullforwardshadows

				struct Input {
					float2 uv_MainTex;
				};

				float random(fixed2 p) {

					fixed t = 0;
					if (_UpdatePer <= 0) {
						t = _Time.y;
					}
					else {
						t = floor(_Time.y / _UpdatePer) * _UpdatePer;
					}

					return frac(sin(dot(p + t, fixed2(12.9898, 78.233))) * 43758.5453);
				}

				float noise(fixed2 st) {
					fixed2 p = floor(st);
					return random(p);
				}

				void surf(Input IN, inout SurfaceOutputStandard o) {
					fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

					if (noise(IN.uv_MainTex * fixed2(_DivideX, _DivideY)) >= _Value) {
						discard;
					}

					o.Albedo = _ColorInside;
					o.Emission = _EmissionColorInside;
					o.Metallic = _Metallic;
					o.Smoothness = _Glossiness;
					o.Alpha = c.a;
				}

				ENDCG
		}
}