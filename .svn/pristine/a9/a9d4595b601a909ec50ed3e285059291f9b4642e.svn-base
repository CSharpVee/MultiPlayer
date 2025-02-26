sampler2D input : register(s0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 color = tex2D(input, uv);
    float lumi = dot(color.rgb, float3(0.299, 0.587, 0.114));
    return float4(lumi, lumi, lumi, color.a);
}