#version 330

struct Material {
	vec4 color;
};

out vec4 fragColor;

uniform Material material;

void main() {
	fragColor = material.color;
}