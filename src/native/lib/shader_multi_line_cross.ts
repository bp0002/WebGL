export const vs_multi_line_cross = `
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
export const fs_multi_line_cross = `
// Author @patriciogv - 2015
// Title: Truchet - 10 print
#ifdef GL_ES
precision mediump float;
#endif

uniform vec2 u_resolution;
uniform float u_time;
uniform vec3 u_translate;
uniform vec3 u_scale;
uniform vec3 u_rotate;

vec2 brickTile(vec2 _st, float _zoom){
    _st *= _zoom;

    // Here is where the offset is happening
    // _st.x += step(1., mod(_st.y,2.0)) * 0.5;

    return fract(_st);
}

float box(vec2 _st, vec2 _size){
    _size = vec2(0.5)-_size*0.5;
    vec2 uv = smoothstep(_size,_size+vec2(1e-4),_st);
    uv *= smoothstep(_size,_size+vec2(1e-4),vec2(1.0)-_st);
    return uv.x*uv.y;
}
float circle(vec2 xy, vec2 center, float radius, float smooth_edge) {
    float dist = distance(xy,center);
    dist = smoothstep(radius, radius + smooth_edge, dist);
    return dist;
}

void main(void){
    float count = 20.0;
    vec2 st = gl_FragCoord.xy/u_resolution.xy;
    st -= u_translate.xy;
    st *= 1.0/u_scale.xy;

    float sin_t = sin(u_time * 3.14 / 10.0);
    float cos_t = cos(u_time * 3.14 / 10.0);
    float col_flag= mod(st.y * count, 2.0) < 1.0 ? 1.0 : -1.0;
    float row_flag= mod(st.x * count, 2.0) < 1.0 ? 1.0 : -1.0;
    st += vec2(
        col_flag * ( sin_t * cos_t < 0.0 ? cos_t : 0.0 ) * 0.5,
        row_flag * ( sin_t * cos_t > 0.0 ? sin_t : 0.0 ) * 0.5);
    vec3 color = vec3(0.0);

    // Modern metric brick of 215mm x 102.5mm x 65mm
    // http://www.jaharrison.me.uk/Brickwork/Sizes.html
    // st /= vec2(2.15,0.65)/1.5;

    // Apply the brick tiling
    st = brickTile(st,count);

    color = vec3(circle(st,vec2(0.5,0.5),0.4, 0.05));

    // Uncomment to see the space coordinates
    // color = vec3(st,0.0);

    gl_FragColor = vec4(color,1.0);
}
`;