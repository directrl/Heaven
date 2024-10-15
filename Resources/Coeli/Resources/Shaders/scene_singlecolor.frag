#version 330

struct Material {
	vec4 color;
};

in vec2 outTexCoords;
in Material outInstanceMaterial;

out vec4 fragColor;

uniform Material material;
uniform sampler2D texSampler;

uniform int instanced;

void main() {
	if(instanced > 0) {
		fragColor = outInstanceMaterial.color;
	} else {
		if(outTexCoords.x > 0 || outTexCoords.y > 0) {
			fragColor = material.color * texture(texSampler, outTexCoords);
		} else {
			fragColor = material.color;
		}
	}
}