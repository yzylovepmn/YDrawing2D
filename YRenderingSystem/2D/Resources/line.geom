#version 330 core
layout(lines) in;
layout(line_strip, max_vertices=2) out;

uniform vec2 screenSize;
uniform bool dashed;
uniform float dashedFactor;

out float texCoord;

void main()
{
	if(dashed)
	{
		vec2 winPos0 = screenSize.xy * gl_in[0].gl_Position.xy;
		vec2 winPos1 = screenSize.xy * gl_in[1].gl_Position.xy;
		gl_Position = gl_in[0].gl_Position;
		texCoord = 0.0;
		EmitVertex();
		gl_Position = gl_in[1].gl_Position;
		texCoord = length(winPos1-winPos0) / dashedFactor;
		EmitVertex();
	}
	else
	{
		gl_Position = gl_in[0].gl_Position;
		EmitVertex();
		gl_Position = gl_in[1].gl_Position;
		EmitVertex();
	}
}