if(vert_in.tex_coords.x > 0 || vert_in.tex_coords.y > 0) {
	final_color = texture(u_material.tex_diffuse, vert_in.tex_coords);
}

final_color *= u_material.albedo;