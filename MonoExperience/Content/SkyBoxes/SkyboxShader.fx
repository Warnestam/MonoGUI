float4x4 World;
float4x4 View;
float4x4 Projection;
float3 CameraPosition;
Texture SkyBoxTexture; 

samplerCUBE SkyBoxSampler = sampler_state 
{ 
   texture = <SkyBoxTexture>; 
   magfilter =  LINEAR; 
   minfilter = LINEAR; 
   mipfilter = LINEAR; 
   AddressU = Mirror; 
   AddressV = Mirror; 
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float3 TextureCoordinate : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

	float4 VertexPosition = mul(input.Position, World);
	output.TextureCoordinate = VertexPosition - CameraPosition;
	
    return output;	
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 output;
	output = texCUBE(SkyBoxSampler, normalize(input.TextureCoordinate));
	output.a = 1.0f;
	return output;
}

technique Skybox
{
    pass Pass1
    {
        VertexShader = compile vs_4_0_level_9_1 VertexShaderFunction();
        PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
    }
}
