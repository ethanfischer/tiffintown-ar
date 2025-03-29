// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


sampler2D _MainTex;
half4 _MainTex_TexelSize;//Unity fills this out automatically since it's part of the material properties
sampler2D _MainTexAlpha;
sampler2D _LightmapTex;
samplerCUBE _ReflectionCubeMap;
sampler2D_float _TransDataTex;
half4 _TransDataTex_TexelSize;//Unity fills this out automatically since it's part of the material properties
sampler2D_float _AtlasDataTex;
half4 _AtlasDataTex_TexelSize;//Unity fills this out automatically since it's part of the material properties

float _MipClamp;

// Dollhouse use only
float4x4 _ClippingMatrix;
float _ClippingEnabled = 1.0;

float3 _LightDir = float3(0,1,0);
float4 _AmbientColor = float4(1,1,1,1);
float4 _DirectionalColor = float4(1,1,1,1);

// Shadows
float4x4 _ShadowMatrix;
float4 _ShadowColor;

//float _Brightness;
//float _Contrast;
//float _Saturation;
//float _Gamma;

//#define DEBUG_SCREEN_POS
//#define VISUALIZE_MIP_LEVELS


struct InVertex
{
	float3 vertex : POSITION;
	float4 normal : NORMAL;
	float4 color : COLOR0;
	float4 data : TEXCOORD0;
	float4 uv : TEXCOORD1;
	float4 params : TEXCOORD2;

	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct OutVertexInPixel
{
	float4 vertex : SV_POSITION;
	float4 color : COLOR;
	float4 uv : TEXCOORD0;
	float4 atlas : TEXCOORD1;
	float3 worldNorm : TEXCOORD2;
	float4 params : TEXCOORD3;
	float4 pos : TEXCOORD4;
#ifdef DEBUG_SCREEN_POS
	float4 screen : TEXCOORD5;
#endif

	UNITY_VERTEX_OUTPUT_STEREO
};

struct Selection_OutVertexInPixel
{
	float4 vertex : SV_POSITION;
	float4 color : COLOR;
	float4 uv : TEXCOORD0;
	float4 atlas : TEXCOORD1;
	float4 data : TEXCOORD2;
	float4 params : TEXCOORD3;
#ifdef DEBUG_SCREEN_POS
	float4 screen : TEXCOORD4;
#endif

