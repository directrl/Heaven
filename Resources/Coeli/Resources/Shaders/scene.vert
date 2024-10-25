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
	position *= u_projection * u_view * u_model * vec4(vertex_pos.x, vertex_pos.y, vertex_pos.z, 1.0);
	//$overlay_call POSITION_POST
	
	gl_Position = position;
	vert_out.tex_coords = vertex_tex;
}