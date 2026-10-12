#version 330 core
out vec4 FragColor;

uniform vec4 color;
uniform float transparent;
in vec2 TexCoords;

uniform sampler2D _mainTex;

void main()
{ 
    vec4 texColor = texture(_mainTex, TexCoords);
    if(transparent != 0 && texColor.a < 0.1)
        discard;
    FragColor = texColor * color;
}