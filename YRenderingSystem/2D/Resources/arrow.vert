#version 330 core
layout (location = 0) in vec2 aPos;
layout (location = 1) in vec4 aParams;

layout (std140) uniform Matrices
{
    mat3 worldToNDC;
    mat3 view;
};

out vec2 arrowP1;
out vec2 arrowP2;

void main()
{
    vec2 arrowDir = aParams.zw;
    vec3 pos = worldToNDC * view * vec3(aPos.xy, 1.0);

    vec2 origin = pos.xy;
    vec2 p1 = origin + arrowDir * aParams.x;
    vec2 hd = vec2(arrowDir.y, -arrowDir.x);
    vec2 wv = hd * aParams.y / 2;
    vec2 p2 = p1 + wv;
    p1 -= wv;
    vec2 v1 = (worldToNDC * vec3(p1 - origin, 0.0)).xy;
    vec2 v2 = (worldToNDC * vec3(p2 - origin, 0.0)).xy;
    arrowP1 = origin + v1;
    arrowP2 = origin + v2;

    gl_Position = vec4(pos.xy, 0.0, 1.0);
}