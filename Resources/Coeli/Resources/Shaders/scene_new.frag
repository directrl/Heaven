#version 330
//$include Structures.material.glsl

out vec4 out_frag_color;

uniform Material u_material;

in VERT_OUT
//$include Interfaces.scene.vert.out
vert_in;

//$overlay_headers

void main() {
	vec4 tex_color = vec4(1.0, 1.0, 1.0, 1.0);
	vec4 mat_color = vec4(1.0, 1.0, 1.0, 1.0);
	
	//$overlay_call COLOR_PRE
	mat_color = u_material.color;
	//$overlay_call COLOR_POST
	
	if(tex_color.a < 0.01) {
		discard;
	}
	
	out_frag_color = mat_color * tex_color;
}