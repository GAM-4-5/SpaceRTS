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

float4 AmbientColor = float4(1, 1, 1, 1);
float AmbientIntensity = 0.1;

float4x4 WorldInverseTranspose;

float3 DiffuseLightDirection = float3(1, 1, 0);
float4 DiffuseColor = float4(1, 1,1, 1);
float DiffuseIntensity = 0.5;
 
texture ModelTexture;
sampler2D textureSampler = sampler_state {
    Texture = (ModelTexture);
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};
 
struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float4 Normal : NORMAL0;
    float2 TextureCoordinate : TEXCOORD0;
};
 
struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float3 Normal : TEXCOORD0;
    float2 TextureCoordinate : TEXCOORD1;
};
 
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    float4x4 WorldViewProjection = mul(mul(World, View), Projection);
    
    VertexShaderOutput output = (VertexShaderOutput)0;

	//output.Position = mul(input.Position, WorldViewProjection); //mul(input.Position + float4(0,0,sin(Time + input.Position.x),0), WorldViewProjection);
	
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    //output.Color = input.Color;

    //float4 normal = normalize(mul(input.Normal, mul(View, World)));
    // output.Normal = normal;

    output.TextureCoordinate = input.TextureCoordinate;

    float lightIntensity = dot(input.Normal, DiffuseLightDirection);
    output.Color = input.Color * lightIntensity;

	return output;

    
 
    // float4 normal = normalize(mul(input.Normal, WorldInverseTranspose));
    // float lightIntensity = dot(normal, DiffuseLightDirection);
    // output.Color = saturate(DiffuseColor * DiffuseIntensity * lightIntensity);
 
    // output.Normal = normal;
 
    // output.TextureCoordinate = input.TextureCoordinate;
    // return output;
}
 
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // return input.Color;

    // float3 light = normalize(DiffuseLightDirection);
    // float3 normal = normalize(input.Normal);
    // float3 r = normalize(2 * dot(light, normal) * normal - light);
    // float3 v = normalize(mul(normalize(ViewVector), World));
    // float dotProduct = dot(r, v);
 
    // // float4 specular = SpecularIntensity * SpecularColor * max(pow(dotProduct, Shininess), 0) * length(input.Color);
 
    float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);
    textureColor.a = 1;

    return textureColor;
 
    // return saturate(textureColor * (input.Color) + AmbientColor * AmbientIntensity + specular);
}
 
technique Textured
{
    pass Pass1
    {
        VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
        PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
    }
}


// float4x4 World;
// float4x4 View;
// float4x4 Projection;
 
// float4 AmbientColor = float4(1, 1, 1, 1);
// float AmbientIntensity = 0.1;
 
// float3 DiffuseLightDirection = float3(1, 0, 0);
// float4 DiffuseColor = float4(1, 1, 1, 1);
// float DiffuseIntensity = 1.0;
 
// float Shininess = 200;
// float4 SpecularColor = float4(1, 1, 1, 1);
// float SpecularIntensity = 1;
// float3 ViewVector = float3(1, 0, 0);

// texture TileTexture;
// sampler2D textureSampler = sampler_state {
//     Texture = (TileTexture);
//     MinFilter = Linear;
//     MagFilter = Linear;
//     AddressU = Clamp;
//     AddressV = Clamp;
// };
 
// struct VertexShaderInput
// {
//     float4 Position : POSITION0;
//     float4 Normal : NORMAL0;
//     float2 TextureCoordinate : TEXCOORD0;
// };
 
// struct VertexShaderOutput
// {
//     float4 Position : POSITION0;
//     float4 Color : COLOR0;
//     float3 Normal : TEXCOORD0;
//     float2 TextureCoordinate : TEXCOORD1;
// };
 
// VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
// {
//     VertexShaderOutput output;
 
//     float4x4 worldViewProjection = mul(mul(World,View), Projection);
//     float4x4 worldView = mul(World,View);

//     output.Position = mul(input.Position, worldViewProjection);
 
//     float4 normal = normalize(mul(worldView, input.Normal));
//     float lightIntensity = dot(normal, DiffuseLightDirection);
//     output.Color = saturate(DiffuseColor * DiffuseIntensity * lightIntensity);
 
//     output.Normal = normal;
 
//     output.TextureCoordinate = input.TextureCoordinate;
//     return output;
// }
 
// float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
// {
//     float3 light = normalize(DiffuseLightDirection);
//     float3 normal = normalize(input.Normal);
//     float3 r = normalize(2 * dot(light, normal) * normal - light);
//     float3 v = normalize(mul(normalize(ViewVector), World));
//     float dotProduct = dot(r, v);
 
//     float4 specular = SpecularIntensity * SpecularColor * max(pow(dotProduct, Shininess), 0) * length(input.Color);
 
//     float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);
//     textureColor.a = 1;
 
//     return saturate(textureColor * (input.Color) + AmbientColor * AmbientIntensity + specular);
// }

// technique Textured
// {
// 	pass P0
// 	{
// 		VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
// 		PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
// 	}
// };

