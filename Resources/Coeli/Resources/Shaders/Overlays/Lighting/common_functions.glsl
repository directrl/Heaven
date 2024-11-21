vec4 calc_light_ambient() {
	return u_scene_env.ambient_color;
}

vec4 calc_light_diffuse(vec4 light_diffuse, Material mat,
						vec3 normal, vec3 light_dir) {

	float diff = max(dot(normal, light_dir), 0.0);
	vec4 diffuse = light_diffuse * diff;
	
	return diffuse;
}

vec4 calc_light_specular(vec4 light_specular, Material mat,
						 vec3 view_dir, vec3 reflect_dir,
						 vec2 tex_coords) {

	float spec = pow(max(dot(view_dir, reflect_dir), 0.0), mat.shininess * 256);
	vec4 specular = light_specular * spec * texture(u_material_tex_specular, tex_coords);
	
	return specular;
}

vec4 calc_light_directional(DirectionalLight light, Material mat, vec3 normal, vec3 view_dir, vec2 tex_coords) {
	vec3 light_dir = normalize(-light.direction);
	vec3 reflect_dir = reflect(-light_dir, normal);

	vec4 diffuse = calc_light_diffuse(light.diffuse, mat, normal, light_dir);
	vec4 specular = calc_light_specular(light.specular, mat, view_dir, reflect_dir, tex_coords);
	
	return diffuse + specular;
}

vec4 calc_light_point(PointLight light, Material mat, vec3 frag_pos, vec3 normal, vec3 view_dir, vec2 tex_coords) {
	vec3 light_dir = normalize(light.position - frag_pos);
	vec3 reflect_dir = reflect(-light_dir, normal);
	
	vec4 diffuse = calc_light_diffuse(light.diffuse, mat, normal, light_dir);
	vec4 specular = calc_light_specular(light.specular, mat, view_dir, reflect_dir, tex_coords);

	float distance  = length(light.position - frag_pos);
	float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));

	diffuse *= attenuation;
	specular *= attenuation;

	return diffuse + specular;
}

vec4 calc_light_spot(SpotLight light, Material mat, vec3 frag_pos, vec3 normal, vec3 view_dir, vec2 tex_coords) {
	vec3 light_dir = normalize(light.position - frag_pos);
	
	float theta = dot(light_dir, normalize(-light.direction));
	
	if(theta > light.cutoff) {
		vec3 reflect_dir = reflect(-light_dir, normal);

		vec4 ambient = calc_light_ambient();
		vec4 diffuse = calc_light_diffuse(light.diffuse, mat, normal, light_dir);
		vec4 specular = calc_light_specular(light.specular, mat, view_dir, reflect_dir, tex_coords);

		float distance  = length(light.position - frag_pos);
		float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));

		diffuse *= attenuation;
		specular *= attenuation;

		float epsilon = (light.cutoff + light.outer_cutoff) - light.cutoff;
		float intensity = clamp((theta - (light.cutoff + light.outer_cutoff)) / epsilon, 0.0, 1.0);
		
		ambient *= intensity;
		diffuse *= intensity;
		specular *= intensity;

		return ambient + diffuse + specular;
	} else {
		return vec4(0.0, 0.0, 0.0, 1.0);
	}
}