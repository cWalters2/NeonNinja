Shader "Custom/Distort4" {
	Properties{
		_Refraction("Refraction", Range (0.00, 100.0)) = 1.0
		_DistortTex("Base (RGB)", 2D) = "white" {}
	}

	SubShader {
	 
	 Tags { "RenderType"="Transparent" "Queue"="Overlay" }
	 LOD 100
	 
	 GrabPass
	 {
	  
	 }
 
CGPROGRAM
#pragma surface surf NoLighting
#pragma vertex vert

fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten){
        fixed4 c;
        c.rgb = s.Albedo;
        c.a = s.Alpha;
        return c;
  }

sampler2D _GrabTexture : register(s0);
sampler2D _DistortTex : register(s2);
float _Refraction;

float4 _GrabTexture_TexelSize;

struct Input {
 float2 uv_DistortTex;
 float3 color;
 float3 worldRefl;
 float4 screenPos;
 INTERNAL_DATA
};

void vert (inout appdata_full v, out Input o) {
  UNITY_INITIALIZE_OUTPUT(Input,o);
  o.color = v.color;
}

void surf (Input IN, inout SurfaceOutput o) 
{
    float4 distort = tex2D(_DistortTex, IN.uv_DistortTex) * IN.color.r;
    float2 offset = distort.rgb * _Refraction * _GrabTexture_TexelSize.xy;

    IN.screenPos.xy = (IN.screenPos.xy+0.036) / IN.screenPos.w;
    
   
        IN.screenPos.y = 1-IN.screenPos.y-0.003;
        
    IN.screenPos.xy = offset * IN.screenPos.z + IN.screenPos.xy;

    float4 refrColor = tex2D(_GrabTexture, IN.screenPos.xy); //tex2Dproj(_GrabTexture, IN.screenPos);
    o.Alpha = distort.a;
    o.Emission = refrColor.rgb;
}
ENDCG
}
}