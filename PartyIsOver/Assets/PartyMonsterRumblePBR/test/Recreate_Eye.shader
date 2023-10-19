Shader "Recreate/Eye" {
	Properties {
		_Tint ("Tint", Vector) = (1,1,1,0)
		[HideInInspector] _EyeCenterPos ("_EyeCenterPos", Vector) = (0,0,0,0)
		[HideInInspector] _EyeOri ("_EyeOri", Vector) = (0,0,0,0)
		_Albedo ("Albedo", 2D) = "white" {}
		_AlbedoBoost ("Albedo Boost", Float) = 1
		_EmissionIntensity ("Emission Intensity", Float) = 0
		_Metallic ("Metallic", Range(0, 1)) = 0
		_Smoothness ("Smoothness", Range(0, 1)) = 0
		_OuterSmoothness ("Outer Smoothness", Range(0, 1)) = 0
		_WidthScale ("宽度尺寸 - Width Scale", Float) = 0
		_HeightScale ("高度尺寸 - Height Scale", Float) = 0
		_OverallHorizontalOffset ("整体水平位置 - Overall Horizontal Offset", Float) = 0.08
		_OverallVerticalOffset ("整体垂直位置 - Overall Vertical Offset", Float) = 0
		_RightEyeInitialHorizontalOffset ("右眼初始水平位置 - Right Eye Initial Horizontal Offset", Float) = -0.2
		_LeftEyeInitialHorizontalOffset ("左眼初始水平位置 - Left Eye Initial Horizontal Offset", Float) = -0.2
		_InitialDepthOffset ("初始深度 - Initial Depth Offset", Range(0, 5)) = 0
		_DepthBiasNear ("近端眼睛深度偏移量 Depth Bias Near", Float) = 0
		_FixedEyeVerticalAngle ("垂直角度修正 Fixed Eye Vertical Angle", Float) = 0
		_VerticalMoveRange ("垂直偏移范围 - Vertical Move Range", Float) = 0
		_HorizontalBiasNear ("近端眼睛水平偏移量- Horizontal Bias Near", Float) = 0
		_NearStart ("近端偏移开始角度 - Near Start", Range(0, 1)) = 0
		_HorizontalBiasFar ("远端眼睛水平偏移量 - Horizontal Bias Far", Float) = 0
		_FarStart ("远端偏移开始角度 - Far Start", Range(0, 1)) = 0
		_PowerUpIntensity ("PowerUp Intensity", Float) = 0
		[HDR] [Gamma] _PowerUpColor ("PowerUp Color", Vector) = (2.828427,0.6322367,0,0)
		[Toggle] _Highlight ("高光", Float) = 0
		_HighlightColor ("高光颜色", Vector) = (1,1,1,0)
		_HighLightBoost ("高光增强", Range(0, 0.1)) = 0.05
		_HighlightNearStart ("近端高光开始角度", Range(0, 1)) = 0
		_HighlightFarStart ("远端高光开始角度", Range(0, 1)) = 0
		_HighlightHorizontalBiasFar ("远端高光水平偏移", Float) = 0
		_HighlightHorizontalBiasNear ("近端高光水平偏移", Float) = 0
		_HighlightMin ("（法线高光）Highlight Min", Float) = 0.95
		_HighlightMax ("（法线高光）Highlight Max", Float) = 1
		_InitHighlightX ("高光初始偏移 X", Float) = -0.2
		_InitHighlightY ("高光初始偏移 Y", Float) = 0
		_HighlightYOffset ("高光偏移 Y", Float) = 0
		[HideInInspector] _texcoord ("", 2D) = "white" {}
		[HideInInspector] __dirty ("", Float) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = 1;
		}
		ENDCG
	}
	Fallback "Diffuse"
	//CustomEditor "ASEMaterialInspector"
}