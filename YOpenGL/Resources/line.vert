#version 330 core
layout (location = 0) in vec2 aPos;

layout (std140, binding = 0) uniform Matrices
{
    mat3 worldToNDC;
    mat3 view;
};

void main()
{
    vec3 pos = worldToNDC * view * vec3(aPos.xy, 1.0);
    gl_Position = vec4(pos.xy, 0.0, 1.0);
}