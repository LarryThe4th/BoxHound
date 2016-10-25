Shader "CustomShader/FullScreenBlur"
{
	Properties
	{
		// This is a image effect shader so the main texture is come form the 
		// currently rendering camera.
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling
		Cull Off
		// No writen in Z buffer
		ZWrite Off
		// Igore depth test
		ZTest Off

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
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			// The source texture.
			sampler2D _MainTex;
			// A magic spell that present by the darkness of Unity's source code.
			// No, seriously, it returns the size of a texel of the texture, if the texture
			// size is 1024 x 768, than the x value in result will be 1.0/1014.0 and
			// the Y value will be 1.0/768.0.
			// The return value of float4 will be like: (1 / width, 1 / height, width, height).
			float4 _MainTex_TexelSize;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}

			// A way to blur the source image.
			// It basicly just add all the pixel's color around one pixel 
			// and divide by the number of sample we used,
			// the result will be the average value of all surrounding pixels's color.
			// See more about it under the link below:
			// https://en.wikipedia.org/wiki/Box_blur
			float4 BoxBlur(sampler2D sourceTex, float2 uv, float4 position)
			{
				float4 c = 
					tex2D(sourceTex, uv + float2(-position.x, position.y)) +	// 1. The pixel on the upper left
					tex2D(sourceTex, uv + float2(0, position.y)) +				// 2. The pixel on the top
					tex2D(sourceTex, uv + float2(position.x, position.y)) +		// 3. The pixel on the upper right
					tex2D(sourceTex, uv + float2(-position.x, 0)) +				// 4. The left hand side pixel
					tex2D(sourceTex, uv + float2(0, 0)) +						// 5. The origin pixel
					tex2D(sourceTex, uv + float2(position.x, 0)) +				// 6. The right hand side pixel
					tex2D(sourceTex, uv + float2(-position.x, -position.y)) +	// 7. The pixel on the lower left
					tex2D(sourceTex, uv + float2(0, -position.y)) +				// 8. The pixel underneath
					tex2D(sourceTex, uv + float2(position.x, -position.y));		// 9. The pixel on the lower right

				// Divide by the number of sample we used.
				return c / 9;
			}

			
			float4 frag (v2f i) : SV_Target
			{
				// Pass in the source texture, the target uv coodinate and the texel size of the texture (the position of the pixel)
				float4 color = BoxBlur(_MainTex, i.uv, _MainTex_TexelSize);
				return color;
			}
			ENDCG
		}
	}
}
