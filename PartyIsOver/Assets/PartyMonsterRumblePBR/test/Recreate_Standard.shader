Shader "Recreate/Standard" {
	Properties {
		[Toggle(_RENDERING_CUTOUT)] _RENDERING_CUTOUT ("RENDERING_CUTOUT", Float) = 0
		_Cutoff ("Cutoff", Range(0, 1)) = 0.01
		[Enum(Two Sided,0,Back,1,Front,2)] _RenderFaces ("Render Faces", Float) = 2
		[Toggle] _FlippedBackfaceNormals ("Flipped Backface Normals", Float) = 0
		_MainTex ("Albedo", 2D) = "white" {}
		_Color ("Color", Vector) = (1,1,1,1)
		_MetallicGlossMap ("Metallic", 2D) = "white" {}
		[Gamma] _Metallic ("Metallic", Range(0, 1)) = 0.5
		_Glossiness ("Smoothness", Range(0, 1)) = 0.5
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_BumpScale ("Normal Scale", Float) = 1
		_OcclusionMap ("Occlusion", 2D) = "white" {}
		_OcclusionStrength ("Occlusion Strength", Range(0, 1)) = 1
		[Toggle(_EMISSION)] _Emission ("Emission", Float) = 0
		_EmissionMap ("Emission", 2D) = "white" {}
		[HDR] [Gamma] _EmissionColor ("Emission Color", Vector) = (0,0,0,0)
		[Enum(UV0,0,UV1,1,UV2,2,UV3,3)] _UVSecondary ("UV Secondary", Float) = 0
		_DetailMask ("Detail Mask", 2D) = "white" {}
		_DetailAlbedoMap ("Detail Albedo", 2D) = "gray" {}
		_DetailNormalMap ("Detail Normal", 2D) = "bump" {}
		_DetailNormalMapScale ("Detail Normal Scale", Float) = 1
		[Toggle(_FRESNELFUR)] _FresnelFur ("Fresnel Fur", Float) = 0
		_FurMaskMap ("Fur Mask Map", 2D) = "white" {}
		[HDR] _FurColor ("Fur Color", Vector) = (1,1,1,0)
		_FurScale ("Fur Scale", Float) = 1
		_FurFalloff ("Fur Falloff", Float) = 5
		_FurAOIntensity ("Fur AO Intensity", Range(0, 1)) = 1
		[Toggle(_ANISOTROPIC)] _Anisotropic ("Anisotropic", Float) = 0
		_AnisotropicMask ("Anisotropic Mask", 2D) = "white" {}
		_AnisotropicDir ("Anisotropic Dir", Range(-1, 1)) = 1
		_AnisotropicSmoothnessMin ("Anisotropic Smoothness Min", Float) = 0
		_AnisotropicSmoothnessMax ("Anisotropic Smoothness Max", Float) = 1
		[HDR] [Gamma] _AnisotropicColor ("Anisotropic Color", Vector) = (1,1,1,0)
		[Toggle(_DISSOLVE)] _Dissolve ("Dissolve", Float) = 0
		[NoScaleOffset] _DissolveMap ("Dissolve Map", 2D) = "white" {}
		_DissolveMapScale ("Dissolve Map Scale", Float) = 0.1
		_DissolvePhase ("Dissolve Phase", Range(0, 1)) = 0.5
		_DissolveMin ("Dissolve Min", Range(0, 1)) = 0
		_DissolveMax ("Dissolve Max", Range(0, 1)) = 1
		_DissolveRandomness ("Dissolve Randomness", Range(0, 1)) = 0
		[HDR] [Gamma] _GlowColor ("Glow Color", Vector) = (11.31371,3.638134,0,0)
		_GlowWidth ("Glow Width", Range(0, 1)) = 0.2
		[Toggle] _DebugDissolve ("Debug Dissolve", Float) = 0
		[Toggle(_HIGHLIGHT)] _SurfaceHighlight ("Surface Highlight", Float) = 0
		_SurfaceHighlightTex ("Surface Highlight Tex", 2D) = "black" {}
		[HDR] [Gamma] _SurfaceHighlightColor ("Surface Highlight Color", Vector) = (1,1,1,0)
		_SurfaceHighlightIntensity ("Surface Highlight Intensity", Range(0, 1)) = 1
		_SurfaceHighlightTexTile ("Surface Highlight Tex Tile", Float) = 1
		_SurfaceHighlightAnimator ("Surface Highlight Animator", Float) = 0
		[Toggle(_FADE)] _Fade ("Fade", Float) = 0
		[Enum(Dither, 0, Central Dither, 1, Central Noise1, 2, Central Noise2, 3)] _FadeMethod ("Fade Method", Float) = 0
		[Toggle] _FadeAffectShadow ("Fade Affect Shadow", Float) = 0
		_CutoutOpacity ("Fadeout Opacity", Range(0, 1)) = 0
		_CentralFadeCameraDistance ("Central Fade Camera Distance", Float) = 13
		_CentralFadeTargetWorldPos ("Central Fade Target World Pos", Vector) = (0,0,0,1)
		[Toggle] _PowerUp ("PowerUp", Float) = 0
		_PowerUpIntensity ("PowerUp Intensity", Float) = 0
		_PowerUpScale ("PowerUp Scale", Float) = 1
		_PowerUpCurve ("PowerUp Curve", Range(0.1, 6)) = 2
		[HDR] [Gamma] _PowerUpColor ("PowerUp Color", Vector) = (2.828427,0.6322367,0,0)
		[Toggle(_SNOWCOVER)] _SnowCover ("Snow Cover", Float) = 0
		_SnowMask ("Snow Mask", 2D) = "black" {}
		_SnowCoverFalloff ("Snow Cover Falloff", Float) = 2.25
		_SnowAlbedo ("Snow Albedo", 2D) = "white" {}
		_SnowTint ("Snow Tint", Vector) = (1,1,1,0)
		_SnowTexTiling ("Snow Tex Tiling", Float) = 1
		_SnowSmoothness ("Snow Smoothness", Range(0, 1)) = 0
		[NoScaleOffset] _SparkleTex ("Sparkle Tex", 2D) = "black" {}
		_SparkleSize ("Sparkle Tiling", Float) = 5
		_SparkleColorization ("Sparkle Colorization", Range(0, 1)) = 0.5
		_SparkleSpeed ("Sparkle Speed", Float) = 0.0005
		_SparkleDensity ("Sparkle Density", Range(0, 1)) = 0.5
		_SparkleIntensity ("Sparkle Intensity", Float) = 10000
		[Toggle] _CustomNormalChannelInput ("CustomNormalChannelInput", Float) = 0
		_SnowNormal ("Snow Normal", 2D) = "bump" {}
		_SnowNormalScale ("Snow Normal Scale", Float) = 1
		[Gamma] _SnowMetallic ("Snow Metallic", Range(0, 1)) = 0
		[Toggle(_DITHERFUR)] _DitherFur ("Dither Fur", Float) = 0
		[Toggle(_USEDECAL)] _UseDecal ("Use Decal", Float) = 0
		[Toggle] _UseCapsuleSDF ("Use Capsule SDF", Float) = 1
		[HideInInspector] _texcoord ("", 2D) = "white" {}
		[HideInInspector] _texcoord2 ("", 2D) = "white" {}
		[HideInInspector] _texcoord3 ("", 2D) = "white" {}
		[HideInInspector] _texcoord4 ("", 2D) = "white" {}
		[HideInInspector] __dirty ("", Float) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	Fallback "Diffuse"
	//CustomEditor "RecreateShaderGUI.RecreateStandardShaderGUI"
}