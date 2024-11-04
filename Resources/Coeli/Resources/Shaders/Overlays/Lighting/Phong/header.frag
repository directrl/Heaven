//$include Structures.light.glsl

uniform DirectionalLight u_directional_lights[MAX_DIRECTIONAL_LIGHTS];

uniform PointLight u_point_lights[MAX_LIGHTS];
uniform SpotLight u_spot_lights[MAX_LIGHTS];

uniform bool u_current_light;

uniform int u_directional_light_count;
uniform int u_point_light_count;
uniform int u_spot_light_count;

//$include Overlays.Lighting.common.glsl
