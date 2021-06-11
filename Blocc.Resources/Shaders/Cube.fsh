#version 420 core
out vec4 FragColor;

in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoords;

uniform sampler2D Texture0;

void main()
{
    FragColor = texture(Texture0, TexCoords);

    if (FragColor.a == 0)
    {
        discard;
    }
}