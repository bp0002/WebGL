export const vs_ribbon = `
#ifdef GL_ES
precision mediump float;
#endif
attribute   vec3    a_position;
attribute   vec4    a_color;
attribute   vec2    a_uv;

varying     vec3    v_surfacePosition;
varying     vec4    v_color;
varying     vec2    v_UV;

uniform     vec3    u_translate;
uniform     vec3    u_scale;
uniform     vec3    u_rotate;
uniform     mat4    u_ViewMatrix;

void main( void ){
    vec2 pos        = a_position.xy;
    pos += vec2(0.5);
    pos *= u_scale.xy;
    pos -= vec2(0.5);

    pos += u_translate.xy;

    gl_Position         = u_ViewMatrix * vec4( pos, 0., 1. );
    gl_PointSize        = 10.0;
    v_surfacePosition   = a_position;
    v_color             = a_color;
    v_UV                = a_uv;
}
`;
export const fs_ribbon = `
#ifdef GL_ES
precision mediump float;
#endif

uniform  sampler2D u_sampler;

varying  vec3   v_surfacePosition;
varying  vec4   v_color;
varying  vec2   v_UV;

void main(void){
    // gl_FragColor = v_color;
    gl_FragColor = texture2D( u_sampler, v_UV );
    // gl_FragColor = texture2D( u_sampler, v_UV );
}
`;