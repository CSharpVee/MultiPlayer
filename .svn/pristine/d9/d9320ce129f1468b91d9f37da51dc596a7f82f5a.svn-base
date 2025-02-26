sampler2D input : register(s0);
float4 tint : register(c0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 color = tex2D(input, uv);
    return color * tint; // Multiply the original color with the tint color
}