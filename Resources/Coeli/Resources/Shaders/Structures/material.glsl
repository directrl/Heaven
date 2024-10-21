struct Material {
	vec4 albedo;
	vec4 ambient_color;
	vec4 diffuse_color;
	vec4 specular_color;

	sampler2D tex_diffuse;
	sampler2D tex_specular;
	sampler2D tex_normal;
	sampler2D tex_height;
};