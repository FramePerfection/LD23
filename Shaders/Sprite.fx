
TEXTURE Texture;

sampler DefaultSampler = sampler_state
{Texture = <Texture>; MagFilter = Linear; AddressU = Wrap; AddressV = Wrap;};

float4 PixelShaderFunc(float2 InTex : TEXCOORD0) : COLOR
{
	return tex2D(DefaultSampler, InTex);
}

TECHNIQUE T1
{
	PASS P1
	{
		PixelShader = compile ps_2_0 PixelShaderFunc();
	}
}