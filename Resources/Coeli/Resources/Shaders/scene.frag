#version 330
//$preprocessor-defines

out vec4 out_frag_color;

//$include Uniforms.camera_matrices.glsl
//$include Uniforms.model_matrices.glsl

in VERT_OUT
//$include Interfaces.scene.vert.out
vert_in;

//$overlay_headers

void main() {
	vec4 final_color = vert_in.color;
	
	//$overlay_call COLOR_PRE
	//$overlay_call COLOR_PRE_STAGE2
	
	if(final_color.a < 0.01) {
		discard;
	}
	
	//$overlay_call COLOR_POST
	
	out_frag_color = final_color;
}