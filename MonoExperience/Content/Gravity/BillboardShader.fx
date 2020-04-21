float4x4 world : World;
float4x4 vp : ViewProjection;

#define worldUp cross(half3(-1,0,0),half3(0,0,-1))

texture particleTexture;
sampler partTextureSampler = sampler_state 
{ 
    Texture = <particleTexture>; 
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
};

struct VertexIn
{
    half4 Position       : POSITION0;             
    half2 TextureCoords: TEXCOORD0;    
    half4 Color        : COLOR0;
    float Size : POSITION1;
};
struct VertexOut
{
    half4 Position       : POSITION0;      
    half2 TextureCoords: TEXCOORD0;
    half4  Color        : COLOR0;    
};

struct PixelToFrame
{
    half4 Color : COLOR0;
};
VertexOut VS(VertexIn input)
{
    VertexOut Out = (VertexOut)0;
    
    half3 center = mul(input.Position,world);    
    half3 eyeVector = vp._m02_m12_m22;
    
    half3 finalPos = center;
    half3 side;
    half3 up;
    
    side = normalize(cross(eyeVector,worldUp));    
    up = normalize(cross(side,eyeVector));        
    
    finalPos += (input.TextureCoords.x - 0.5) * side * input.Size;
    finalPos += (0.5 - input.TextureCoords.y) * up * input.Size;
    
    half4 finalPos4 = half4(finalPos,1);    
    
    Out.Position = mul(finalPos4,vp);
    Out.TextureCoords = input.TextureCoords;
    
    Out.Color = input.Color;    
    
    return Out;
}

PixelToFrame PS(VertexOut input)
{
    PixelToFrame Out = (PixelToFrame)0;
    
    half2 texCoord;
    
    texCoord = input.TextureCoords.xy;    
    
    half4 color = tex2D(partTextureSampler,texCoord);
    
    Out.Color = color * input.Color;    
    
    return Out;
}

technique Go
{
    pass P0 
    {
        VertexShader = compile vs_4_0_level_9_1 VS();
        PixelShader  = compile ps_4_0_level_9_1 PS();
    }
}