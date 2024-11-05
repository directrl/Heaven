layout(std140) uniform CameraMatrices {
	mat4 u_projection;
	mat4 u_view;
	vec3 u_camera_pos;
};