// camera parameters
float4x4 World;
float4x4 View;
float4x4 Projection;

// clipping plane
bool IsClippingPlaneEnabled = false;
float4 ClipPlane;

// material parameters
float3 DiffuseColor = float3(1.0f, 1.0f, 1.0f);

// texturing parameters
bool IsTexturingEnabled = false;
texture2D DiffuseTexture;

sampler2D DiffuseTextureSampler = sampler_state
{
	texture = <DiffuseTexture>;
	MinFilter = ANISOTROPIC;	// Minification Filter
	MagFilter = ANISOTROPIC;	// Magnification Filter
	MipFilter = LINEAR;			// Mip-mapping
	AddressU = WRAP;			// Address Mode for U Coordinates
	AddressV = WRAP;			// Address Mode for V Coordinates
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 Texture : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float3 WorldPosition : TEXCOORD3;
	float2 Texture : TEXCOORD0;
};



VertexShaderOutput VS_Base(VertexShaderInput input)
{
	VertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4x4 viewProjection = mul(View, Projection);

	output.Position = mul(worldPosition, viewProjection);
	output.WorldPosition = worldPosition;
	output.Texture = input.Texture;

	return output;
}



float4 PS_Base(VertexShaderOutput input) : COLOR0
{
	float3 output = DiffuseColor;

	// performs clipping
    if (IsClippingPlaneEnabled)
        clip(dot(float4(input.WorldPosition, 1), ClipPlane));

	// performs texturing
	if (IsTexturingEnabled) output *= tex2D(DiffuseTextureSampler, input.Texture);

	return float4(output, 1.0f);
}



technique BaseTechnique
{
	pass Pass0
	{
		// TODO: set renderstates here.
		VertexShader = compile vs_2_0 VS_Base();
		PixelShader = compile ps_2_0 PS_Base();
	}
}
