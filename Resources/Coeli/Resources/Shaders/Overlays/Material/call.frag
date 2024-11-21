if(u_material.has_textures) {
	final_color *= texture(u_material_tex_diffuse, vert_in.tex_coords);
}

final_color *= u_material.albedo;