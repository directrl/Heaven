struct Material {
	vec4 albedo;
	vec4 ambient_color;
	vec4 diffuse_color;
	vec4 specular_color;
	
	float shininess;
	float reflectivity;
	
	bool has_textures;
};

uniform sampler2D u_material_tex_diffuse;
uniform sampler2D u_material_tex_specular;
uniform sampler2D u_material_tex_normal;
uniform sampler2D u_material_tex_height;