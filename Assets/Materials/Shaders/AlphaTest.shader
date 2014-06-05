Shader "Custom/AlphaTest" {
    Properties {
       _MainTex ("Base (RGB) Transparency (A)", 2D) = "" {}
       _Blend ("Blend", Range (0, 1) ) = 0.5
 
    }
    SubShader {
       Pass {
         AlphaTest LEQUAL [_Blend]
         SetTexture [_MainTex] { combine texture }
       }
    }
}