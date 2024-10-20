if(vert_in.tex_coords.x > 0 || vert_in.tex_coords.y > 0) {
	tex_color = texture(u_tex2d_sampler, vert_in.tex_coords);
}