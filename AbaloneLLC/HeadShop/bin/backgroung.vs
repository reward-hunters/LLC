#version 110

void main(void) 
{
	gl_Position = gl_Vertex;//vec4( gl_Vertex.x * 2.0 - 1.0, 1.0 - gl_Vertex.y * 2.0, 0.0, 1.0);
	gl_TexCoord[0] = gl_MultiTexCoord0;
}