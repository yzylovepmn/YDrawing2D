#version 330 core
layout (location = 0) in vec2 aPos;
layout (location = 1) in vec2 aTexCoords;

out vec2 texCoords;
uniform vec2 screenSize;

void main()
{
    texCoords = aTexCoords * screenSize;
    gl_Position = vec4(aPos, 0, 1);
}