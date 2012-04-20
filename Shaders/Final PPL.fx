#include <Default.inc>

float multiply;
bool EnableShadows = true;

sampler LinearSampler = sampler_state 
{ Texture = <Texture>; AddressU = Wrap; AddressV = Wrap; MIPFilter = Linear; MinFilter = Linear; MagFilter = Linear;};

TEXTURE ShadowMap;
sampler ShadowSampler = sampler_state 
{ Texture = <ShadowMap>; MinFilter = Linear; MagFilter = Linear;};

bool TestShadow(float3 WorldPosition)
{


	float4 PLightDirection = 0.0f;
	PLightDirection.xyz = WorldPosition - LightPosition.xyz;
	float distance = length(PLightDirection.xyz);
	PLightDirection.xyz = PLightDirection.xyz / distance;

	//sample depth from cubic shadow map                         		 
	float2 moments = texCUBE(ShadowSampler, float4((PLightDirection.xyz), 0.0f)).rg;
	moments *= 90;
	moments.y *= 90;

	float lit_factor = (distance <= moments[0]);
 
	float E_x2 = moments.y;
	float Ex_2 = moments.x * moments.x;
	float variance = min(max(E_x2 - Ex_2, 0.0)*0.6 - 0.5, 1.0);
	float m_d = (moments.x - distance);
	float p = variance / (variance + m_d * m_d); //Chebychev's inequality

	return max(lit_factor, p);

}


float4 ComputePixel(VSOutput In) : COLOR
{
	float4 TexColor = tex2D(LinearSampler, In.Texture);
	float4 result = 0;
	float ShadowMul = 1;

	if (EnableShadows)
	{ShadowMul = TestShadow(In.WorldPosition);}

	if (ShadowMul > 0)
	{
		if (LightType == 3)
			{result = PerformSingleLighting(Diffuse, Specular, SpecSharpness, LightDiffuse, LightSpecular, In.WorldPosition, TexColor, In.Normal, LightDirection.xyz, 1) * multiply * ShadowMul;}
		else
			{result = PerformSingleLighting(Diffuse, Specular, SpecSharpness, LightDiffuse, LightSpecular, In.WorldPosition, TexColor, In.Normal, normalize(In.WorldPosition - LightPosition.xyz), max(LightRange - length(In.WorldPosition - LightPosition), 0) / LightRange) * multiply * ShadowMul;}
	}	
	return result;
}



float4 AmbientPass(VSOutput In) : COLOR
{
	float4 TexColor = tex2D(LinearSampler, In.Texture);
	return TexColor * (Emissive + Ambient * TotalLightAmbient);
}

Technique T1
{
	Pass Ambient
	{
		ZWriteEnable = true;
		PixelShader = compile ps_3_0 AmbientPass();
		VertexShader = compile vs_3_0 ComputeVertex();
	}
	Pass Lighting
	{
		SrcBlend = One;
		DestBlend = One;
		AlphaBlendEnable = true;
		ZWriteEnable = false;
		PixelShader = compile ps_3_0 ComputePixel();
		VertexShader = compile vs_3_0 ComputeVertex();
	}
}