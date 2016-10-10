Shader "BMFont/Default"
{
	Properties
	{
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Color ("Tint", Color) = (1, 1, 1, 1)
	}
	SubShader
	{
		Tags {"Queue"="Transparent+100" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            Material {
                Diffuse [_Color]
            }


            Lighting Off Cull Off ZTest Always ZWrite Off Fog { Mode Off }

			SetTexture[_MainTex] {
				constantColor [_Color]
				combine texture * constant
			}
        }
	}
}
