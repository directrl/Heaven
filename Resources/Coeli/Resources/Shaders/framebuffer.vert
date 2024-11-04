#version 330 core
//$include Structures.vertex.glsl

out vec2 out_tex_coords;

void main() {
	gl_Position = vec4(vertex_pos.x, vertex_pos.y, 0.0, 1.0);
	out_tex_coords = vertex_tex;
}