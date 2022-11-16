varying vec3 v_position;
varying vec3 v_world_position;
varying vec3 v_normal;
varying vec2 v_uv;
varying vec4 v_color;

uniform vec3 u_camera_pos;
uniform sampler3D u_texture;

void main()
{

	//Ray Setup
	vec3 ray_dir = normalize(v_position - u_camera_pos);

	float stepLength = 0.001;
	vec4 finalColor = vec4(0.0);
	vec3 sample_pos = v_position;

	for (int i = 0; i < 1000; i++){


		// volume sampling
		vec3 text_coord = (sample_pos + 1) /2;
		float d = texture(u_texture, text_coord).x;

		//classification

		vec4 sampleColor = vec4(d,d,d,d);

		sampleColor.rgb *= sampleColor.a;

		// Composition
		finalColor += stepLength * (1.0 - finalColor.a) * sampleColor;

		//Next sample & early termination

		sample_pos += stepLength * ray_dir;

		if (sample_pos.x > 1.0 || sample_pos.x < -1.0 || sample_pos.y > 1.0 || sample_pos.y < -1.0 || sample_pos.y > 1.0 || sample_pos.y < -1.0){
			break;
		}
		
		if (finalColor.a >= 1.0) {
			break;
		}
		//float brightness = 0.2;
		//finalColor.x *= brightness; 
		//finalColor.y *= brightness; 
		//finalColor.z *= brightness; 
	}

	if (finalColor.a <= 0.01){
		discard;
	} else {
		gl_FragColor = finalColor;
	}


}
