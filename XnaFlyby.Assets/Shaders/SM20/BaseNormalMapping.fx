// camera parameters
float4x4 World;
float4x4 WorldInverseTranspose;
float4x4 View;
float4x4 Projection;
float3 CameraPosition;

// clipping plane
bool IsClippingPlaneEnabled = false;
float4 ClipPlane;

// material parameters
float3 AmbientLightColor = float3(0.08f, 0.08f, 0.1f);
float3 DiffuseColor = float3(1.0f, 1.0f, 1.0f);
float3 SpecularColor = float3(1.0f, 1.0f, 1.0f);
float SpecularPower = 64.0f;

// lighting parameters
float3 LightDirection = float3(1.0f, 1.0f, 1.0f);
float3 LightIntensity = float3(1.0f, 1.0f, 1.0f);

// texturing parameters
texture DiffuseTexture;

sampler DiffuseTextureSampler =
sampler_state
{
	texture = <DiffuseTexture>;
	MinFilter = ANISOTROPIC;	// Minification Filter
	MagFilter = ANISOTROPIC;	// Magnification Filter
	MipFilter = LINEAR;			// Mip-mapping
	AddressU = WRAP;			// Address Mode for U Coordinates
	AddressV = WRAP;			// Address Mode for V Coordinates
};

// normal map parameters
texture NormalMap;

sampler2D NormalMapSampler =
sampler_state
{
	texture = <NormalMap>;
	MinFilter = ANISOTROPIC;
	MagFilter = ANISOTROPIC;
	MipFilter = LINEAR;
	AddressU = WRAP;
	AddressV = WRAP;
};

// specular map parameters
texture SpecularMap;

sampler2D SpecularMapSampler =
sampler_state
{
	texture = <SpecularMap>;
	MinFilter = ANISOTROPIC;
	MagFilter = ANISOTROPIC;
	MipFilter = LINEAR;
	AddressU = WRAP;
	AddressV = WRAP;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float3 Tangent : TANGENT0;
	float3 Binormal : BINORMAL0;
	float2 Texture : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float3 WorldPosition : TEXCOORD3;
	float3 Normal : TEXCOORD1;
	float3 Tangent : TEXCOORD4;
	float3 Binormal : TEXCOORD5;
	float2 Texture : TEXCOORD0;
	float3 ViewDirection : TEXCOORD2;
};



VertexShaderOutput VS_BaseNormalMapping(VertexShaderInput input)
{
	VertexShaderOutput output;
	input.Position.w = 1.0f;

	float4 worldPosition = mul(input.Position, World);
	float4x4 viewProjection = mul(View, Projection);

	output.Position = mul(worldPosition, viewProjection);
	output.WorldPosition = worldPosition;
	output.Normal = normalize(mul(input.Normal, WorldInverseTranspose)); // WorldInverseTranspose = (float3x3)World;
	output.Tangent = normalize(mul(input.Tangent, WorldInverseTranspose));
	output.Binormal = normalize(mul(input.Binormal, WorldInverseTranspose));
	output.Texture = input.Texture;
	output.ViewDirection = worldPosition - CameraPosition;

	return output;
}



float4 PS_BaseNormalMapping(VertexShaderOutput input) : COLOR0
{
	// performs clipping if necessary
	if (IsClippingPlaneEnabled) clip(dot(float4(input.WorldPosition, 1), ClipPlane));

	float3 ambientColor = AmbientLightColor;	// ambient color

	float3 diffuseColor = DiffuseColor;			// base diffuse color
	diffuseColor *= tex2D(DiffuseTextureSampler, input.Texture);

	float3 view = normalize(input.ViewDirection);		// normalized direction of the view
	float3 lightDirection = normalize(LightDirection); 	// normalized direction of the light source

	float3 normalMap = tex2D(NormalMapSampler, input.Texture).rgb; // sampling from the normal map
	normalMap = normalMap * 2.0f - 1.0f; // changes the normal's range from [0, 1] to [-1, 1]

	// generates the usable normal vector from the normal plus the sample from the normal map
	float3 normal = normalize((input.Normal + normalMap.x * input.Tangent + normalMap.y * input.Binormal));

	// the reflection direction at the given pixel, based on the normal and the incoming light
	float3 reflectionDirection = reflect(lightDirection, normal);

	// Lambertian directional lighting formula ( kdiff = max(dot(ligth_vector, normal), 0) )
	// also the intensity of the light at the given pixel
	float lightIntensity = saturate(dot(lightDirection, normal));

	float3 output = ambientColor + diffuseColor * saturate(lightIntensity * LightIntensity) ;

	if (lightIntensity > 0.0f)
	{
		// Sample the pixel from the specular map texture.
		float3 specularIntensity = tex2D(SpecularMapSampler, input.Texture).rgb;

		// Phong specular highlights formula ( kspec = max(dot(reflection_vector, view_direction), 0) * normal )
		specularIntensity *= pow(saturate(dot(normalize(2.0f * lightIntensity * reflectionDirection), view)), SpecularPower);
		
		// adds the specular intensity
		output = saturate(output + specularIntensity);
	}

	// returning to the frame buffer
	return float4(output, 1.0f);
}



technique BaseLightingTechnique
{
	pass Pass0
	{
		// TODO: set renderstates here.
		VertexShader = compile vs_2_0 VS_BaseNormalMapping();
		PixelShader = compile ps_2_0 PS_BaseNormalMapping();
	}
}