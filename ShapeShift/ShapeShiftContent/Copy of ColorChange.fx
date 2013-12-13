// Water pp

float4 tint = (0,0,0,1);
sampler samplerState;

float wave ;                // pi/.75 is a good default
float distortion ;        // 1 is a good default
float2 centerCoord;        // 0.5,0.5 is the screen center

float time;

struct PS_INPUT 
{
  float2 TexCoord  : TEXCOORD0;
};

float4 Invert(PS_INPUT Input) : COLOR0 
{  
  float2 distance = abs(Input.TexCoord - centerCoord)/5;
    float scalar = length(distance);

    // invert the scale so 1 is centerpoint
    scalar = abs(1 - scalar);
        
    // calculate how far to distort for this pixel    
    float sinoffset = cos(wave / scalar);
    sinoffset = clamp(sinoffset, 0, 1);
    
    // calculate which direction to distort
    float sinsign = cos(wave / scalar);        
    // reduce the distortion effect
    sinoffset = sinoffset * distortion/32;
    
    // pick a pixel on the screen for this pixel, based on
    // the calculated offset and direction
    float4 color = tex2D(samplerState, Input.TexCoord+(sinoffset*sinsign)) * tint;       
  
  return color;
}

technique PostInvert 
{
  pass P0
  {
    PixelShader = compile ps_2_0 Invert();
  }
}