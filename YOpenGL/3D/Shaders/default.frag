#version 330 core
#define MAX_NUM_LIGHTS 10

out vec4 FragColor;

struct Material {
    vec4 emissive;
    vec4 diffuse;
    vec4 specular;
    float shininess;
    bool hasEmissive;
    bool hasDiffuse;
    bool hasSpecular;
};

// size 4 * 4
struct AmbientLight {
    vec4 ambient;
};

// size 12 * 4
struct DirLight {
    vec3 direction;

    vec4 diffuse;
    vec4 specular;
};

// size 16 * 4
struct PointLight {
    vec3 position;
    vec4 diffuse;
    vec4 specular;

    float constant;
    float linear;
    float quadratic;
    float range;
};

// size 24 * 4
struct SpotLight {
    vec3 position;
    vec3 direction;
    vec4 diffuse;
    vec4 specular;

    float constant;
    float linear;
    float quadratic;
    float range;

    float cutOff;
    float outerCutOff;
};

layout (std140) uniform Lights
{
    AmbientLight ambientLights[MAX_NUM_LIGHTS];
    DirLight dirLights[MAX_NUM_LIGHTS];
    PointLight pointLights[MAX_NUM_LIGHTS];
    SpotLight spotLights[MAX_NUM_LIGHTS];
    int ambientLightCount;
    int dirLightCount;
    int pointLightCount;
    int spotLightCount;
};

in vec3 fragPos;
in vec3 normal;
in vec2 texCoords;
uniform vec3 viewPos;
uniform Material material;
uniform Material materialBack;

vec3 CalcAmbientLight(AmbientLight light, Material material);
vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir, Material material, float a);
vec3 CalcPointLight(PointLight light, vec3 normal, vec3 viewDir, Material material, float a);
vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 viewDir, Material material, float a);

void main()
{
    Material mat = gl_FrontFacing ? material : materialBack;
    vec3 norm = normalize(normal);
    vec3 viewDir = normalize(viewPos - fragPos);

    vec3 ret = vec3(0);
    float a = mat.hasDiffuse ? mat.diffuse.a : (mat.hasEmissive ? mat.emissive.a : 1);

    if (mat.hasDiffuse)
    {
        for(int i = 0; i < ambientLightCount; i++)
            ret += CalcAmbientLight(ambientLights[i], mat);
    }
    for(int i = 0; i < dirLightCount; i++)
        ret += CalcDirLight(dirLights[i], norm, viewDir, mat, a);
    for(int i = 0; i < pointLightCount; i++)
        ret += CalcPointLight(pointLights[i], norm, viewDir, mat, a);
    for(int i = 0; i < spotLightCount; i++)
        ret += CalcSpotLight(spotLights[i], norm, viewDir, mat, a);
    if (mat.hasEmissive)
        ret += mat.emissive.rgb * (mat.hasDiffuse ? mat.emissive.a / a : 1);
    FragColor = vec4(ret.rgb, a);
}

vec3 CalcAmbientLight(AmbientLight light, Material material)
{
    return light.ambient.a * light.ambient.rgb * material.diffuse.rgb;
}

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir, Material material, float a)
{
    vec3 dirRet = vec3(0);
    vec3 lightDir = normalize(-light.direction);

    // Diffuse shading
    if (material.hasDiffuse)
    {
        float diff = max(dot(normal, lightDir), 0.0);
        vec3 diffuse = light.diffuse.a * light.diffuse.rgb * diff * material.diffuse.rgb;
        dirRet += diffuse;
    }

    // Specular shading
    if (material.hasSpecular)
    {
        vec3 reflectDir = reflect(-lightDir, normal);
        float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
        vec3 specular = light.specular.a * light.specular.rgb * spec * material.specular.rgb * material.specular.a / a;
        dirRet += specular;
    }

    return dirRet;
}

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 viewDir, Material material, float a)
{
    vec3 pointRet = vec3(0);
    vec3 lightDir = normalize(light.position - fragPos);
    float distance = length(lightDir);

    if (distance <= light.range)
    {
        // Attenuation
        float attenuation = 1.0f / (light.constant + light.linear * distance + light.quadratic * (distance * distance));

        // Diffuse shading
        if (material.hasDiffuse)
        {
            float diff = max(dot(normal, lightDir), 0.0);
            vec3 diffuse = light.diffuse.a * light.diffuse.rgb * diff * material.diffuse.rgb;
            pointRet += diffuse * attenuation;
        }

        // Specular shading
        if (material.hasSpecular)
        {
            vec3 reflectDir = reflect(-lightDir, normal);
            float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
            vec3 specular = light.specular.a * light.specular.rgb * spec * material.specular.rgb * material.specular.a / a;
            pointRet += specular * attenuation;
        }
    }

    return pointRet;
}

vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 viewDir, Material material, float a)
{
    vec3 spotRet = vec3(0);
    vec3 lightDir = normalize(light.position - fragPos);
    float distance = length(lightDir);

    if (distance <= light.range)
    {
        // Attenuation
        float attenuation = 1.0f / (light.constant + light.linear * distance + light.quadratic * (distance * distance));
        // Spotlight intensity
        float theta = dot(lightDir, normalize(-light.direction));
        float epsilon = light.cutOff - light.outerCutOff;
        float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);

        // Diffuse shading
        if (material.hasDiffuse)
        {
            float diff = max(dot(normal, lightDir), 0.0);
            vec3 diffuse = light.diffuse.a * light.diffuse.rgb * diff * material.diffuse.rgb;
            spotRet += diffuse * attenuation * intensity;
        }

        // Specular shading
        if (material.hasSpecular)
        {
            vec3 reflectDir = reflect(-lightDir, normal);
            float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
            vec3 specular = light.specular.a * light.specular.rgb * spec * material.specular.rgb * material.specular.a / a;
            spotRet += specular * attenuation * intensity;
        }
    }

    return spotRet;
}