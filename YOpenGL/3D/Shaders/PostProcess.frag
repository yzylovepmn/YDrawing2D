#version 330 core

in vec2 texCoords;
out vec4 FragColor;
uniform sampler2DMS textureMS;

void main()
{
	ivec2 tex = ivec2(texCoords);
	vec4 colorSample1 = texelFetch(textureMS, tex, 0);
	vec4 colorSample2 = texelFetch(textureMS, tex, 1);
	vec4 colorSample3 = texelFetch(textureMS, tex, 2);
	vec4 colorSample4 = texelFetch(textureMS, tex, 3);
	vec3 color = (colorSample1.rgb +  colorSample2.rgb + colorSample3.rgb + colorSample4.rgb) / 4;
	FragColor = vec4(color, 1f);
}