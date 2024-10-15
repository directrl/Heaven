#version 330

struct Material {
	vec4 color;
};

in vec4 outInstanceColor;

out vec4 fragColor;

uniform Material material;
uniform int instanced;

void main() {
	if(instanced != 1) {
		fragColor = outInstanceColor;
	} else {
		fragColor = material.color;
	}
}