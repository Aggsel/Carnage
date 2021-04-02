#ifndef test

#define test

void test_float(float2 uv, out float3 color)
{
	color = float3(uv.x, uv.y, 1.0);
}
	
#endif