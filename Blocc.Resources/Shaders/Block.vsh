#version 420 core
layout (location = 0) in vec3 inPos;
layout (location = 1) in float inIndex;
layout (location = 2) in float inTexture;
layout (location = 3) in vec3 inOverlayColor;
layout (location = 4) in float inOverlayTexture;

out vec2 TexCoords;
out float TexId;
out vec3 OverlayTexColor;
out float OverlayTexId;

uniform mat4 Projection;
uniform mat4 View;

vec2 TextureCoordinates[4] = vec2[4](
    vec2(1, 1),
    vec2(0, 1),
    vec2(0, 0),
    vec2(1, 0)
);

void main()
{
    TexCoords = TextureCoordinates[int(inIndex)];
    TexId = inTexture;
    OverlayTexColor = inOverlayColor;
    OverlayTexId = inOverlayTexture;

    gl_Position = Projection * View * vec4(inPos, 1.0);
}