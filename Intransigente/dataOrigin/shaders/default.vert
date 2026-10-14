#version 330 core
layout (location = 0) in lowp vec3 aPos;
layout (location = 1) in lowp vec3 aNormal;
layout (location = 2) in lowp vec2 aTexCoords;

uniform lowp mat4 model;
uniform lowp mat4 view;
uniform lowp mat4 projection;

out lowp vec3 FragPos;
out lowp vec3 Normal;
out lowp vec2 TexCoord;

void main()
{
    FragPos = vec3(vec4(aPos, 1.0) * model);
    Normal =  aNormal * mat3(transpose(inverse(model)));
    TexCoord = aTexCoords;
    gl_Position = vec4(aPos, 1.0) * model * view * projection;
}