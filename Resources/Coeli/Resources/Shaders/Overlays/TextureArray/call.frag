if(overlay_textureArray > 0 && material.texLayer >= 0) {
	texColor = texture(texArraySampler, vec3(outTexCoords.xy, material.texLayer));
}