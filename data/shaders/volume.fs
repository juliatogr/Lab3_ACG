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
uniform sampler2D u_tf;
uniform bool u_usetransfer;
uniform bool u_usejittering;
uniform bool u_useclipping;
uniform vec4 u_plane;


float rand(vec2 co){
	return fract(sin(dot(co, vec2(12.9898, 78.233))) * 43758.5453);
}

void main()
{

	//Ray Setup
	vec3 ray_dir = normalize(v_position - u_camera_pos);
	vec4 finalColor = vec4(0.0);

	vec3 sample_pos;

	if (u_usejittering){
		vec2 noise_pos = gl_FragCoord.xy / 128;
		//float offset = texture(u_noise, noise_pos).x; //first approach
		float offset = rand(gl_FragCoord.xy);		 // second approach
		sample_pos = v_position + u_steplength*ray_dir*offset;
	} else {
		sample_pos = v_position;
	}

	for (int i = 0; i < 10000; i++){
		
		// volume sampling
		vec3 text_coord = (sample_pos + 1) /2;
		float d = texture(u_texture, text_coord).x;

		//classification

		vec4 sampleColor;

		if (u_useclipping && u_plane.x*sample_pos.x + u_plane.y*sample_pos.y + u_plane.z*sample_pos.z + u_plane.a > 0){
			sampleColor = vec4(0);
		} else {
			if (u_usetransfer && d > 0.1){

				if (d < 0.3){
					sampleColor = texture(u_tf, vec2(0.2,0));
				} else if (d < 0.4){
					sampleColor = texture(u_tf, vec2(0.5,0));
				} else{
					sampleColor = texture(u_tf, vec2(0.8,0));
				}
			} else {
				sampleColor = vec4(d, d, d, d);
				sampleColor.rgb *= u_color.rgb;
			}
		
			sampleColor.rgb *= sampleColor.a;
		
			sampleColor.rgb *= u_brightness;
		}


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
