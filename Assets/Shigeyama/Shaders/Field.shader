Shader "Custom/Field"
{
    Properties
    {
		_MainTex("Texture", 2D) = "white" {}
		_FadeTex("FadeTexture", 2D) = "white" {}
		[HDR]_FillColor("FillColor", Color) = (1,1,1,1)
		[HDR]_LineColor("LineColor", Color) = (1,1,1,1)
		_Period("Period", Float) = 1
		_LineWidth("LineWidth", Float) = 0.1
		_FadeTexScale("FadeTexScale", Float) = 1
		_ScrollSpeedX("ScrollSpeedX", Float) = 1
		_ScrollSpeedY("ScrollSpeedY", Float) = 1
		[Enum(UnityEngine.Rendering.CullMode)]_Cull("Cull", Float) = 0
    }
    SubShader
    {
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
			}
        LOD 100

			Lighting Off
			ZWrite Off
			Blend One OneMinusSrcAlpha

        Pass
        {
			Cull [_Cull]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				half3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;

				// v2fにワールド座標伝達用のfloat3を追加
				// セマンティクスは適当なTEXCOORDを使えばいいでしょう
				// (「TEXCOORD」という名前ですが、テクスチャ座標に限らず自由に使って構わないかと思います)
				// すでにTEXCOORD0はテクスチャUV用、TEXCOORD1はフォグ効果用に使われてしまっているので
				// TEXCOORD2を使うことにしました
				// もしテクスチャ座標やフォグ効果が不要なら削除してしまい、空いたTEXCOORDを利用してもOKです
				float3 worldPos : TEXCOORD1;
            };
			
			sampler2D _MainTex;
			sampler2D _FadeTex;
			fixed4 _FillColor;
			fixed4 _LineColor;
			fixed _Period; 
			fixed _LineWidth;
			fixed _FadeTexScale;
			fixed _ScrollSpeedX;
			fixed _ScrollSpeedY;
			float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				// モデルの頂点座標をモデル変換行列(unity_ObjectToWorld)で変換し
				// ワールド座標にして、それのxyzをworldPosに渡す
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                return o;
            }

			fixed rule(v2f IN, float period, float width) {
				float modX = abs(fmod(IN.worldPos.x, period));
				float modY = abs(fmod(IN.worldPos.y, period));
				float modZ = abs(fmod(IN.worldPos.z, period));

				float minBorder = width * .5f;
				float maxBorder = period - width * .5f;

				fixed x = max(-1 * sign(minBorder - modX) * sign(modX - maxBorder), 0);
				fixed y = max(-1 * sign(minBorder - modY) * sign(modY - maxBorder), 0);
				fixed z = max(-1 * sign(minBorder - modZ) * sign(modZ - maxBorder), 0);
				return saturate(x + y + z);
			}

            fixed4 frag (v2f i) : SV_Target
			{
				// fill
				fixed4 c = _FillColor;

				// line
				fixed v = rule(i, _Period, _LineWidth);
				fixed4 l = _LineColor * fixed4(v, v, v, v);

				// scroll
				fixed2 uv = frac(float2(i.worldPos.x, i.worldPos.y) / _FadeTexScale + float2(_Time.y * _ScrollSpeedX, _Time.y * _ScrollSpeedY));
				fixed4 tex = tex2D(_FadeTex, uv);

				c += l * tex.r;

				c.rgb = c.rgb + max(fixed3(0, 0, 0), -0.5);
				c.rgb *= c.a;
				return c;
            }
            ENDCG
        }
    }
}
