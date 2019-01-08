#version 110

uniform float		u_Scale;
uniform float		u_AspectRatio;

void main(void) 
{
	//gl_TexCoord[0].st
	float x = gl_TexCoord[0].x * u_Scale * u_AspectRatio;
	float y = gl_TexCoord[0].y * u_Scale;

	if(fract(x) > 0.5 == fract(y) < 0.5)
	{
		gl_FragColor = vec4(0.8, 0.8, 0.8, 1.0);
	}
	else
	{
		gl_FragColor = vec4(1.0, 1.0, 1.0, 1.0);
	}
}
