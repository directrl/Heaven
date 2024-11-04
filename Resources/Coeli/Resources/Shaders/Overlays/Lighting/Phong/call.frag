if(!u_light.current) {
	vec3 normal = vert_in.normal;
	vec3 view_dir = normalize(u_camera_pos - vert_in.frag_pos);
	vec2 tex_coords = vert_in.tex_coords;
	
	if(u_light.type == LIGHT_DIRECTIONAL) {
		final_color *= vec4(calc_light_directional(u_light, u_material, normal, view_dir, tex_coords), 1);
	} else if(u_light.type == LIGHT_POINT) {
		final_color *= vec4(calc_light_point(u_light, u_material, vert_in.frag_pos, normal, view_dir, tex_coords), 1);
	} else if(u_light.type == LIGHT_SPOT) {
		final_color *= vec4(calc_light_spot(u_light, u_material, vert_in.frag_pos, normal, view_dir, tex_coords), 1);
	}
}