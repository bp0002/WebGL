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

    vec2 scale = mix(vec2(u_resolution.x/u_resolution.y, 1.0), vec2(1.0, u_resolution.y/u_resolution.x), step(u_resolution.x, u_resolution.y));
    st      = st * 2.0 - 1.0; // -1 ~ 1
    st      *= scale;

    float pi = 3.141592653589;
    // 圆点数
    float pointCount    = 16.0;
    float perThi        = pi * 2.0 / pointCount;
    float deltaThi      = u_float;

    // 寻找圆点圆心
    float baseThi       = acos(dot(normalize(st), vec2(1.,0.))) * mix(-1.0, 1.0, step(0., st.y)); // / pi / 2.0 + 0.5;
    baseThi             = (floor(baseThi / perThi) + 0.5) * perThi;

    // 转动半径
    float r1 = 0.4;
    // 原点半径
    float r2 = 0.05;

    // 从 deltaThi 角度旋转一周的进度
    float p = fract((deltaThi - baseThi) / pi / 2.0);

    vec2 target         = vec2(cos(baseThi) * r1, sin(baseThi) * r1);

    float alpha         = 1.0 - smoothstep(r2, r2 + 0.01, length(target - st));

    vec3 color          = vec3(1.0, 0., 0.);

    gl_FragColor = vec4( color, alpha * p );
}
`;