// MUST BE IN SAME ORDER AS THEY ARE USED - ONLY MONOGAME,NOT XNA
sampler paintTexture : register(s0)
{
	Texture = <ScreenTexture>;
};

sampler sandTexture : register(s1)
{
	Texture = (Sand);
};


sampler backgroundTexture : register(s2)
{
	Texture = (Background);
};







float4  PS_PaintDust(
	float4 position : SV_Position, 
	float4 color : COLOR0, 
	float2 texCoord : TEXCOORD0) : COLOR0
{
	
	float4 colorHeight = tex2D(paintTexture, texCoord);
	float4 colorSand = tex2D(sandTexture, texCoord);
	float4 colorBack = tex2D(backgroundTexture, texCoord);
	
	colorHeight.a = 255;
	colorSand.a = 255;

	float sandFactor = colorHeight.r;
	float backgroundFactor = 1.0 - sandFactor;
	float4 colorX = colorBack * backgroundFactor + colorSand * sandFactor;
	return colorX;
}


 
technique PaintDust
{
    pass Pass1
    {
        PixelShader = compile ps_4_0_level_9_1 PS_PaintDust();
    }
}

float4  PS_PaintDustOnly(
	float4 position : SV_Position,
	float4 color : COLOR0,
	float2 texCoord : TEXCOORD0) : COLOR0
{
	float4 colorX = tex2D(paintTexture, texCoord);
	colorX.a=255;
	return colorX;
}
 
technique PaintDustOnly
{
    pass Pass1
    {
        PixelShader = compile ps_4_0_level_9_1 PS_PaintDustOnly();
    }
}

 //float4 getpixel(float2 texCoord, float du, float dv)   
 //{   
 //    texCoord.x = texCoord.x + du;   
 //    texCoord.y = texCoord.y + dv;   
 //    return tex2D(paintTexture, texCoord);   
 //} 
 
 float BlurDownFX = 0.00001; 
 float BlurDownFY = 0.00001; 

float4  PS_BlurDownEffect(
	float4 position : SV_Position,
	float4 color : COLOR0,
	float2 texCoord : TEXCOORD0) : COLOR0
{
	float4 colorX = tex2D(paintTexture, texCoord);
	float height = colorX.r;

	if (texCoord.y<=BlurDownFY)
	{
		float4 colorBelow = tex2D(paintTexture, float2(texCoord.x, texCoord.y+BlurDownFY));
		float heightBelow = colorBelow.r;
		float deltaBelow = 1 - heightBelow;
		if (deltaBelow > height)
			deltaBelow = height;
		height -= deltaBelow;
	}
	else if (texCoord.y>=(1-BlurDownFY))
	{
		float4 colorAbove = tex2D(paintTexture, float2(texCoord.x, texCoord.y-BlurDownFY));
		float heightAbove = colorAbove.r;
		float deltaAbove = 1 - height;
		if (deltaAbove>heightAbove)
			deltaAbove=heightAbove;
		height += deltaAbove;
	}
	else
	{
		float4 colorAbove = tex2D(paintTexture, float2(texCoord.x, texCoord.y-BlurDownFY));
		float4 colorBelow = tex2D(paintTexture, float2(texCoord.x, texCoord.y+BlurDownFY));
		float heightAbove = colorAbove.r;
		float heightBelow = colorBelow.r;
		float deltaAbove = 1 - height;
		if (deltaAbove>heightAbove)
			deltaAbove=heightAbove;
		float deltaBelow = 1 - heightBelow;
		if (deltaBelow > height)
			deltaBelow = height;
		height += deltaAbove;
		height -= deltaBelow;
	}
	
	if (texCoord.x<=BlurDownFX)
	{
		float4 colorRight = tex2D(paintTexture, float2(texCoord.x+BlurDownFX, texCoord.y));
		float heightRight = colorRight.r;		
		float deltaRight = (height - heightRight)/10;
        height = height - deltaRight;	
	}
	else if (texCoord.x>=(1-BlurDownFX))
	{
		float4 colorLeft = tex2D(paintTexture, float2(texCoord.x-BlurDownFX, texCoord.y));
		float heightLeft = colorLeft.r;
		float deltaLeft = (height - heightLeft)/10;
        height = height - deltaLeft;	
	}
	else
	{
		float4 colorLeft = tex2D(paintTexture, float2(texCoord.x-BlurDownFX, texCoord.y));
		float4 colorRight = tex2D(paintTexture, float2(texCoord.x+BlurDownFX, texCoord.y));
		float heightLeft = colorLeft.r;
		float heightRight = colorRight.r;		
		float deltaLeft = (height - heightLeft)/10;
        float deltaRight = (height - heightRight)/10;
        height = height - deltaLeft - deltaRight;		
	}

	colorX.r = height;
	return colorX;
}
 
technique BlurDownEffect
{
    pass Pass1
    {
        PixelShader = compile ps_4_0_level_9_1 PS_BlurDownEffect();
    }
}

  
   

