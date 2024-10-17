#version 330
//$include Structures.material.glsl

in vec2 outTexCoords;

out vec4 fragColor;

uniform Material material;

//$include Overlays.Texture2D.header.frag
//$include Overlays.TextureArray.header.frag

void main() {
	vec4 texColor = vec4(1.0, 1.0, 1.0, 1.0);
	vec4 matColor = vec4(1.0, 1.0, 1.0, 1.0);
	
//$include Overlays.Texture2D.call.frag
//$include Overlays.TextureArray.call.frag

	matColor = material.color;
	
	if(texColor.a < 0.01) {
		discard;
	}
	
	fragColor = matColor * texColor;
}