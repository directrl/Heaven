if(u_instance) {
	position = u_projection * u_view * instance_model * vec4(vertex_pos.xyz, 1.0);
	out_instance_material = Material(instance_color, instance_tex_layer);
}