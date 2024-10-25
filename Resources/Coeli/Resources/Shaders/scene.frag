#version 330

out vec4 out_frag_color;

in VERT_OUT
//$include Interfaces.scene.vert.out
vert_in;

//$overlay_headers

void main() {
	vec4 final_color = vec4(1.0, 1.0, 1.0, 1.0);
	
	//$overlay_call COLOR_PRE
	
	if(final_color.a < 0.01) {
		discard;
	}

	//$overlay_call COLOR_POST
	
	out_frag_color = final_color;
}