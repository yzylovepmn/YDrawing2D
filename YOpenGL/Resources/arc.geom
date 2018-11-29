#version 330 core
layout(points) in;
layout(line_strip, max_vertices=65) out;

uniform vec2 screenSize;
uniform vec2 samples[65];
uniform bool dashed;

in vec3 arcParams[];
in float factor[];

out float texCoord;

vec4 getpoint(vec2 p, float xfactor, float yfactor);
bool genscanpoint(float startRadian, float curRadian, int index, float xfactor, float yfactor);
float emitpoint(vec4 p1, vec4 p2, float _texCoord, bool emitp1);

void main()
{
  float xfactor = 2 * factor[0] / screenSize.x;
  float yfactor = 2 * factor[0] / screenSize.y;
  int i;
  bool isCicle = arcParams[0].y == arcParams[0].z;
  if (isCicle)
  {
    if (dashed)
    {
      vec4 p1, p2;
      float _texCoord = 0.0;
      p1 = getpoint(samples[0], xfactor, yfactor);
      for (i = 1; i < 65; i++)
      {
        p2 = getpoint(samples[i], xfactor, yfactor);
        vec2 winPos0 = screenSize.xy * p1.xy;
        vec2 winPos1 = screenSize.xy * p2.xy;
        if (i == 1)
        {
          gl_Position = p1;
          texCoord = _texCoord;
          EmitVertex();
        }
        gl_Position = p2;
        _texCoord = _texCoord + length(winPos1-winPos0) / 16.0;
        texCoord = _texCoord;
        EmitVertex();
        p1 = p2;
      }
    }
    else
    {
      for (i = 0; i < 65; i++)
      {
        gl_Position = getpoint(samples[i], xfactor, yfactor);
        EmitVertex();
      }
    }
  }
  else
  {
    float pi = 3.1415926535897931;
    float deltaRadian = pi / 32;
    float curRadian = 0;
    float startRadian = arcParams[0].y;
    float endRadian = arcParams[0].z;
    bool flag = false, isAfter = startRadian > endRadian, needRestart = endRadian > 0;
    if (!isAfter)
    {
      endRadian -= 2 * pi;
      curRadian -= 2 * pi;
    }

    int i;
    if(dashed)
    {
      bool assignedFirst = false, hasEmitFirst = false;
      vec4 p1, p2;
      float _texCoord = 0.0;
      for (i = 0; i < 65;)
      {
        if (!flag)
        {
          if (endRadian <= curRadian)
          {
            flag = true;
            if (endRadian < curRadian)
            {
              vec2 p = vec2(cos(endRadian), sin(endRadian));
              p1 = getpoint(p, xfactor, yfactor);
              assignedFirst = true;
            }
            if (curRadian < startRadian)
            {
              if (!assignedFirst)
                p1 = getpoint(samples[i], xfactor, yfactor);
              else
              {
                p2 = getpoint(samples[i], xfactor, yfactor);
                _texCoord = emitpoint(p1, p2, _texCoord, !hasEmitFirst);
                p1 = p2;
                hasEmitFirst = true;
              }
            }
            else if (curRadian >= startRadian)
            {
              if (assignedFirst)
              {
                p2 = getpoint(vec2(cos(startRadian), sin(startRadian)), xfactor, yfactor);
                _texCoord = emitpoint(p1, p2, _texCoord, !hasEmitFirst);
                p1 = p2;
                hasEmitFirst = true;
              }
              break;
            }
          }
        }
        else
        {
          if (curRadian < startRadian)
          {
            p2 = getpoint(samples[i], xfactor, yfactor);
            _texCoord = emitpoint(p1, p2, _texCoord, !hasEmitFirst);
            p1 = p2;
            hasEmitFirst = true;
          }
          else if (curRadian >= startRadian)
          {
            p2 = getpoint(vec2(cos(startRadian), sin(startRadian)), xfactor, yfactor);
            _texCoord = emitpoint(p1, p2, _texCoord, false);
            break;
          }
        }
        curRadian += deltaRadian;
        i++;
        if (i == 65 && needRestart)
          i = 1;
      }
    }
    else
    {
      for (i = 0; i < 65;)
      {
        if (!flag)
        {
          if (endRadian <= curRadian)
          {
            flag = true;
            if (endRadian < curRadian)
            {
              vec2 p = vec2(cos(endRadian), sin(endRadian));
              gl_Position = getpoint(p, xfactor, yfactor);
              EmitVertex();
            }
            if (genscanpoint(startRadian, curRadian, i, xfactor, yfactor))
              break;
          }
        }
        else
        {
          if (genscanpoint(startRadian, curRadian, i, xfactor, yfactor))
            break;
        }
        curRadian += deltaRadian;
        i++;
        if (i == 65 && needRestart)
          i = 1;
      }
    }
  }
  EndPrimitive();
}

vec4 getpoint(vec2 p, float xfactor, float yfactor)
{
  return vec4(gl_in[0].gl_Position.x + p.x * xfactor * arcParams[0].x, gl_in[0].gl_Position.y + p.y * yfactor * arcParams[0].x, 0.0, 1.0);
}

bool genscanpoint(float startRadian, float curRadian, int index, float xfactor, float yfactor)
{
  if (curRadian < startRadian)
  {
    gl_Position = getpoint(samples[index], xfactor, yfactor);
    EmitVertex();
  }
  else if (curRadian >= startRadian)
  {
    vec2 p = vec2(cos(startRadian), sin(startRadian));
    gl_Position = getpoint(p, xfactor, yfactor);
    EmitVertex();
    return true;
  }
  return false;
}

// Returns the latest texture coordinates
float emitpoint(vec4 p1, vec4 p2, float _texCoord, bool emitp1)
{
  vec2 winPos0 = screenSize.xy * p1.xy;
  vec2 winPos1 = screenSize.xy * p2.xy;
  if (emitp1)
  {
    gl_Position = p1;
    texCoord = _texCoord;
    EmitVertex();
  }
  _texCoord = _texCoord + length(winPos1-winPos0) / 16.0;
  gl_Position = p2;
  texCoord = _texCoord;
  EmitVertex();
  return _texCoord;
}