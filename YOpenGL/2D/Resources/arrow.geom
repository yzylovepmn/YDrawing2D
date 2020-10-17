#version 330 core
layout(points) in;
layout(triangle_strip, max_vertices=3) out;

in vec2 arrowP1[];
in vec2 arrowP2[];

void main()
{
  vec2 origin = gl_in[0].gl_Position.xy;

  gl_Position = vec4(origin.xy, 0.0, 1.0);
  EmitVertex();
  gl_Position = vec4(arrowP2[0].xy, 0.0, 1.0);
  EmitVertex();
  gl_Position = vec4(arrowP1[0].xy, 0.0, 1.0);
  EmitVertex();

  EndPrimitive();
}