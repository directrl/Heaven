#version 330
//$include Structures.material.glsl

in vec2 outTexCoords;
flat in Material outInstanceMaterial;

out vec4 fragColor;

uniform Material material;
/*layout (binding = 0) */uniform sampler2D texSampler;
/*layout (binding = 1) */uniform sampler2DArray texArraySampler;

uniform int instanced;

void main() {
	vec4 texColor = vec4(1.0, 1.0, 1.0, 1.0);
	vec4 matColor = vec4(1.0, 1.0, 1.0, 1.0);
	
	if(instanced > 0) {
		if(outInstanceMaterial.texLayer >= 0) {
			texColor = texture(texArraySampler, vec3(outTexCoords.xy, outInstanceMaterial.texLayer));
		}
		
		matColor = outInstanceMaterial.color;
	} else {
		if(outTexCoords.x > 0 || outTexCoords.y > 0) {
			if(material.texLayer >= 0) {
				texColor = texture(texArraySampler, vec3(outTexCoords.xy, material.texLayer));
			} else {
				texColor = texture(texSampler, outTexCoords);
			}
		}
		
		matColor = material.color;
	}
	
	if(texColor.a < 0.01) {
		discard;
	}
	
	fragColor = matColor * texColor;
}