#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 World;
float4x4 View;
float4x4 Projection;

float AmbientIntensity = 0.2;

float4x4 WorldInverseTranspose;
float3 DiffuseLightDirection = float3(0, 1, 0);
float4 DiffuseColor = float4(1, 1, 1, 1);
float DiffuseIntensity = 1.0;

float Shininess = 1;
float4 SpecularColor = float4(1, 1, 1, 1);
float SpecularIntensity = 1.0;
float3 ViewVector = float3(1, 0, 0);

float3 LightPos1 = float3(0, 0, 0);
float3 LightPos2 = float3(0, 0, 0);
float3 LightPos3 = float3(0, 0, 0);
float3 LightPos4 = float3(0, 0, 0);
float LightPower = 1.0;

/*
*************************************************************************
* 
*************************************************************************
*/
float DotProduct(float3 lightPos, float3 pos3D, float3 normal)
{
	float3 lightDir = normalize(pos3D - lightPos);
	return dot(-lightDir, normal);
}

/*
*************************************************************************
* region EFFECT 1
*************************************************************************
*/

struct VertexShaderInput1
{
	float4 Position : POSITION0;
	float4 Normal : NORMAL0;
	float4 Color : COLOR0;
};

struct VertexShaderOutput1
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
};

VertexShaderOutput1 VertexShaderFunction1(VertexShaderInput1 input)
{
	VertexShaderOutput1 output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	
	float4 normal = normalize(mul(input.Normal, World));

	float lightIntensity = dot(normal, DiffuseLightDirection);
	output.Color = saturate(input.Color *DiffuseColor * DiffuseIntensity * lightIntensity + input.Color* AmbientIntensity);
	output.Color.a = 1;

	return output;
}

float4 PixelShaderFunction1(VertexShaderOutput1 input) : COLOR0
{
	return input.Color;
}


/*
*************************************************************************
* region EFFECT 2
*************************************************************************
*/

struct VertexShaderInput2
{
	float4 Position : POSITION0;
	float4 Normal : NORMAL0;
	float4 Color : COLOR0;
};

struct VertexShaderOutput2
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float3 Normal : TEXCOORD0;
};

VertexShaderOutput2 VertexShaderFunction2(VertexShaderInput2 input)
{
	VertexShaderOutput2 output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	float4 normal = normalize(mul(input.Normal, WorldInverseTranspose));
	float lightIntensity = dot(normal, DiffuseLightDirection);
	output.Color = saturate(input.Color * lightIntensity + DiffuseColor * DiffuseIntensity * lightIntensity);
	output.Normal = normal;

	return output;
}

float4 PixelShaderFunction2(VertexShaderOutput2 input) : COLOR0
{
	float3 light = normalize(DiffuseLightDirection);
	float3 normal = normalize(input.Normal);
	float3 r = normalize(2 * dot(light, normal) * normal - light);
	float3 v = normalize(mul(normalize(ViewVector), World));
	float dotProduct = dot(r, v);

	float4 specular = SpecularIntensity * SpecularColor * max(pow(dotProduct, Shininess), 0) * length(input.Color);

	float4 color = saturate(input.Color + input.Color * AmbientIntensity + specular);
	color.a = 1;
	return color;
}


/*
*************************************************************************
* region EFFECT 3 - Point Lights x 3
*************************************************************************
*/

struct VertexShaderInput3
{
	float4 Position : POSITION0;
	float4 Normal : NORMAL0;
	float4 Color : COLOR0;
};

struct VertexShaderOutput3
{
	float4 Position : POSITION0;
	float4 Normal : NORMAL0;
	float4 Color : COLOR0;
	float3 Position3D    : TEXCOORD1;
};

VertexShaderOutput3 VertexShaderFunction3(VertexShaderInput3 input)
{
	VertexShaderOutput3 output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	float4 normal = normalize(mul(input.Normal, WorldInverseTranspose));
	float lightIntensity = dot(normal, DiffuseLightDirection);
	output.Color = saturate(input.Color * lightIntensity + DiffuseColor * DiffuseIntensity * lightIntensity);
	output.Normal = normal;

	output.Position3D = worldPosition;

	return output;

	
	//Output.Position = mul(inPos, xWorldViewProjection);
	//Output.TexCoords = inTexCoords;
	//Output.Normal = normalize(mul(inNormal, (float3x3)xWorld));
	//Output.Position3D = mul(inPos, xWorld);

}

float4 PixelShaderFunction3(VertexShaderOutput3 input) : COLOR0
{
	float f1 = DotProduct(LightPos1, input.Position3D, input.Normal);
	float f2 = DotProduct(LightPos2, input.Position3D, input.Normal);
	float f3 = DotProduct(LightPos3, input.Position3D, input.Normal);
	float f4 = DotProduct(LightPos4, input.Position3D, input.Normal);
	float diffuseLightingFactor = saturate(f1+f2+f3+f4);
	diffuseLightingFactor *= LightPower;
	float4 baseColor = input.Color;
	float4 color = baseColor * (diffuseLightingFactor + AmbientIntensity);
	color.a = 1;
	return color;
}

/*
*************************************************************************
* region TECHNIQUES
*************************************************************************
*/

technique MyTechnique1
{
	pass Pass1
	{
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction1();
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction1();
	}
}

technique MyTechnique2
{
	pass Pass1
	{
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction2();
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction2();
	}
}

technique MyTechnique3
{
	pass Pass1
	{
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction3();
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction3();
	}
}


