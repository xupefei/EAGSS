sampler TextureSampler : register(s0);
sampler OverlaySampler : register(s1);


float4 main(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
    // Look up the texture color.
    float4 tex = tex2D(TextureSampler, texCoord);
    
    // Look up the fade speed from the scrolling overlay texture.
    float fadeSpeed = tex2D(OverlaySampler, texCoord).x;
    
    // Apply a combination of the input color alpha and the fade speed.
    tex *= saturate((color.a * 2 + fadeSpeed) * 2 - 2);
    
    return tex;
}


technique Desaturate
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 main();
    }
}
