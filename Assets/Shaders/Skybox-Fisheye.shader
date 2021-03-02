// For Documentation, see https://docs.google.com/document/d/1enusteUBDpHemOxpGrpasH_f7sT_tqDMNkcPdP22Kyw/edit?usp=sharing

Shader "Skybox/Fisheye" {
    Properties{
        _Tint("Tint Color", Color) = (.5, .5, .5, .5)
        [Gamma] _Exposure("Exposure", Range(0, 8)) = 1.0
        _Rotation("Rotation", Range(0, 360)) = 0
        [NoScaleOffset] _Tex("Stereo Spherical  (HDR)", 2D) = "grey" {}
        // Fisheye requires two separate images
        [NoScaleOffset] _LTex("Left Spherical  (HDR)", 2D) = "grey" {}
        [NoScaleOffset] _RTex("Right Spherical  (HDR)", 2D) = "grey" {}
        [KeywordEnum(6 Frames Layout, Latitude Longitude Layout, Fisheye Layout)] _Mapping("Mapping", Float) = 1
        [Enum(360 Degrees, 0, 180 Degrees, 1)] _ImageType("Image Type", Float) = 0
        [Toggle] _MirrorOnBack("Mirror on Back", Float) = 0
        [Enum(3D Mode, 0, 2D Mode, 1)] _Mode("Display Mode", Int) = 0
        [Enum(None, 0, Side by Side, 1, Over Under, 2)] _Layout("3D Layout", Float) = 0

        // Fisheye Calibration
        _L_CX("Left Image Center (X)", Range(0, 1)) = 0.5
        _L_CY("Left Image Center (Y)", Range(0, 1)) = 0.5
        _L_RX("Left Image Radius (X)", Range(0, 1)) = 0.5
        _L_RY("Left Image Radius (Y)", Range(0, 1)) = 0.5

        _R_CX("Right Image Center (X)", Range(0, 1)) = 0.5
        _R_CY("Right Image Center (Y)", Range(0, 1)) = 0.5
        _R_RX("Right Image Radius (X)", Range(0, 1)) = 0.5
        _R_RY("Right Image Radius (Y)", Range(0, 1)) = 0.5

        _a("Fisheye a", Range(-2, 2)) = 0
        _b("Fisheye b", Range(-2, 2)) = 0
        _c("Fisheye c", Range(-2, 2)) = 0
    }

        SubShader{
            Tags { "Queue" = "Background" "RenderType" = "Background" "PreviewType" = "Skybox" }
            Cull Off ZWrite Off

            Pass {

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 2.0
                #pragma multi_compile __ _MAPPING_6_FRAMES_LAYOUT
                #pragma multi_compile __ _MAPPING_LATITUDE_LONGITUDE_LAYOUT
                #pragma multi_compile __ _MAPPING_FISHEYE_LAYOUT

                #include "UnityCG.cginc"

                half4 _Tint;
                half _Exposure;
                float _Rotation;

        #ifndef _MAPPING_6_FRAMES_LAYOUT
                bool _MirrorOnBack;
                int _ImageType;
                int _Layout;
        #endif

        #ifdef _MAPPING_FISHEYE_LAYOUT
                sampler2D _LTex;
                float4 _LTex_TexelSize;
                half4 _LTex_HDR;
                sampler2D _RTex;
                float4 _RTex_TexelSize;
                half4 _RTex_HDR;

                int _Mode;

                float _L_CX;
                float _L_CY;
                float _R_CX;
                float _R_CY;
                float _L_RX;
                float _L_RY;
                float _R_RX;
                float _R_RY;

                float _a;
                float _b;
                float _c;
        #else
                // Non-fisheye layouts take in one image as input
                sampler2D _Tex;
                float4 _Tex_TexelSize;
                half4 _Tex_HDR;
        #endif

        #ifndef _MAPPING_6_FRAMES_LAYOUT
                inline float2 ToRadialCoords(float3 coords)
                {
                    float3 normalizedCoords = normalize(coords);
                    float latitude = acos(normalizedCoords.y);
                    float longitude = atan2(normalizedCoords.z, normalizedCoords.x);
                    float2 sphereCoords = float2(longitude, latitude) * float2(0.5 / UNITY_PI, 1.0 / UNITY_PI);
                    return float2(0.5,1.0) - sphereCoords;
                }
        #endif


        #ifdef _MAPPING_FISHEYE_LAYOUT
                inline float2 ToFisheyeCoords(float3 coords)
                {
                    float3 n = normalize(coords);

                    float r = atan2(length(n.xy), abs(n.z)) / UNITY_PI;
                    float phi = atan2(n.y, n.x * sign(n.z));

                    float d = 1 - (_a + _b + _c);
                    float r_src = _a * r*r*r*r + _b * r*r*r + _c * r*r + d * r;

                    float2 uv = float2(cos(phi), sin(phi)) * r_src + .5;

                    uv.x *= .5; 
                    uv.x += .25 * (1 - sign(n.z));

                    return uv;
                }
        #endif

        #ifdef _MAPPING_6_FRAMES_LAYOUT
                inline float2 ToCubeCoords(float3 coords, float3 layout, float4 edgeSize, float4 faceXCoordLayouts, float4 faceYCoordLayouts, float4 faceZCoordLayouts)
                {
                    // Determine the primary axis of the normal
                    float3 absn = abs(coords);
                    float3 absdir = absn > float3(max(absn.y,absn.z), max(absn.x,absn.z), max(absn.x,absn.y)) ? 1 : 0;
                    // Convert the normal to a local face texture coord [-1,+1], note that tcAndLen.z==dot(coords,absdir)
                    // and thus its sign tells us whether the normal is pointing positive or negative
                    float3 tcAndLen = mul(absdir, float3x3(coords.zyx, coords.xzy, float3(-coords.xy,coords.z)));
                    tcAndLen.xy /= tcAndLen.z;
                    // Flip-flop faces for proper orientation and normalize to [-0.5,+0.5]
                    bool2 positiveAndVCross = float2(tcAndLen.z, layout.x) > 0;
                    tcAndLen.xy *= (positiveAndVCross[0] ? absdir.yx : (positiveAndVCross[1] ? float2(absdir[2],0) : float2(0,absdir[2]))) - 0.5;
                    // Clamp values which are close to the face edges to avoid bleeding/seams (ie. enforce clamp texture wrap mode)
                    tcAndLen.xy = clamp(tcAndLen.xy, edgeSize.xy, edgeSize.zw);
                    // Scale and offset texture coord to match the proper square in the texture based on layout.
                    float4 coordLayout = mul(float4(absdir,0), float4x4(faceXCoordLayouts, faceYCoordLayouts, faceZCoordLayouts, faceZCoordLayouts));
                    tcAndLen.xy = (tcAndLen.xy + (positiveAndVCross[0] ? coordLayout.xy : coordLayout.zw)) * layout.yz;
                    return tcAndLen.xy;
                }
        #endif

                float3 RotateAroundYInDegrees(float3 vertex, float degrees)
                {
                    float alpha = degrees * UNITY_PI / 180.0;
                    float sina, cosa;
                    sincos(alpha, sina, cosa);
                    float2x2 m = float2x2(cosa, -sina, sina, cosa);
                    return float3(mul(m, vertex.xz), vertex.y).xzy;
                }

                struct appdata_t {
                    float4 vertex : POSITION;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct v2f {
                    float4 vertex : SV_POSITION;
                    float3 texcoord : TEXCOORD0;
        #ifdef _MAPPING_6_FRAMES_LAYOUT
                    float3 layout : TEXCOORD1;
                    float4 edgeSize : TEXCOORD2;
                    float4 faceXCoordLayouts : TEXCOORD3;
                    float4 faceYCoordLayouts : TEXCOORD4;
                    float4 faceZCoordLayouts : TEXCOORD5;
        #else
                    float2 image180ScaleAndCutoff : TEXCOORD1;
                    float4 layout3DScaleAndOffset : TEXCOORD2;
        #endif
                    UNITY_VERTEX_OUTPUT_STEREO
                };

                v2f vert(appdata_t v)
                {
                    v2f o;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                    float3 rotated = RotateAroundYInDegrees(v.vertex, _Rotation);
                    o.vertex = UnityObjectToClipPos(rotated);
                    o.texcoord = v.vertex.xyz;
        #ifdef _MAPPING_6_FRAMES_LAYOUT
                    // layout and edgeSize are solely based on texture dimensions and can thus be precalculated in the vertex shader.
                    float sourceAspect = float(_Tex_TexelSize.z) / float(_Tex_TexelSize.w);
                    // Use the halfway point between the 1:6 and 3:4 aspect ratios of the strip and cross layouts to
                    // guess at the correct format.
                    bool3 aspectTest =
                        sourceAspect >
                        float3(1.0, 1.0f / 6.0f + (3.0f / 4.0f - 1.0f / 6.0f) / 2.0f, 6.0f / 1.0f + (4.0f / 3.0f - 6.0f / 1.0f) / 2.0f);
                    // For a given face layout, the coordinates of the 6 cube faces are fixed: build a compact representation of the
                    // coordinates of the center of each face where the first float4 represents the coordinates of the X axis faces,
                    // the second the Y, and the third the Z. The first two float componenents (xy) of each float4 represent the face
                    // coordinates on the positive axis side of the cube, and the second (zw) the negative.
                    // layout.x is a boolean flagging the vertical cross layout (for special handling of flip-flops later)
                    // layout.yz contains the inverse of the layout dimensions (ie. the scale factor required to convert from
                    // normalized face coords to full texture coordinates)
                    if (aspectTest[0]) // horizontal
                    {
                        if (aspectTest[2])
                        { // horizontal strip
                            o.faceXCoordLayouts = float4(0.5,0.5,1.5,0.5);
                            o.faceYCoordLayouts = float4(2.5,0.5,3.5,0.5);
                            o.faceZCoordLayouts = float4(4.5,0.5,5.5,0.5);
                            o.layout = float3(-1,1.0 / 6.0,1.0 / 1.0);
                        }
                        else
                        { // horizontal cross
                            o.faceXCoordLayouts = float4(2.5,1.5,0.5,1.5);
                            o.faceYCoordLayouts = float4(1.5,2.5,1.5,0.5);
                            o.faceZCoordLayouts = float4(1.5,1.5,3.5,1.5);
                            o.layout = float3(-1,1.0 / 4.0,1.0 / 3.0);
                        }
                    }
                    else
                    {
                        if (aspectTest[1])
                        { // vertical cross
                            o.faceXCoordLayouts = float4(2.5,2.5,0.5,2.5);
                            o.faceYCoordLayouts = float4(1.5,3.5,1.5,1.5);
                            o.faceZCoordLayouts = float4(1.5,2.5,1.5,0.5);
                            o.layout = float3(1,1.0 / 3.0,1.0 / 4.0);
                        }
                        else
                        { // vertical strip
                            o.faceXCoordLayouts = float4(0.5,5.5,0.5,4.5);
                            o.faceYCoordLayouts = float4(0.5,3.5,0.5,2.5);
                            o.faceZCoordLayouts = float4(0.5,1.5,0.5,0.5);
                            o.layout = float3(-1,1.0 / 1.0,1.0 / 6.0);
                        }
                    }
                    // edgeSize specifies the minimum (xy) and maximum (zw) normalized face texture coordinates that will be used for
                    // sampling in the texture. Setting these to the effective size of a half pixel horizontally and vertically
                    // effectively enforces clamp mode texture wrapping for each individual face.
                    o.edgeSize.xy = _Tex_TexelSize.xy * 0.5 / o.layout.yz - 0.5;
                    o.edgeSize.zw = -o.edgeSize.xy;
        #else // !_MAPPING_6_FRAMES_LAYOUT
                    // Calculate constant horizontal scale and cutoff for 180 (vs 360) image type
                    if (_ImageType == 0)  // 360 degree
                        o.image180ScaleAndCutoff = float2(1.0, 1.0);
                    else  // 180 degree
                        o.image180ScaleAndCutoff = float2(2.0, _MirrorOnBack ? 1.0 : 0.5);

                    // Calculate constant scale and offset for 3D layouts
                    if (_Layout == 0) // No 3D layout
                        o.layout3DScaleAndOffset = float4(0,0,1,1);
                    else if (_Layout == 1) // Side-by-Side 3D layout
                        o.layout3DScaleAndOffset = float4(unity_StereoEyeIndex,0,0.5,1);
                    else // Over-Under 3D layout
                        o.layout3DScaleAndOffset = float4(0, 1 - unity_StereoEyeIndex,1,0.5);
        #endif
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
        #ifdef _MAPPING_6_FRAMES_LAYOUT
                    float2 tc = ToCubeCoords(i.texcoord, i.layout, i.edgeSize, i.faceXCoordLayouts, i.faceYCoordLayouts, i.faceZCoordLayouts);
        #elif _MAPPING_FISHEYE_LAYOUT
                    float2 tc = ToFisheyeCoords(i.texcoord);
                    if (tc.x > i.image180ScaleAndCutoff[1])
                        return half4(0,0,0,1);

                    tc.x = fmod(tc.x * i.image180ScaleAndCutoff[0], 1);

                    half4 tex;
                    half3 c;
                    
                    if (_Mode == 0)
                    {
                        // 3D Mode - compute separate frames for each eye
                        if (unity_StereoEyeIndex == 0) { // Left Eye
                            tc.x = (2 * _L_RX * tc.x) + _L_CX - _L_RX;
                            tc.y = (2 * _L_RY * tc.y) + _L_CY - _L_RY;
                            tex = tex2D(_LTex, tc);
                            c = DecodeHDR(tex, _LTex_HDR);
                        }
                        else { // Right Eye
                            tc.x = (2 * _R_RX * tc.x) + _R_CX - _R_RX;
                            tc.y = (2 * _R_RY * tc.y) + _R_CY - _R_RY;
                            tex = tex2D(_RTex, tc);
                            c = DecodeHDR(tex, _RTex_HDR);
                        }
                    }
                    else {
                        // 2D Mode - use one eye only
                        tc.x = (2 * _L_RX * tc.x) + _L_CX - _L_RX;
                        tc.y = (2 * _L_RY * tc.y) + _L_CY - _L_RY;
                        tex = tex2D(_LTex, tc);
                        c = DecodeHDR(tex, _LTex_HDR);
                    }


                    if (tc.y > 1 || tc.y < 0)
                        return half4(0, 0, 0, 1);
                    c = c * _Tint.rgb * unity_ColorSpaceDouble.rgb;
                    c *= _Exposure;
                    return half4(c, 1);
        #else// _MAPPING_LATITUDE_LONGITUDE_LAYOUT
                    float2 tc = ToRadialCoords(i.texcoord);
                    if (tc.x > i.image180ScaleAndCutoff[1])
                        return half4(0,0,0,1);
                    tc.x = fmod(tc.x * i.image180ScaleAndCutoff[0], 1);
                    tc = (tc + i.layout3DScaleAndOffset.xy) * i.layout3DScaleAndOffset.zw;
        #endif

        #ifndef _MAPPING_FISHEYE_LAYOUT
                    half4 tex = tex2D(_Tex, tc);
                    half3 c = DecodeHDR(tex, _Tex_HDR);
                    c = c * _Tint.rgb * unity_ColorSpaceDouble.rgb;
                    c *= _Exposure;
                    return half4(c, 1);
        #endif
                }
                ENDCG
            }
        }


            CustomEditor "SkyboxFisheyeShaderGUI"
                    Fallback Off

}
