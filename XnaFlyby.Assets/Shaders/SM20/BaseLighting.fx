// camera parameters
float4x4 World;
float4x4 View;
float4x4 Projection;
float3 CameraPosition;

// clipping plane
bool IsClippingPlaneEnabled = false;
float4 ClipPlane;

// material parameters
float3 AmbientLightColor = float3(0.1f, 0.1f, 0.1f);
float3 DiffuseColor = float3(1.0f, 1.0f, 1.0f);
float3 SpecularColor = float3(1.0f, 1.0f, 1.0f);
float SpecularPower = 32.0f;

// lighting parameters
float3 LightDirection = float3(1.0f, 1.0f, 1.0f);
float3 LightIntensity = float3(0.9f, 0.9f, 0.9f);

// texturing parameters
bool IsTexturingEnabled = false;
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

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float2 Texture : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float3 WorldPosition : TEXCOORD3;
	float3 Normal : TEXCOORD1;
	float2 Texture : TEXCOORD0;

	float3 ViewDirection : TEXCOORD2;
};



VertexShaderOutput VS_BaseLighting(VertexShaderInput input)
{
	VertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4x4 viewProjection = mul(View, Projection);

	output.Position = mul(worldPosition, viewProjection);
	output.WorldPosition = worldPosition;
	output.Normal = mul(input.Normal, World);
	output.Texture = input.Texture;

	output.ViewDirection = worldPosition - CameraPosition;

	return output;
}



float4 PS_BaseLighting(VertexShaderOutput input) : COLOR0
{
	// performs clipping if necessary
	if (IsClippingPlaneEnabled) clip(dot(float4(input.WorldPosition, 1), ClipPlane));

	float3 ambientColor = AmbientLightColor; // ambient color

	float3 diffuseColor = DiffuseColor;			// base diffuse color
	// performs texturing if necessary
	if (IsTexturingEnabled) diffuseColor *= tex2D(DiffuseTextureSampler, input.Texture);

	float3 view = normalize(input.ViewDirection);		// normalized direction of the view
	float3 lightDirection = normalize(LightDirection); 	// normalized direction of the light

	// the normal at the given pixel
	float3 normal = normalize(input.Normal);

	// the reflection direction at the given pixel, based on the normal and the incoming light
	float3 reflectionDirection = reflect(lightDirection, normal);

	// Lambertian directional lighting formula ( kdiff = max(dot(ligth_vector, normal), 0) )
	// the intensity of the light at the given pixel
	float3 lightIntensity = saturate(saturate(dot(lightDirection, normal)) * LightIntensity);

	// Phong specular highlights formula ( kspec = max(dot(reflection_vector, view_direction), 0) * normal )
	float3 specularIntensity = pow(saturate(dot(normalize(2.0f * lightIntensity * reflectionDirection), view)), SpecularPower) * SpecularColor;
	
	// applying the calculated Lambertian light intesity at the pixel
	float3 output = saturate(ambientColor + diffuseColor * lightIntensity + specularIntensity);

	// returning to the frame buffer
	return float4(output, 1.0f);
}



technique BaseLightingTechnique
{
	pass Pass0
	{
		// TODO: set renderstates here.
		VertexShader = compile vs_2_0 VS_BaseLighting();
		PixelShader = compile ps_2_0 PS_BaseLighting();
	}
}
