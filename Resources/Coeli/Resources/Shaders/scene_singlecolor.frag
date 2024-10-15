#version 330

in vec4 outInstanceColor;

out vec4 fragColor;

uniform vec4 color;
uniform int instanced;

void main() {
	if(instanced != 1) {
		fragColor = outInstanceColor;
	} else {
		fragColor = color;
	}
}