	UNITY_VERTEX_OUTPUT_STEREO
};

void ColorSpaceCorrection(inout float4 clr)
{
	// Used in SMX2 for Dollhouses
	// Do not use this if using Gamma space
	float powVal = 2.2;
	clr = pow(clr, float4(powVal, powVal, powVal, 1));
	clr = saturate(clr);
}

float4x4 BuildWorldMatrix(float transformIdx)
{
	float4x4 worldMat;
	float2 dataUV[3];
	float whole[3];
	float fract[3];

	transformIdx *= 3.0;

	fract[0] = modf(float(transformIdx) * _TransDataTex_TexelSize.x, whole[0]);
	dataUV[0] = float2(fract[0], whole[0] * _TransDataTex_TexelSize.x);
	worldMat[0] = tex2Dlod(_TransDataTex, float4(dataUV[0], 0, 0));

	fract[1] = modf(float(transformIdx + 1) * _TransDataTex_TexelSize.x, whole[1]);
	dataUV[1] = float2(fract[1], whole[1] * _TransDataTex_TexelSize.x);
	worldMat[1] = tex2Dlod(_TransDataTex, float4(dataUV[1], 0, 0));

	fract[2] = modf(float(transformIdx + 2) * _TransDataTex_TexelSize.x, whole[2]);
	dataUV[2] = float2(fract[2], whole[2] * _TransDataTex_TexelSize.x);
	worldMat[2] = tex2Dlod(_TransDataTex, float4(dataUV[2], 0, 0));

	worldMat[3] = float4(0, 0, 0, 1);


	return worldMat;
}

void ProcessVertex(InVertex i, inout OutVertexInPixel o)
{
	o.color = i.color;
	o.uv = i.uv;
	o.params = i.params;

	uint transformIdx = (int(i.data.r * 255.0) << 8) | (int(i.data.g * 255.0));
	uint textureIdx = (int(i.data.b * 255.0) << 8) | (int(i.data.a * 255.0));

	float4x4 worldMat = BuildWorldMatrix(transformIdx);
	float4 worldPos = mul(worldMat, float4(i.vertex, 1.0));
	o.pos = mul(unity_ObjectToWorld, worldPos);
	o.vertex = UnityObjectToClipPos(worldPos);

	// We only use the world normal for reflections. It's not at accurate but
	// gives us enough of an effect that it works. Anything more would be slower
	// without much more value as we're not trying to be super accurate
	o.worldNorm = mul(worldMat, float4(i.normal.xyz, 0.0)).xyz;
	o.atlas = tex2Dlod(_AtlasDataTex, float4((float)textureIdx * _AtlasDataTex_TexelSize.x, 0, 0, 0));

#ifdef DEBUG_SCREEN_POS
	o.screen = ComputeScreenPos(o.vertex);
#endif
}

void ProcessShadowVertex(InVertex i, inout OutVertexInPixel o)
{
	o.color = i.color;
	o.uv = i.uv;
	o.params = i.params;
	o.worldNorm = i.normal.xyz;
	o.atlas = float4(0,0,0,0);

	uint transformIdx = (int(i.data.r * 255.0) << 8) | (int(i.data.g * 255.0));
	float4x4 worldMat = BuildWorldMatrix(transformIdx);
	float4 worldPos = mul(worldMat, float4(i.vertex, 1.0));

	o.pos = mul(unity_ObjectToWorld, worldPos);
	o.pos = mul(_ShadowMatrix, o.pos);
	o.vertex = mul(UNITY_MATRIX_VP, o.pos);

#ifdef DEBUG_SCREEN_POS
	o.screen = ComputeScreenPos(o.vertex);
#endif
}

void Selection_ProcessVertex(InVertex i, inout Selection_OutVertexInPixel o)
{
	o.data = i.data;
	o.color = i.color;
	o.uv = i.uv;
	o.params = i.params;

	uint transformIdx = (int(i.data.r * 255.0) << 8) | (int(i.data.g * 255.0));
	uint textureIdx = (int(i.data.b * 255.0) << 8) | (int(i.data.a * 255.0));

	float4x4 worldMat = BuildWorldMatrix(transformIdx);
	o.vertex = mul(worldMat, float4(i.vertex, 1.0));
	o.vertex = UnityObjectToClipPos(o.vertex);

	o.atlas = tex2Dlod(_AtlasDataTex, float4((float)textureIdx * _AtlasDataTex_TexelSize.x, 0, 0, 0));

#ifdef DEBUG_SCREEN_POS
	o.screen = ComputeScreenPos(o.vertex);
#endif
}

float GetMipmapLevel(float2 textureUV, float2 resScalar)
{
	textureUV = (textureUV * _MainTex_TexelSize.z) * resScalar;
	float2 dx = ddx(textureUV);
	float2 dy = ddy(textureUV);
	float delta_max_sqr = max(dot(dx, dx), dot(dy, dy));
	return min(max(0.0, 0.5 * log2(delta_max_sqr) - 2.0), _MipClamp);
}

void ApplyAtlasTextureAndAlphaCutoff(inout float4 clr, float4 atlasData, float2 uv, float alphaCutoff)
{
	float mip = GetMipmapLevel(uv, atlasData.zw);

	// We use the typical repeat uvs for basic colors, but we need to use
	// the triangle-wave mirrored repeat uvs for alpha to avoid seams caused by msaa.
	// Hopefully this is a temp hack-fix but eventually it'd be good to actually store
	// and respect uv sampling modes we find in export.
	float2 repeatUV = frac(uv);
	repeatUV *= atlasData.zw;
	repeatUV += atlasData.xy;
	float2 mirrorUV = 2.0 * abs((uv * 0.5) - floor(uv * 0.5 + 0.5));
	mirrorUV *= atlasData.zw;
	mirrorUV += atlasData.xy;

	clr.a *= tex2Dlod(_MainTexAlpha, float4(mirrorUV, 0, mip)).r;
	if(clr.a <= alphaCutoff)
	{
		discard;
	}

	clr.rgb *= tex2Dlod(_MainTex, float4(repeatUV, 0, mip)).rgb;

#ifdef VISUALIZE_MIP_LEVELS
	if(mip > 5)
		clr.rgb = float3(1, 0, 1);
	else if(mip > 4)
		clr.rgb = float3(0, 1, 1);
	else if(mip > 3)
		clr.rgb = float3(1, 1, 0);
	else if(mip > 2)
		clr.rgb = float3(0, 0, 1);
	else if(mip > 1)
		clr.rgb = float3(0, 1, 0);
	else if(mip > 0)
		clr.rgb = float3(1, 0, 0);
#endif
}

void ApplyLightmaps(inout float4 clr, float4 uv)
{
	// If we aren't supposed to use lightmaps the uvs will be -1... but we may have ended up 
	// in material that has lightmaps, if so just bail out early
	if (uv.z < 0)
		return;

	// If we don't have a valid lightmap texture bound it should just default to white for us
	clr.rgb *= tex2D(_LightmapTex, uv.zw).rgb;
}

void CalculateReflections(inout float4 clr, float3 normal, float4 params)
{
	// THIS IS SUBJECTIVELY CHOSEN MATH... NO SCIENCE HERE, JUST PRETTY PIXELS

	// First apply reflection coloring from a cubemap
	normal = normalize(normal);
	float reflectivity = params.r;
	float4 reflection = texCUBE(_ReflectionCubeMap, normal);
	// The stronger the whiteness the stronger the alpha
	reflection.a = clr.a + (reflection.r + reflection.g + reflection.b) * 0.333;
	float4 reflectClr = lerp(clr, clr + reflection, reflectivity);

	// Now add some specularity
	float dotLerp = saturate(dot(normal, UNITY_MATRIX_V[2].xyz));
	float specular = pow(dotLerp, 30) * params.g * reflectivity;
	clr = lerp(reflectClr, reflectClr + (reflectClr + reflection), specular);
	
	// Make sure we stay within proper color ranges (0 - 1)
	clr = saturate(clr);
}

bool CheckClipping(float4 pos)
{
	float3 invisPos = mul(_ClippingMatrix, pos).xyz;
	float3 volume = float3(0.5, 0.5, 0.5);
	return (!all(clamp(invisPos, -volume, volume) == invisPos));
}

//float3 rgb_to_hsv(float3 c)
//{
//	float4 K = float4(0.0, -0.33333, 0.66666, -1.0);
//	float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
//	float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
//
//	float d = q.x - min(q.w, q.y);
//	float e = 1.0e-10;
//	return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
//}
//
//float3 hsv_to_rgb(float3 c)
//{
//	float4 K = float4(1.0, 0.66666, 0.33333, 3.0);
//	float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
//	return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
//}

//void ApplyColorCorrection(inout float3 color)
//{
//	float oneOverGamma = 1.0 / _Gamma;
//
//	// Adjust gamma
//	color = pow(color, float3(oneOverGamma, oneOverGamma, oneOverGamma));
//
//	// Adjust saturation and brightness
//	color = rgb_to_hsv(color);
//	color.y = clamp(color.y * _Saturation, 0.0, 1.0);
//	color.z = clamp(color.z + _Brightness - 1.0, 0.0, 1.0);
//	color = hsv_to_rgb(color);
//
//	// Adjust contrast
//	color = (color - float3(0.5, 0.5, 0.5)) * _Contrast + float3(0.5, 0.5, 0.5);
//
//	// Make sure we stay within 0.0 -> 1.0
//	color = saturate(color);
//}

void CalculateLighting(inout float4 color, float3 normal)
{
    float  dotProd = dot(normal, _LightDir);
    dotProd = saturate(dotProd * 0.5 + 0.5);
    color = lerp(_AmbientColor * color, _DirectionalColor * color, dotProd);
}

#ifdef DEBUG_SCREEN_POS
float2 GetNormalizedScreenPos(float4 inScreenPos)
{
	return inScreenPos.xy / inScreenPos.w;
}
#endif
