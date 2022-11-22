varying vec3 v_position;
varying vec3 v_world_position;
varying vec3 v_normal;
varying vec2 v_uv;
varying vec4 v_color;

uniform vec3 u_camera_pos;
uniform sampler3D u_texture;
uniform sampler2D u_noise;
uniform int u_noisewidth;
uniform float u_brightness;
uniform float u_steplength;
uniform vec4 u_color;


float rand(vec2 co){
	return fract(sin(dot(co, vec2(12.9898, 78.233))) * 43758.5453);
}

void main()
{

	//Ray Setup
	vec3 ray_dir = normalize(v_position - u_camera_pos);
	vec4 finalColor = vec4(0.0);

	//vec2 noise_pos = gl_FragCoord.xy / u_noisewidth;
	//float offset = texture(u_noise, noise_pos).x; //first approach
	//float offset = rand(gl_FragColor.xy);		 // second approach
	vec3 sample_pos = v_position;
	//sample_pos.x += offset;

	for (int i = 0; i < 10000; i++){

		

		
		// volume sampling
		vec3 text_coord = (sample_pos + 1) /2;
		float d = texture(u_texture, text_coord).x;

		//classification

		vec4 sampleColor;

		sampleColor = vec4(d, d, d, d);
		sampleColor.rgb *= sampleColor.a;
		sampleColor.rgb *= u_color.rgb;
		sampleColor.rgb *= u_brightness;


		// Composition
		finalColor += u_steplength * (1.0 - finalColor.a) * sampleColor;
		 

		//Next sample & early termination

		sample_pos += u_steplength * ray_dir;

		if (sample_pos.x > 1.0 || sample_pos.x < -1.0 || sample_pos.y > 1.0 || sample_pos.y < -1.0 || sample_pos.z > 1.0 || sample_pos.z < -1.0){
			break;
		}
		
		if (finalColor.a >= 1.0) {
			break;
		}


	}

	if (finalColor.a <= 0.01){
		discard;
	} else {
		gl_FragColor = finalColor ;
	}


}
