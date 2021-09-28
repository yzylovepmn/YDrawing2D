#version 330 core
uniform sampler1D pattern;
uniform vec4 color;
uniform bool dashed;

in float texCoord;

out vec4 FragColor;

void main()
{
    if(dashed && texture(pattern, texCoord).r < 0.5)
        discard;
    FragColor = color;
}