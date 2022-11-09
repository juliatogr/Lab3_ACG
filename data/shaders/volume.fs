
uniform vec3 u_camera_position;
uniform vec4 u_color;
varying vec3 v_pixel_position;

void main()
{
	vec3 ray_dir = u_camera_position - v_pixel_position;
	vec3 ray_dir = normalize(ray_dir);

	float ray_step = 0.02;


	gl_FragColor = u_color;
}
