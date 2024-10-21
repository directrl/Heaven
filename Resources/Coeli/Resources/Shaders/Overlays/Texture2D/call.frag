if(vert_in.tex_coords.x > 0 || vert_in.tex_coords.y > 0) {
	tex_color = texture(u_tex_diffuse_0, vert_in.tex_coords);
	tex_color *= texture(u_tex_diffuse_1, vert_in.tex_coords);
	tex_color *= texture(u_tex_diffuse_2, vert_in.tex_coords);
	tex_color *= texture(u_tex_diffuse_3, vert_in.tex_coords);
}