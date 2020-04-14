export const vs_simple = `
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

    vec4 pos4 = vec4( pos, 0., 1. );

    // Adjust the z to divide by
    float zToDivideBy = 1.0 + pos4.z * 2.0;

    gl_Position         = u_ViewMatrix * vec4(pos4.xy / zToDivideBy, pos4.zw);

    gl_PointSize        = 2.0;
    v_surfacePosition   = a_position;
    v_color             = a_color;
    v_UV                = a_uv;
}
`;
export const fs_simple = `
#ifdef GL_ES
precision mediump float;
#endif

uniform  sampler2D u_sampler;

varying  vec3   v_surfacePosition;
varying  vec4   v_color;
varying  vec2   v_UV;

void main(void){
    gl_FragColor = v_color;
}
`;