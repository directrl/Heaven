if(u_instance) {
	if(out_instance_material.tex_layer >= 0) {
		tex_color = texture(u_texArray_sampler, vec3(vert_in.tex_coords.xy, out_instance_material.tex_layer));
	}
	
	mat_color = out_instance_material.color;
}