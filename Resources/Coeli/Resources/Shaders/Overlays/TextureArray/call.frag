if(u_material.tex_layer >= 0) {
	tex_color = texture(u_texArray_sampler, vec3(vert_in.tex_coords.xy, u_material.tex_layer));
}