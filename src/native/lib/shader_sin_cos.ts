export const vs_sin_cos = `
#ifdef GL_ES
precision mediump float;
#endif
attribute   vec2    a_position;
varying     vec2    v_surfacePosition;

void main( void ){
    gl_Position = vec4( a_position, 0., 1. );
    v_surfacePosition      = a_position;
}
`;
export const fs_sin_cos = `
// Author @patriciogv - 2015
// Title: Truchet - 10 print
#ifdef GL_ES
precision mediump float;
#endif

uniform vec2    u_resolution;
uniform vec2    u_mouse;
uniform float   u_time;
uniform vec3    u_translate;
uniform vec3    u_scale;
uniform vec3    u_rotate;

void main(){
    vec2 st = gl_FragCoord.xy/u_resolution.xy;
    st -= u_translate.xy;
    st *= 1.0/u_scale.xy;
    vec3 color = vec3(0.0);

    vec2 pos = vec2(0.5)-st;

    float r = distance(pos, vec2(0.0,0.0))*2.0;
    float a = atan(pos.y,pos.x);
    float xxx = floor(u_time / 10.0) * 10.0 - u_time ;
    float yyy = floor(u_time / 20.0) * 20.0 - u_time ;
    float f = cos(a*0.5);
    // f = abs(cos(a*3.));
    // f = abs(cos(a*2.5))*.5+.3;
    // f = abs(cos(a*12.)*sin(a*3.))*.8+.1;
    // f = smoothstep(-.5,1., cos(a*10.))*0.2+0.5;
    f = abs( cos(a*xxx) * sin(a*yyy) ) *.9 + .2;

    color = vec3( 1.-smoothstep(f,f+0.005,r) );

    gl_FragColor = vec4(color, 1.0);
}
`;