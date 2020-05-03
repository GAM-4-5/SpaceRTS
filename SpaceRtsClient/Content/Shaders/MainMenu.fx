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

sampler TexSampler : register(s0);

Texture2D<float4> noiseTexture;
Texture2D<float4> planetTexture;

sampler noiseSampler = sampler_state
{
    Texture = <noiseTexture>;
};

sampler planetSampler = sampler_state
{
    Texture = <planetTexture>;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 texCoord : TEXCOORD0;
};

static const float sphsize=.7;
static const float dist=.27;
static const float perturb=.3;
static const float displacement=.015;
static const float windspeed=.2;
static const float steps=110.;
static const float stepsize=.025; 
static const float brightness=.43;
static const float3 planetcolor=float3(0.55,0.4,0.3);
static const float fade=.005;
static const float glow=3.5;
static const int iterations=15;
static const float fractparam=0.5;
static const float3 offset=float3(1.5,2.,-1.5);

float wind(float3 p) {
	float d=max(0.,dist-max(0.,length(p)-sphsize)/sphsize)/dist; // Udaljenost od centra planete
	float x=max(0.2,p.x*2.); // svjetlo na lijevoj strani
	p.y*=1.+max(0.,-p.x-sphsize*.25)*1.5; // distororcija na lijevo
	p-=d*normalize(p)*perturb; // distorcija oko planete
	p+=float3(time*windspeed,0.,0.); // pomicanje
	p=abs(frac((p+offset)*.1)-.5);
	for (int i=0; i<iterations; i++) {  
		p=abs(p)/dot(p,p)-fractparam; // distorcija kao kod vatre
	}
	return length(p)*(1.+d*glow*x)+d*glow*x; // apply osvjetljenja 
}

float4 PSMain( VertexShaderOutput input ) : COLOR
{
	float2 uv = input.Position.xy / resolution.xy-.5;
	float3 dir=float3(uv,1.); // smjer zrake
	dir.x*=resolution.x/resolution.y;
	float3 from=float3(0.,0.,-2.+tex2D(noiseSampler,uv*.5+time).x*stepsize);

	// Volumetrično renderanje - TODO optimizirati
	float v=0., l=-0.0001, t=time*windspeed*.2;
	for (float r=10.;r<steps;r++) {
		float3 p=from+r*dir*stepsize;
		float tx=tex2D(noiseSampler,uv*.2+float2(t,0.)).x*displacement; // dodatna distorcija kao od vatre
		if (length(p)-sphsize-tx>0.)
			// zraka se udaljava od planete
			v+=min(50.,wind(p))*max(0.,1.-r*fade); 		
		else if (l<0.) 
			// zraka pogada planetu - TODO optimizirati
			l=pow(max(.53,dot(normalize(p),normalize(float3(-1.,.5,-0.3)))),4.)*(.5+tex2D(planetSampler,uv*float2(2.,1.)*(1.+p.z*.5)+float2(tx+t*.5,0.)).x*2.); // planeta
		}
	v/=steps; v*=brightness;
	float3 col=float3(v*1.25,v*v,v*v*v)+l*planetcolor;
	col*=1.-length(pow(abs(uv),float2(5.,5.)))*14.;
	return float4(col,1.0);
}

technique Planet
{
    pass P0
    {	
        PixelShader = compile PS_SHADERMODEL PSMain();
    }
};
