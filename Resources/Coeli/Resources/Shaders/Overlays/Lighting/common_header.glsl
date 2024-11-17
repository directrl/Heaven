//$include Structures.light.glsl

layout(std140) uniform Lights {
	int u_directional_light_count;
	int u_point_light_count;
	int u_spot_light_count;

	DirectionalLight u_directional_lights[MAX_DIRECTIONAL_LIGHTS];
	PointLight u_point_lights[MAX_LIGHTS];
	SpotLight u_spot_lights[MAX_LIGHTS];
};