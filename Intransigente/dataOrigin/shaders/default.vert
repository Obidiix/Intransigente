#version 330 core
layout (location = 0) in lowp vec3 aPos;
layout (location = 1) in lowp vec3 aNormal;
layout (location = 2) in lowp vec2 aTexCoords;

//In order to calculate some basic lighting we need a few things per model basis, and a few things per fragment basis:
uniform lowp vec3 objectColor; //The color of the object.
uniform lowp vec3 lightColor; //The color of the light.
uniform lowp vec3 ambientColor;
uniform lowp vec3 lightPos; //The position of the light.

uniform lowp mat4 model;
uniform lowp mat4 view;
uniform lowp mat4 projection;

out lowp vec3 LightResult;
out lowp vec2 TexCoord;

void main()
{
    gl_Position = vec4(aPos, 1.0) * model * view * projection;
    vec3 FragPos = vec3(vec4(aPos, 1.0) * model);
    TexCoord = aTexCoords;
    vec3 Normal = aNormal * mat3(transpose(inverse(model)));

    float ambientStrength = 1;
    vec3 ambient = ambientStrength * ambientColor;

    //We calculate the light direction, and make sure the normal is normalized.
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(lightPos - FragPos); //Note: The light is pointing from the light to the fragment

    //The diffuse part of the phong model.
    //This is the part of the light that gives the most, it is the color of the object where it is hit by light.
    float diff = max(dot(norm, lightDir), 0.0); //We make sure the value is non negative with the max function.
    vec3 diffuse = diff * lightColor;

    //At last we add all the light components together and multiply with the color of the object. Then we set the color
    //and makes sure the alpha value is 1
    LightResult = (ambient + diffuse) * objectColor;
}