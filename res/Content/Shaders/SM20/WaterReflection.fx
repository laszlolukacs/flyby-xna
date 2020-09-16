// camera parameters
float4x4 World;
float4x4 View;
float4x4 Projection;
float3 CameraPosition;

// viewport parameters
float ViewportWidth;
float ViewportHeight;

// material parameters
float3 DiffuseColor = float3(0.2f, 0.28f, 0.78f);
float DiffuseColorAmount = 0.28f;
float Opacity = 0.8f;

// lighting parameters
float3 LightDirection = float3(1.0f, 1.0f, 1.0f);

// reflection parameters
float4x4 ReflectedView;
texture ReflectionMap;

sampler2D ReflectionMapSampler = sampler_state
{
	texture = <ReflectionMap>;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
	AddressU = CLAMP;			// Address Mode for U Coordinates, should be MIRROR for better scenery on the edges
	AddressV = CLAMP;			// Address Mode for V Coordinates, should be MIRROR for better scenery on the edges
};

// wave parameters
float Time = 0.0f;
float WaveLength = 0.48f;
float WaveHeight = 0.1f;
float WaveSpeed = 0.028f;
texture NormalMap;

sampler2D NormalMapSampler = sampler_state
{
	texture = <NormalMap>;
	MinFilter = ANISOTROPIC;
	MagFilter = ANISOTROPIC;
	MipFilter = LINEAR;
};
 
struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 Texture : TEXCOORD0;
};
 
struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float4 WorldPosition : TEXCOORD3;
	float4 ReflectedPosition : TEXCOORD1;
	float2 NormalMapPosition : TEXCOORD2;
};
 


VertexShaderOutput VS_WaterReflection(VertexShaderInput input)
{
	VertexShaderOutput output;
 
	float4x4 worldViewProjection = mul(World, mul(View, Projection));
	float4x4 worldReflectedViewProjection = mul(World, mul(ReflectedView, Projection));

	output.Position = mul(input.Position, worldViewProjection);
	output.WorldPosition = mul(input.Position, World);
	output.ReflectedPosition = mul(input.Position, worldReflectedViewProjection);

	output.NormalMapPosition = input.Texture/WaveLength;
	output.NormalMapPosition.y -= Time * WaveSpeed;

	return output;
}
 


float4 PS_WaterReflection(VertexShaderOutput input) : COLOR0
{
	// required for specular
	float3 viewDirection = normalize(CameraPosition - input.WorldPosition);

	// converts a 3D position to 2D screen position
	float2 transientPosition = input.ReflectedPosition.xy / input.ReflectedPosition.w;
	float2 screenPosition = 0.5f * (float2(transientPosition.x, -transientPosition.y) + 1.0f);

	// calculates the size of a half pixel, to convert between texels and pixels
	float2 halfPixel = 0.5f / float2(ViewportWidth, ViewportHeight);

	// sets the reflection texture's position;
	float2 reflectionTexture = screenPosition + halfPixel;

	float4 normal = tex2D(NormalMapSampler, input.NormalMapPosition) * 2.0f - 1.0f;

	float2 reflectionTextureOffset = WaveHeight * normal.rg; // for normal mapping

	float3 reflection = tex2D(ReflectionMapSampler, reflectionTexture + reflectionTextureOffset);

	float3 reflectionVector = -reflect(LightDirection, normal.rgb); // for specular

	float specularIntensity = dot(normalize(reflectionVector), viewDirection);
	specularIntensity = pow(specularIntensity, 92.0f);

	return float4(lerp(reflection, DiffuseColor, DiffuseColorAmount) + specularIntensity, Opacity);
}
 



technique WaterReflectionTechnique
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 VS_WaterReflection();
		PixelShader = compile ps_2_0 PS_WaterReflection();
	}
}