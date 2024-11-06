struct DirectionalLight {
	
	vec4 diffuse;
	vec4 specular;
	
	vec3 direction;
};

struct PointLight {
	
	vec4 diffuse;
	vec4 specular;
	
	vec3 position;
	
	float constant;
	float linear;
	float quadratic;
};

struct SpotLight {

	vec4 diffuse;
	vec4 specular;

	vec3 direction;
	vec3 position;
	
	float constant;
	float linear;
	float quadratic;
	
	float cutoff;
	float outer_cutoff;
};