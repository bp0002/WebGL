export const vs_wave2d = `
#ifdef GL_ES
precision mediump float;
#endif

attribute   vec2    a_position;

uniform     mat4    u_ViewMatrix;

attribute   vec4    a_color;
attribute   vec2    a_uv;

varying     vec4    v_color;
varying     vec2    v_UV;

void main( void ){
    gl_Position = u_ViewMatrix * vec4( a_position, 0., 1. );
    v_color     = a_color;
    v_UV        = a_uv;
}
`;
export const fs_wave2d = `
#ifdef GL_ES
precision mediump float;
#endif

uniform  sampler2D u_sampler;

varying  vec3   v_surfacePosition;
varying  vec4   v_color;
varying  vec2   v_UV;

void main(void){
    float height = 1.0;
    float total  = 200.0;
    float scale_y = total / height;
    float width  = 2.0;
    float total_w  = 200.0;
    float repeat = total_w / width;

    float cc = 2.0 * ((v_UV.y) * (scale_y) - (scale_y - 1.0)) - (cos(v_UV.x * 3.141592653589793 * 2.0 * repeat) + 1.0);
    float alpha = (cc > 0.0) ? 0.0 : 1.0;
    float blend = smoothstep(-1.0-0.01, -1.0, cc);
    gl_FragColor = (cc > -2.0) ? vec4(0.0, 0.0, 1.0, alpha) : vec4(1.0, 0.0, 0.0, 1.0);
}
`;