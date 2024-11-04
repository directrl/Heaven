#version 330 core

out vec4 out_frag_color;
in vec2 out_tex_coords;

uniform sampler2D tex;

void main() {
	out_frag_color = texture(tex, out_tex_coords);
}