#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

layout (std140, binding = 0) uniform Matrices
{
    mat4 view;
    mat4 projection;
};

void main()
{
    vec4 pos = projection * view * vec4(aPos, 1.0);
    gl_Position = pos;
}