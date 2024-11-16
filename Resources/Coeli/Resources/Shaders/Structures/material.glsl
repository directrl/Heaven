struct Material {
	vec4 albedo;
	vec4 ambient_color;
	vec4 diffuse_color;
	vec4 specular_color;
	
	float shininess;
	float reflectivity;
	
	bool has_textures;

	sampler2D tex_diffuse;
	sampler2D tex_specular;
	sampler2D tex_normal;
	sampler2D tex_height;
};