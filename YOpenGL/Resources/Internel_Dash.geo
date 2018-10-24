layout(lines) in;
layout(line_strip, max_vertices=2) out;

uniform vec2 screenSize;
uniform float patternSize;

noperspective out float texCoord;

void main()
{
    vec2 winPos0 = screenSize.xy * gl_in[0].gl_Position.xy / gl_in[0].gl_Position.w;
    vec2 winPos1 = screenSize.xy * gl_in[1].gl_Position.xy / gl_in[1].gl_Position.w;
    gl_Position = gl_in[0].gl_Position;
    texCoord = 0.0;
    EmitVertex();
    gl_Position = gl_in[1].gl_Position;
    texCoord = 0.5 * length(winPos1-winPos0) / patternSize;
    EmitVertex();
}