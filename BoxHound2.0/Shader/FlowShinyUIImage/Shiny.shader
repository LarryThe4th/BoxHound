// This shader is based on the Unity3D's build-in default UI shader.
Shader "CustomShader/FlowingShiny"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		// Add a shiny reflection texture.
		_ShinyTex("Shiny Texture", 2D) = "Black" {}

		_Color("Tint", Color) = (1,1,1,1)
		_ScrollXSpeed("HorizontalSpeed", Range(0, 10)) = 2
		_ScrollYSpeed("VerticalSpeed", Range(0, 10)) = 0
		_ScrollDirection("Driection", Range(-1, 1)) = -1
		_FlowColor("FlowColor",Color) = (1,1,1,1)


		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
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

		Stencil
	{
		Ref[_Stencil]
		Comp[_StencilComp]
		Pass[_StencilOp]
		ReadMask[_StencilReadMask]
		WriteMask[_StencilWriteMask]
	}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest[unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask[_ColorMask]

		Pass
	{
		Name "Default"
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 2.0

#include "UnityCG.cginc"
#include "UnityUI.cginc"

#pragma multi_compile __ UNITY_UI_ALPHACLIP

		struct appdata_t
		{
			float4 vertex   : POSITION;
			float4 color    : COLOR;

			float2 texcoord : TEXCOORD0;
			float2 texcoord2 : TEXCOORD0;
		};

		struct v2f
		{
			float4 vertex   : SV_POSITION;
			fixed4 color : COLOR;
			half2 texcoord  : TEXCOORD0;
			half2 texcoord2  : TEXCOORD1;
			float4 worldPosition : TEXCOORD2;
		};

		fixed4 _Color;
		fixed4 _TextureSampleAdd;
		float4 _ClipRect;

		fixed _ScrollXSpeed;
		fixed _ScrollYSpeed;
		fixed _ScrollDirection;
		float4 _FlowColor;

		v2f vert(appdata_t IN)
		{
			v2f OUT;
			OUT.worldPosition = IN.vertex;
			OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

			OUT.texcoord = IN.texcoord;
			OUT.texcoord2 = IN.texcoord;

	#ifdef UNITY_HALF_TEXEL_OFFSET
			OUT.vertex.xy += (_ScreenParams.zw - 1.0) * float2(-1,1) * OUT.vertex.w;
	#endif

			OUT.color = IN.color * _Color;

			return OUT;
		}

		sampler2D _MainTex;
		sampler2D _ShinyTex;

		fixed4 frag(v2f IN) : SV_Target
		{
			// half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd)* IN.color;

			fixed2 scrolledUV = IN.texcoord2;
			fixed xScrollValue = _ScrollXSpeed * _Time.y / 2;
			fixed yScrollValue = _ScrollYSpeed * _Time.y / 2;
			scrolledUV += fixed2(xScrollValue, yScrollValue) * _ScrollDirection;

			// half4 color = ((tex2D(_MainTex, IN.texcoord)) * IN.color) * (tex2D(_ShinyTex, scrolledUV));

			// _flowShinyColor in here use as a height map.
			half4 _flowShinyColor = tex2D(_ShinyTex, scrolledUV);
			half4 _originalColor = tex2D(_MainTex, IN.texcoord);

			half4 color = (_originalColor * IN.color) + (_flowShinyColor * _FlowColor);

			color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

	#ifdef UNITY_UI_ALPHACLIP
			clip(color.a - 0.001);
	#endif

			return color;
		}
		ENDCG
	}
	}
}
