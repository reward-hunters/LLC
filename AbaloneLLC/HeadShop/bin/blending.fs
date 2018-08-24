#version 110

uniform sampler2D   u_Texture;
uniform sampler2D   u_BaseTexture;
uniform float		u_BlendDirectionX;

varying vec3 v_OrigitnalPosition;

void main(void) 
{
	float ax = v_OrigitnalPosition.x;
	float ay = v_OrigitnalPosition.y;
	float k = 1.0 - clamp((ax * ax + ay * ay) / 49.0, 0.0, 1.0);

	float b = clamp(sign(v_OrigitnalPosition.z), 0.0, 1.0) * k;
	vec2 t = gl_Color.xy;

	if(u_BlendDirectionX * v_OrigitnalPosition.x > 0.0) {
		t = gl_Color.zw;
	}

	vec4 color = texture2D( u_Texture, t ) * b;
	vec4 colorBase = texture2D( u_BaseTexture, gl_TexCoord[0].st ) * (1.0 - b);
  
	gl_FragColor = vec4(color.xyz + colorBase.xyz, 1.0);
}
