Shader "Custom/Barrier"
{
    Properties
    {
		_MainTex("Texture", 2D) = "white" {}
		[HDR]_FillColor("FillColor", Color) = (1,1,1,1)
		[HDR]_SideColor("SideColor", Color) = (1,1,1,1)
		[HDR]_LineColor("LineColor", Color) = (1,1,1,1)
		_Divide("Divide", Int) = 1
		_LineRatio("LineRatio", Float) = 0.9
		_FillClear("FillClear", Float) = 0
		_ScrollSpeed("ScrollSpeed", Float) = 1
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
			fixed4 _FillColor;
			fixed4 _SideColor;
			fixed4 _LineColor;
			int _Divide;
			half _LineRatio;
			half _FillClear;
			fixed _ScrollSpeed;
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

            fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				static const float PI = 3.14159265f;

				fixed4 tex = tex2D(_MainTex, i.uv); // + _Time.y * _ScrollSpeed
				fixed4 c = _SideColor;

				// side
				float l = abs(0.5 - i.uv.x) * 2;
				c.a *= clamp((l - _FillClear) / (1 - _FillClear), 0, 1);

				// fill
				c += _FillColor;

				// line
				c += _LineColor * step(abs(sin(i.worldPos.y * _Divide + -1 * _Time.y * _ScrollSpeed)), _LineRatio);

				c.rgb = c.rgb + max(fixed3(0, 0, 0), -0.5);
				c.rgb *= c.a;
				return c;
            }
            ENDCG
        }
    }
}
