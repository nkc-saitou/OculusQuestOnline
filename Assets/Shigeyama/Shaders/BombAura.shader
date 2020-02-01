Shader "Custom/BombAura"
{
    Properties
    {
		_MainTex("Texture", 2D) = "white" {}
		[HDR]_Color("Color", Color) = (1,1,1,1)
		_Clear("Clear", Float) = 0.0
		_Amplified("Amplified", Float) = 1
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
				half vdotn : TEXCOORD1;
            };

            sampler2D _MainTex;
			fixed4 _Color;
			fixed _Amplified;
			fixed _Clear;
			fixed _ScrollSpeed;
			float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				half3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
				o.vdotn = dot(viewDir, v.normal.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture

				fixed4 tex = tex2D(_MainTex, i.uv + _Time.y * _ScrollSpeed);
                fixed4 t = clamp(tex.r * _Amplified - (_Amplified / 2), 0, 1);
				fixed4 c = _Color;
				clip(t - i.vdotn * _Clear);
				c.a *= t;

				c.rgb = c.rgb + max(fixed3(0, 0, 0), -0.5);
				c.rgb *= c.a;
				return c;
            }
            ENDCG
        }
    }
}
