#version 330

layout (location = 0) in vec3 position;
layout (location = 3) in mat4 instanceModel;
layout (location = 7) in vec4 instanceColor;

out vec4 outInstanceColor;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

uniform int instanced;

void main() {
	if(instanced != 1) {
		gl_Position = projection * view * instanceModel * vec4(position.x, position.y, position.z, 1.0);
		outInstanceColor = instanceColor;
	} else {
		gl_Position = projection * view * model * vec4(position.x, position.y, position.z, 1.0);
	}
}