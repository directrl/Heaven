#version 330

struct Material {
	vec4 color;
};

in Material outInstanceMaterial;

out vec4 fragColor;

uniform Material material;
uniform int instanced;

void main() {
	if(instanced > 0) {
		fragColor = outInstanceMaterial.color;
	} else {
		fragColor = material.color;
	}
}