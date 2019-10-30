export const vs_multi_line_diff_speed = `
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
export const fs_multi_line_diff_speed = `
// Author @patriciogv - 2015
// Title: Truchet - 10 print

#ifdef GL_ES
precision mediump float;
#endif

#define PI 3.14159265358979323846

uniform vec2 u_resolution;
uniform vec2 u_mouse;
uniform float u_time;
uniform vec3 u_translate;
uniform vec3 u_scale;
uniform vec3 u_rotate;

float random (in vec2 _st) {
    return fract(sin(dot(_st.xy,
                         vec2(12.9898,78.233)))*
        43758.5453123);
}

vec2 truchetPattern(in vec2 _st, in float _index){
    _index = fract(((_index-0.5)*2.0));
    if (_index > 0.75) {
        _st = vec2(1.0) - _st;
    } else if (_index > 0.5) {
        _st = vec2(1.0-_st.x,_st.y);
    } else if (_index > 0.25) {
        _st = 1.0-vec2(1.0-_st.x,_st.y);
    }
    return _st;
}
float circle(vec2 xy, vec2 center, float radius, float smooth_edge) {
    float dist = distance(xy,center);
    dist = smoothstep(radius, radius + smooth_edge, dist);
    return dist;
}

void main() {
    float count = 20.0;
    vec2 st = gl_FragCoord.xy/u_resolution.xy;
    st -= u_translate.xy;
    st *= 1.0/u_scale.xy;
    st *= count;
	st.x -= u_time * 2.0 *  random(vec2(floor(st).y,1.0));

    vec2 ipos = floor(st);  // integer
    vec2 fpos = fract(st);  // fraction

    float speed = random(vec2(ipos.y));

    float x_f = random(vec2(ipos.x, 0));
    float y_f = random(vec2(0, ipos.y));

    float color = 0.0;
    color = random(vec2(x_f, y_f));
	color =
    	1.0 - (1.0 - circle(fract(st), vec2(0.5), 0.25, 0.1)) * color
    	;
    vec4 f_color = vec4(vec3(color),1.0);
    f_color.r = color < 1.0 ? 0.0 : 1.0;
    f_color.g = color < 1.0 ? 0.0 : 1.0;
    f_color.b = color < 1.0 ? color : 1.0;
    f_color.a = color < 1.0 ? color : 0.0;

    gl_FragColor = f_color;
}
`;