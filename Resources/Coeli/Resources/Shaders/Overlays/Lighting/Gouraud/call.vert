vec3 norm = normalize(vert_out.normal);
vec3 light_dir = normalize(u_scene_env.light_pos - vert_out.frag_pos);
vec3 view_dir = normalize(u_camera_pos - vert_out.frag_pos);
vec3 reflect_dir = reflect(-light_dir, norm);

// diffuse
float diff = max(dot(norm, light_dir), 0.0);
vec3 diffuse = diff * u_scene_env.light_color;

// specular
float specular_strength = 1.5;
float spec = pow(max(dot(view_dir, reflect_dir), 0.0), u_material.shininess * 256);
vec3 specular = specular_strength * spec * u_scene_env.light_color;

gouraud_light_final = u_scene_env.ambient_light + diffuse + specular;