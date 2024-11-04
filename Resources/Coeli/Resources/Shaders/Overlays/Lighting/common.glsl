vec3 calc_light_ambient() {
	return u_scene_env.ambient_light;
}

vec3 calc_light_diffuse(Light light, Material mat,
						vec3 norm, vec3 light_dir,
						vec2 tex_coords) {

	float diff = max(dot(norm, light_dir), 0.0);
	vec3 diffuse = light.diffuse * diff * texture(mat.tex_diffuse, tex_coords).rgb;
	
	return diffuse;
}

vec3 calc_light_specular(Light light, Material mat,
						 vec3 view_dir, vec3 reflect_dir,
						 vec2 tex_coords) {

	float spec = pow(max(dot(view_dir, reflect_dir), 0.0), mat.shininess * 256);
	vec3 specular = light.specular * light.specular_strength * spec * texture(mat.tex_specular, tex_coords).rgb;
	
	return specular;
}

vec3 calc_light_directional(Light light, Material mat, vec3 normal, vec3 view_dir, vec2 tex_coords) {
	vec3 norm = normalize(normal);
	vec3 light_dir = normalize(-light.direction);
	vec3 reflect_dir = reflect(-light_dir, norm);

	vec3 ambient = calc_light_ambient();
	vec3 diffuse = calc_light_diffuse(light, mat, norm, light_dir, tex_coords);
	vec3 specular = calc_light_specular(light, mat, view_dir, reflect_dir, tex_coords);
	
	return ambient + diffuse + specular;
}

vec3 calc_light_point(Light light, Material mat, vec3 frag_pos, vec3 normal, vec3 view_dir, vec2 tex_coords) {
	vec3 norm = normalize(normal);
	vec3 light_dir = normalize(light.position - frag_pos);
	vec3 reflect_dir = reflect(-light_dir, norm);
	
	vec3 ambient = calc_light_ambient();
	vec3 diffuse = calc_light_diffuse(light, mat, norm, light_dir, tex_coords);
	vec3 specular = calc_light_specular(light, mat, view_dir, reflect_dir, tex_coords);

	float distance  = length(light.position - frag_pos);
	float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));

	ambient *= attenuation;
	diffuse *= attenuation;
	specular *= attenuation;

	return ambient + diffuse + specular;
}

vec3 calc_light_spot(Light light, Material mat, vec3 frag_pos, vec3 normal, vec3 view_dir, vec2 tex_coords) {
	vec3 light_dir = normalize(light.position - frag_pos);
	float theta = dot(light_dir, normalize(-light.direction));
	
	if(theta > light.cutoff) {
		return calc_light_point(light, mat, frag_pos, normal, view_dir, tex_coords);
	} else {
		return calc_light_ambient();
	}
}