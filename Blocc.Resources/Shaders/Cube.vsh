#version 420 core
layout (location = 0) in vec3 inPos;
layout (location = 1) in vec3 inNormal;
layout (location = 2) in vec2 inTexCoords;

out vec3 FragPos;
out vec3 Normal;
out vec2 TexCoords;

uniform mat4 Model;
uniform mat4 Projection;
uniform mat4 View;

void main()
{
    FragPos = vec3(Model  * vec4(inPos, 1.0));
    Normal = mat3(transpose(inverse(Model))) * inNormal;
    TexCoords = inTexCoords;

    gl_Position = Projection * View * vec4(FragPos, 1.0);
}