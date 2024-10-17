#version 330
//$include Structures.material.glsl
//$include Structures.vertex.glsl

out vec2 outTexCoords;

//$include Uniforms.model_matrices.glsl

void main() {
	gl_Position = projection * view * model * vec4(vertex_pos.x, vertex_pos.y, vertex_pos.z, 1.0);
	outTexCoords = vertex_tex;
}