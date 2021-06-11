#version 420 core
out vec4 FragColor;

in vec2 TexCoords;
in float TexId;
in float OverlayTexId;
in vec3 OverlayTexColor;

uniform sampler2DArray TextureArray;

void main()
{
    vec4 tex = texture(TextureArray, vec3(TexCoords, TexId));

    FragColor = tex;

    if (OverlayTexId >= 0)
    {
        vec4 overlay = texture(TextureArray, vec3(TexCoords, OverlayTexId)) * vec4(OverlayTexColor, 1);

        if (overlay.a > 0)
        {
            FragColor = mix(tex, overlay, overlay.a);
        }
    }

    if (FragColor.a == 0)
    {
        discard;
    }
}