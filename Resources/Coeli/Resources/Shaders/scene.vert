#version 330
//$include Structures.vertex.glsl

//$include Uniforms.camera_matrices.glsl
//$include Uniforms.model_matrices.glsl

out VERT_OUT
//$include Interfaces.scene.vert.out
vert_out;

//$overlay_headers

void main() {
	vec4 position = vec4(1.0, 1.0, 1.0, 1.0);
	
	//$overlay_call POSITION_PRE
	position *= u_projection * u_view * u_model * vec4(vertex_pos, 1.0);
	//$overlay_call POSITION_POST
	
	gl_Position = position;
	vert_out.tex_coords = vertex_tex;
	vert_out.normal = mat3(transpose(inverse(u_model))) * vertex_norm; // TODO do this on the CPU
	vert_out.frag_pos = vec3(u_model * vec4(vertex_pos, 1.0));

	//$overlay_call RETURN
}