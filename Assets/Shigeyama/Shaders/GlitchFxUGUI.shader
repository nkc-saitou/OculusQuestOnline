Shader "Custom/GlitchFxUGUI"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		_DivideX("DivideX", Int) = 128
		_DivideY("DivideY", Int) = 128
		_UpdatePer("UpdatePer", Float) = 0
		_CutOff("CutOff", Float) = 0.0
		_RadiusOffset("RadiusOffset", Float) = 0.0
		_GradationPower("GradationPower", Float) = 1
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Cull Off
			Lighting Off
			ZWrite Off
			Blend One OneMinusSrcAlpha

			Pass {
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile _ PIXELSNAP_ON
				#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
				#include "UnityCG.cginc"

				struct appdata_t {
					float4 vertex   : POSITION;
					float4 color    : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f {
					float4 vertex   : SV_POSITION;
					fixed4 color : COLOR;
					float2 texcoord  : TEXCOORD0;
				};

				fixed4 _Color;

				v2f vert(appdata_t IN) {
					v2f OUT;
					OUT.vertex = UnityObjectToClipPos(IN.vertex);
					OUT.texcoord = IN.texcoord;
					OUT.color = IN.color * _Color;
					#ifdef PIXELSNAP_ON
					OUT.vertex = UnityPixelSnap(OUT.vertex);
					#endif

					return OUT;
				}

				sampler2D _MainTex;
				sampler2D _AlphaTex;
				int _DivideX;
				int _DivideY;
				fixed _UpdatePer;
				float _CutOff;
				float _RadiusOffset;
				float _GradationPower;

				fixed4 SampleSpriteTexture(float2 uv) {
					fixed4 color = tex2D(_MainTex, uv);

	#if ETC1_EXTERNAL_ALPHA
					// get the color from an external texture (usecase: Alpha support for ETC1 on android)
					color.a = tex2D(_AlphaTex, uv).r;
	#endif ETC1_EXTERNAL_ALPHA

					return color;
				}

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

				fixed4 frag(v2f IN) : SV_Target {

					fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
					fixed2 texcoord = IN.texcoord;
					c *= noise(IN.texcoord * fixed2(_DivideX, _DivideY));
					fixed2 ld = (texcoord - 0.5) * (texcoord - 0.5);
					c.a *= clamp(sqrt(ld.x + ld.y) * _GradationPower + _RadiusOffset, 0, 1);

					clip(c.a - _CutOff);
					
					c.rgb = c.rgb + max(fixed3(0,0,0),IN.color.rgb - 0.5);
					c.rgb *= c.a;
					return c;

				}
			ENDCG
			}
		}
}