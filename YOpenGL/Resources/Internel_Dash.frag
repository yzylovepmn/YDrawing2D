uniform sampler1D pattern;
uniform vec4 color;

noperspective in float texCoord;

out vec4 FragColor;

void main()
{
    if(texture(pattern, texCoord).r < 0.5)
        discard;
    FragColor = color;
}