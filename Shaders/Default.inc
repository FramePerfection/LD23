TEXTURE Texture;

float4 Emissive = 0;
float4 Ambient = 0.3;
float4 Diffuse = 1;
float4 Specular = 1;
float4 CameraPosition = float4(0, 0, -2, 0);
float SpecSharpness = 4;

float4 TotalLightAmbient;
float4 LightDiffuse;
float4 LightSpecular;
float4 LightDirection;
float4 LightPosition;
float LightRange;
float LightType;

float4x4 World;
float4x4 View;
float4x4 Projection;

struct VSOutput
{
	float4 Position : POSITION;
	float3 WorldPosition : TEXCOORD1;
	float3 Normal : TEXCOORD2;
	float2 Texture : TEXCOORD0;
};


VSOutput ComputeVertex(float4 InPosition : POSITION, float3 InNormal : NORMAL, float2 InTexture : TEXCOORD0)
{
	VSOutput Result = (VSOutput)(0);
	float3 Pos = mul(InPosition, (float4x3)(mul(World, View)));
	Result.WorldPosition = mul(InPosition, (float4x3)(World));
	Result.Position = mul(float4(Pos, 1.0f), Projection);
	Result.Texture = InTexture;
	Result.Normal = normalize(mul(InNormal, (float4x3)(World)));
	
	return Result;
}


float4 PerformSingleLighting(float4 MDiffuse, float4 MSpecular, float SpecularSharpness, float4 LDiffuse, float4 LSpecular, float3 WorldPosition , float4 TexColor, float3 Normal, float3 LightDirection, float LightPower)
{
	float3 LightDir = normalize(LightDirection);
	float3 Eye = normalize(WorldPosition - CameraPosition.xyz);
	float fDiffuse = saturate(dot(Normal, LightDir));
	
	float3 Reflect = normalize(2 * fDiffuse * Normal - LightDir);
	float fSpecular = min(pow(saturate(dot(Reflect, Eye)), SpecularSharpness), fDiffuse);
	return TexColor * (MDiffuse * LDiffuse * fDiffuse * max(LightPower, 0)) + (MSpecular * LSpecular * fSpecular * max(LightPower, 0));
}