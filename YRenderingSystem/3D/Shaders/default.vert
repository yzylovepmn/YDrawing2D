#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;
layout (location = 3) in float aDistance;

layout (std140) uniform Matrices
{
    mat4 view;
    mat4 projection;
    mat4 vp;
};

out vec3 fragPos;
out vec3 normal;
out vec2 texCoords;
out float distance;
uniform bool dashed;
uniform float dashedFactor;

void main()
{
    fragPos = aPos;
    normal = aNormal;
    texCoords = aTexCoords;
    distance = aDistance / dashedFactor;
    vec4 pos = vp * vec4(aPos, 1.0);
    if (dashed)
        gl_Position = pos / pos.w;
    else gl_Position = pos;
}