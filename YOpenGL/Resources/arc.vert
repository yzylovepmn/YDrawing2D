#version 330 core
layout (location = 0) in vec2 aPos;
layout (location = 1) in vec3 aParams;

layout (std140) uniform Matrices
{
    mat3 worldToNDC;
    mat3 view;
};

out vec3 arcParams;
out float factor;

void main()
{
    arcParams = aParams;
    factor = view[0][0];
    vec3 pos = worldToNDC * view * vec3(aPos.xy, 1.0);
    gl_Position = vec4(pos.xy, 0.0, 1.0);
}