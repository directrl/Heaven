#version 330
//$include material.glsl

layout (location = 0) in vec3 position;
layout (location = 1) in vec2 texCoords;
layout (location = 2) in vec3 normals;
layout (location = 3) in mat4 instanceModel;
layout (location = 7) in vec4 instanceMaterial_color;
layout (location = 8) in int instanceMaterial_texLayer;

out vec2 outTexCoords;
flat out Material outInstanceMaterial;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

uniform int instanced;

void main() {
	if(instanced > 0) {
		gl_Position = projection * view * instanceModel * vec4(position.x, position.y, position.z, 1.0);
		outInstanceMaterial = Material(instanceMaterial_color, instanceMaterial_texLayer);
		outTexCoords = texCoords;
	} else {
		gl_Position = projection * view * model * vec4(position.x, position.y, position.z, 1.0);
		outTexCoords = texCoords;
	}
}