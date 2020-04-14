export const vs_texture_grass = `
#ifdef GL_ES
precision mediump float;
#endif

attribute   vec2    a_position;
attribute   vec2    a_uv;

varying     vec2    v_surfacePosition;
varying     vec2    v_UV;

uniform     vec3    u_translate;
uniform     vec3    u_scale;
uniform     vec3    u_rotate;
uniform     float   u_time;
uniform     mat4    u_ViewMatrix;

void main( void ){
    vec2 pos        = a_position.xy;
    pos += vec2(0.5);
    pos *= u_scale.xy;
    pos -= vec2(0.5);

    pos += u_translate.xy;
    // pos.x = pos.x + sin(a_uv.y) ;

    gl_Position     = u_ViewMatrix * vec4( pos, 0., 1. );
    v_surfacePosition      = a_position;
    v_UV = a_uv;
}
`;
export const fs_texture_grass = `
#ifdef GL_ES
precision mediump float;
#endif

varying  vec2 v_UV;

uniform  sampler2D u_sampler;
uniform  sampler2D u_sampler1;

void main(void){
    // [ 0, 0, 0, 1 ]  rgba颜色向量
    // gl_FragColor = vec4( vColor, 1. );

    vec4 mask_color = texture2D( u_sampler1, v_UV );
    float alpha     = (mask_color.r + mask_color.g + mask_color.b) > 1.0 ? 1.0 : 0.0;

    float x = v_UV.x < 0.5 ? abs(v_UV.x - 0.5) : 1.0 - (v_UV.x - 0.5);
    float y = v_UV.y < 0.5 ? abs(v_UV.y - 0.5) : 1.0 - (v_UV.y - 0.5);
    vec4 ctx_color  = texture2D( u_sampler, vec2(x, y) );

    gl_FragColor    = vec4(ctx_color.rgb, alpha);
}
`;