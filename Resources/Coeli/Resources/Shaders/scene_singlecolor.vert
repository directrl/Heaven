#version 330

struct Material {
	vec4 color;
};

layout (location = 0) in vec3 position;
layout (location = 1) in vec2 texCoords;
layout (location = 2) in vec3 normals;
layout (location = 3) in mat4 instanceModel;
layout (location = 7) in vec4 instanceMaterial_color;

out vec2 outTexCoords;
out Material outInstanceMaterial;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

uniform int instanced;

void main() {
	if(instanced > 0) {
		gl_Position = projection * view * instanceModel * vec4(position.x, position.y, position.z, 1.0);
		outInstanceMaterial = Material(instanceMaterial_color);
	} else {
		gl_Position = projection * view * model * vec4(position.x, position.y, position.z, 1.0);
		outTexCoords = texCoords;
	}
}