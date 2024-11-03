const int LIGHT_DIRECTIONAL = 0;
const int LIGHT_POINT = 1;
const int LIGHT_SPOT = 2;

struct Light {
	int type;
	bool current;
	
	vec3 ambient;
	vec3 diffuse;
	vec3 specular;
	
	float specular_strength;
	
	vec3 direction;
	vec3 position;
	
	// point light
	float constant;
	float linear;
	float quadratic;
	
	// spot light
	float cutoff;
};