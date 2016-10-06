// camera parameters
float4x4 World;
float4x4 View;
float4x4 Projection;
float3 CameraPosition;

// viewport parameters
float ViewportWidth = 512; // useless
float ViewportHeight = 512; // useless

// material parameters
float3 DiffuseColor = float3(0.2f, 0.28f, 0.78f);
float DiffuseColorAmount = 0.28f;
float Opacity = 0.8f;

// lighting parameters
float3 LightDirection = float3(1.0f, 1.0f, 1.0f);

// reflection parameters
float4x4 ReflectedView; // useless
textureCUBE ReflectionMap;

sampler ReflectionCubeSampler =
sampler_state
{
	texture = <ReflectionMap>;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
	MipFilter = LINEAR;
};

// wave parameters
float Time = 0.0f;
float WaveLength = 0.48f;
float WaveHeight = 0.1f;
float WaveSpeed = 0.028f;
texture NormalMap;

sampler2D NormalMapSampler =
sampler_state
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
	float4 WorldPosition : TEXCOORD5;
	float2 NormalMapPosition : TEXCOORD2;
};

VertexShaderOutput VS_WaterCubeMap(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4x4 worldViewProjection = mul(World, mul(View, Projection));

	//float4 worldPosition = mul(input.Position, World);
	//float4 viewPosition = mul(worldPosition, View);

	output.Position = mul(input.Position, worldViewProjection);
	//output.Position = mul(viewPosition, Projection);
	output.WorldPosition = mul(input.Position, World);

	output.NormalMapPosition = input.Texture / WaveLength;
	output.NormalMapPosition.y -= Time * WaveSpeed;

	return output;
}

float4 PS_WaterCubeMap(VertexShaderOutput input) : COLOR0
{
	float3 viewDirection = normalize(input.WorldPosition - CameraPosition);
	//float3 viewDirection = normalize(CameraPosition - input.WorldPosition);

	float3 reflectionDirection = reflect(viewDirection, float3(0.0f, 0.0f, 1.0f));
	reflectionDirection = reflectionDirection.yxz;
	reflectionDirection.x = -reflectionDirection.x;

	float4 normal = tex2D(NormalMapSampler, input.NormalMapPosition);

	float3 reflectionTextureOffset = float3(0.0f, 0.0f, 0.0f);
	reflectionTextureOffset.xy = WaveHeight * normal.rg;

	float3 reflection = texCUBE(ReflectionCubeSampler, reflectionDirection + reflectionTextureOffset);

	float3 reflectionVector = -reflect(LightDirection, normal.rgb);

	float specularIntensity = dot(normalize(reflectionVector), viewDirection);
	specularIntensity = pow(specularIntensity, 92.0f);

	return float4(lerp(reflection, DiffuseColor, DiffuseColorAmount) + specularIntensity, Opacity);
}

technique WaterCubeMap
{
	pass Pass0
	{
		// TODO: set renderstates here.

		VertexShader = compile vs_2_0 VS_WaterCubeMap();
		PixelShader = compile ps_2_0 PS_WaterCubeMap();
	}
}
