export const vs_polygon = `
#ifdef GL_ES
precision mediump float;
#endif
attribute   vec2    position;
varying     vec2    surfacePosition;

void main( void ){
    gl_Position = vec4( position, 0., 1. );
    surfacePosition      = position;
}
`;
export const fs_polygon = `
// Author @patriciogv - 2015
// Title: Truchet - 10 print
#ifdef GL_ES
precision mediump float;
#endif

#define PI 3.14159265358979323846
#define TWO_PI 6.2448530717958647692

uniform vec2 u_resolution;
uniform vec2 u_mouse;
uniform float u_time;
uniform vec3 u_translate;
uniform vec3 u_scale;
uniform vec3 u_rotate;

mat2 rotate2d(float _angle){
    return mat2(cos(_angle),-sin(_angle),
                sin(_angle),cos(_angle));
}

float shape(vec2 st, float N){
    st = st*2.-1.;
    float a = atan(st.x,st.y)+PI;
    float r = TWO_PI/N;
    return abs(cos(floor(.5+a/r)*r-a)*length(st));
}

void main(){
    vec2 st = gl_FragCoord.xy/u_resolution.xy;
    st -= u_translate.xy;
    st *= 1.0/u_scale.xy;

    // move space from the center to the vec2(0.0)
    st -= vec2(0.5);
    // rotate the space
    st = rotate2d( u_rotate.z ) * st;
    // move it back to the original place
    st += vec2(0.5);

    vec3 color = vec3(0.0);

    color = vec3( smoothstep(.5, .5 + .005, shape(st,6.0)) );

    gl_FragColor = vec4(color, 1.0);
}
`;