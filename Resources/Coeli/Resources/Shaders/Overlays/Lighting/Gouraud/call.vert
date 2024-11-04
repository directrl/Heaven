if(!u_current_light) {
	vec3 normal = vert_out.normal;
	vec3 view_dir = normalize(u_camera_pos - vert_out.frag_pos);
	vec2 tex_coords = vert_out.tex_coords;

	vec4 light_result;

	for(int i = 0; i < u_directional_light_count; i++) {
		light_result += calc_light_directional(u_directional_lights[i], u_material, normal, view_dir, tex_coords);
	}

	for(int i = 0; i < u_spot_light_count; i++) {
		light_result += calc_light_spot(u_spot_lights[i], u_material, vert_out.frag_pos, normal, view_dir, tex_coords);
	}

	for(int i = 0; i < u_point_light_count; i++) {
		light_result += calc_light_point(u_point_lights[i], u_material, vert_out.frag_pos, normal, view_dir, tex_coords);
	}

	vert_out.color *= light_result;
}