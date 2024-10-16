#version 330

flat struct Material {
	vec4 color;
	int texLayer;
};

in vec2 outTexCoords;
flat in Material outInstanceMaterial;

out vec4 fragColor;

uniform Material material;
/*layout (binding = 0) */uniform sampler2D texSampler;
/*layout (binding = 1) */uniform sampler2DArray texArraySampler;

uniform int instanced;

void main() {
	if(instanced > 0) {
		if(outInstanceMaterial.texLayer >= 0) {
			fragColor = outInstanceMaterial.color
				* texture(texArraySampler, vec3(outTexCoords.xy, outInstanceMaterial.texLayer));
		} else {
			fragColor = outInstanceMaterial.color;
		}
	} else {
		if(outTexCoords.x > 0 || outTexCoords.y > 0) {
			if(material.texLayer >= 0) {
				fragColor = material.color
					* texture(texArraySampler, vec3(outTexCoords.xy, material.texLayer));
			} else {
				fragColor = material.color
					* texture(texSampler, outTexCoords);
			}
		} else {
			fragColor = material.color;
		}
	}
}