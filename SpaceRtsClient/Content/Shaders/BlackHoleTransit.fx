#if OPENGL
	//#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float2 resolution;           
float time;                 

Texture2D<float4> Tex : register(t0);

sampler TexSampler : register(s0) = sampler_state {
    Texture = <Tex>;
    
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 texCoord : TEXCOORD0;
};

float4 PSMain( VertexShaderOutput input ) : COLOR
{
	float2 uv = input.Position.xy / resolution.xy;
    
    // get the distance between current pixel and the center
    float2 dist = uv-.5;
    float radius = .5;
    // uncomment the line 41 to see the val variable values
    // we will manipulate the white area
    float val = 1.-smoothstep(radius-(radius*2.),
                         radius+(radius*0.8),
                         dot(dist,dist)*4.0);
    
    // get the angle between the current pixel and the center
    float angle = atan2(dist.y, dist.x);
    
    // some movement to see the effect based on iTime
    float s = sin(time);
    
    // get a new angle based on the original to get a near pixel value
    // multiplied by distance to only manipulate the pixels inside the radius
    float theta = angle + (s*val*val*10.);
    uv.x -= cos(theta)*val*s;
    uv.y -= sin(theta)*val*s;
    
    // get texture color values
    float4 col = tex2D(TexSampler, uv);
    
    // add the multiplication of val and s to the final color
    // to get the center of the image brighter or darker.
    
    // Comment the line below to get the result without the bright/dark effect
    col += float4(val*s*1.5,val*s*1.5,val*s*1.5,val*s*1.5);
    
    // uncommnet the lines below to change the image color
	return float4(col.xyz, 1.);
	//fragColor = float4(col.xzy, 1.);
	//fragColor = float4(col.yzx, 1.);
	//fragColor = float4(col.yxz, 1.);
	//fragColor = float4(col.zyx, 1.);
	//fragColor = float4(col.zxy, 1.);
    
    // fragColor = float4(val);
}

technique Transit
{
    pass P0
    {	
        PixelShader = compile PS_SHADERMODEL PSMain();
    }
};
