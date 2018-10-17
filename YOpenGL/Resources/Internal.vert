layout (location = 0) in vec2 aPos;

uniform mat3 worldToNDC;
uniform mat3 view;
void main()
{
    vec3 pos = worldToNDC * view * vec3(aPos.xy, 1.0);
    gl_Position = vec4(pos.x, pos.y, 0.0, 1.0);
}