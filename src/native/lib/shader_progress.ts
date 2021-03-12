export const vs_progress = `
#ifdef GL_ES
precision mediump float;
#endif

attribute   vec2    a_position;

uniform     mat4    u_ViewMatrix;

void main( void ){
    gl_Position = u_ViewMatrix * vec4( a_position, 0., 1. );
}
`;
export const fs_progress = `
#define SAMPLER3DBGRMAP 0.0

#define SHADER_NAME fragment:default

#extension GL_OES_standard_derivatives : enable
#ifdef GL_ES
precision mediump float;
#endif

uniform  float u_float;

uniform  vec2 u_resolution;

void main(void){
    vec2 st = gl_FragCoord.xy / u_resolution.xy;
    vec3 color = st.x < u_float ? vec3(0.0, 0.8, 0.0) : vec3(0.2, 0.2, 0.2);
    // 20 对应js层进度条高度
    color = st.y > (20.0 / u_resolution.y / 2.0) ? vec3(0.0) : color;

    float alpha = st.y > (20.0 / u_resolution.y / 2.0) ? 0.0 : 0.8;
    alpha = st.x < u_float ? alpha : 0.8 + SAMPLER3DBGRMAP;

    gl_FragColor = vec4( color, alpha );
}
`;