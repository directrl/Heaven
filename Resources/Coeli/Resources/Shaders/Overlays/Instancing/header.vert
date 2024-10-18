layout (location = 10) in mat4 instance_model;
layout (location = 14) in vec4 instance_color; 
layout (location = 15) in int instance_tex_layer;

uniform bool u_instance;
flat out Material out_instance_material;