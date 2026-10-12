#version 330 core
out vec4 FragColor;

uniform lowp sampler2D mainTex;

in lowp vec3 LightResult;
in lowp vec2 TexCoord;

void main()
{
    FragColor = vec4(LightResult, 1.0) * texture(mainTex, TexCoord);
}