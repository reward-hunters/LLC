#version 110

uniform sampler2D   u_Texture;
uniform sampler2D   u_BaseTexture;
uniform float		u_BlendDirectionX;

varying vec3 v_OrigitnalPosition;
varying float v_Blending;

void main(void) 
{
	float b = clamp(sign(v_OrigitnalPosition.z), 0.0, 1.0) * v_Blending;
	vec2 t = gl_Color.xy;

	if(u_BlendDirectionX * v_OrigitnalPosition.x > 0.0) {
		t = gl_Color.zw;
	}

	vec4 color = texture2D( u_Texture, t ) * b;
	vec4 colorBase = texture2D( u_BaseTexture, gl_TexCoord[0].st ) * (1.0 - b);
  
	gl_FragColor = vec4(color.xyz + colorBase.xyz, 1.0);
}
