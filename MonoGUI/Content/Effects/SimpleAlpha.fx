// Just test... 
float4 PixelShaderFunction(float2 coords: TEXCOORD0) : COLOR0
{

	return float4(1, 0, 0, 1);
}

technique Technique1
{
	pass Pass1
	{
		PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
	}
}
