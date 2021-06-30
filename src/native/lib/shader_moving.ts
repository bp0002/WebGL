export const vs_moving = `
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
    v_UV        = (vec2(1.0, 1.0) - a_uv) * 2.0 - 1.0;
}
`;
export const fs_moving = `
#ifdef GL_ES
precision mediump float;
#endif

uniform  sampler2D u_sampler;
uniform  float  u_time;

varying  vec3   v_surfacePosition;
varying  vec4   v_color;
varying  vec2   v_UV;

void main(void){
    float radius    = 0.5;
    float angle     = cos(u_time * 3.14159 * 0.1);

    float len       = length(v_UV);
    float angle0    = v_UV.x / len;
    float angleMask = 2.0 * smoothstep(angle - 0.01, angle + 0.01, angle0) * (1.0 - smoothstep(angle - 0.01, angle + 0.01, angle0));
    float radiuMask = smoothstep(radius - 0.1, radius + 0.1, len) * (1.0 - smoothstep(radius - 0.1, radius + 0.1, len));

    gl_FragColor.rgb = vec3(1.0, 0.0, 0.0) * angleMask * radiuMask;
    gl_FragColor.a = smoothstep(0.0, 0.1, angleMask * radiuMask);
}
`;