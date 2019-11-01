export const vs_simple = `
#ifdef GL_ES
precision mediump float;
#endif
attribute   vec3    a_position;
attribute   vec4    a_color;

varying     vec3    v_surfacePosition;
varying     vec4    v_color;

uniform     vec3    u_translate;
uniform     vec3    u_scale;
uniform     vec3    u_rotate;

void main( void ){
    vec2 pos        = a_position.xy;
    pos += vec2(0.5);
    pos *= u_scale.xy;
    pos -= vec2(0.5);

    pos += u_translate.xy;

    gl_Position         = vec4( pos, 0., 1. );
    v_surfacePosition   = a_position;
    v_color             = a_color;
}
`;
export const fs_simple = `
#ifdef GL_ES
precision mediump float;
#endif

varying  vec3   v_surfacePosition;
varying  vec4   v_color;

void main(void){
    gl_FragColor = v_color;
}
`;