// TODO make these configurable
#define MAX_DIRECTIONAL_LIGHTS 4
#define MAX_LIGHTS 100

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

//struct Light {
//	int type;
//	bool current;
//	
//	vec3 ambient;
//	vec3 diffuse;
//	vec3 specular;
//	
//	float specular_strength;
//	
//	vec3 direction;
//	vec3 position;
//	
//	// point light
//	float constant;
//	float linear;
//	float quadratic;
//	
//	// spot light
//	float cutoff;
//	float outer_cutoff;
//};