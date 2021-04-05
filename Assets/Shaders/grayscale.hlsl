#ifndef grayscale

#define grayscale
/// <summary>
///             Carl tests custom hlsl shaders in HDRP
///             2021-04-05
/// </summary>
void grayscale_float(float3 inColor, float strength, out float3 outColor)
{
    float3 weight = float3(0.299, 0.587, 0.114);
    float luminance = dot(inColor, weight);
    
    float3 finalColor = lerp(float3(luminance, luminance, luminance), inColor, 1.0 - strength);
    outColor = finalColor;
}
	
#endif