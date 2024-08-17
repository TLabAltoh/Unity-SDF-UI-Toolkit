/***
* This code is adapted and modified from
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/SDFUtils.cginc
* https://iquilezles.org/articles/distfunctions2d/
**/

#define JACOBIAN(uv) (transpose( float2x2( ddx(uv), ddy(uv) ) ))

#ifdef SDF_UI_QUAD
/*
* p:
* h:
*/
inline float sdRectangle(float2 p, float2 h) {
    float2 distanceToEdge = abs(p) - h;
    float outsideDistance = length(max(distanceToEdge, 0));
    float insideDistance = min(max(distanceToEdge.x, distanceToEdge.y), 0);
    return outsideDistance + insideDistance;
}

/*
* p:
* h:
* r: radius
*/
inline float sdRoundedBox(float2 p, float2 b, float4 r) {
    r.xy = (p.x > 0.0) ? r.xy : r.zw;
    r.x = (p.y > 0.0) ? r.x : r.y;
    float2 q = abs(p) - b + r.x;
    return min(max(q.x, q.y), 0.0) + length(max(q, 0.0)) - r.x;
}
#endif

#ifdef SDF_UI_CIRCLE
/*
* p:
* r: radius
*/
inline float sdCircle(float2 p, float r) {
    return length(p) - r;
}
#endif

#ifdef SDF_UI_PIE
/*
* p:
* c: range (sin(theta), cos(theta))
* r: radius
*/
inline float sdPie(float2 p, float2 c, float r)
{
    p.x = abs(p.x);
    float l = length(p) - r;
    float m = length(p - c * clamp(dot(p, c), 0.0, r)); // c=sin/cos of aperture
    return max(l, m * sign(c.y * p.x - c.x * p.y));
}
#endif

#ifdef SDF_UI_ARC
/*
* p:
* sc: range (sin(theta), cos(theta))
* ra: radius
* rb: width
*/
inline float sdArc(float2 p, float2 sc, float ra, float rb)
{
    p.x = abs(p.x);
    return ((sc.y * p.x > sc.x * p.y) ? length(p - sc * ra) :
        abs(length(p) - ra)) - rb;
}
#endif

#ifdef SDF_UI_RING
/*
* p:
* n: range (cos(theta), sin(theta))
* r: raidus
* th: width
*/
inline float sdRing(float2 p, float2 n, float r, float th)
{
    p.x = abs(p.x);

    float2 t = p;

    p.x = t.x * n.x - t.y * n.y;
    p.y = t.x * n.y + t.y * n.x;

    return max(abs(length(p) - r) - th * 0.5,
        length(float2(p.x, max(0.0, abs(r - p.y) - th * 0.5))) * sign(p.x));
}
#endif

#ifdef SDF_UI_TRIANGLE
/*
* p:
* p0:
* p1:
* p2:
*/
inline float sdTriangle(float2 p, float2 p0, float2 p1, float2 p2)
{
    float2 e0 = p1 - p0, e1 = p2 - p1, e2 = p0 - p2;
    float2 v0 = p - p0, v1 = p - p1, v2 = p - p2;
    float2 pq0 = v0 - e0 * clamp(dot(v0, e0) / dot(e0, e0), 0.0, 1.0);
    float2 pq1 = v1 - e1 * clamp(dot(v1, e1) / dot(e1, e1), 0.0, 1.0);
    float2 pq2 = v2 - e2 * clamp(dot(v2, e2) / dot(e2, e2), 0.0, 1.0);
    float s = sign(e0.x * e2.y - e0.y * e2.x);
    float2 d = min(min(float2(dot(pq0, pq0), s * (v0.x * e0.y - v0.y * e0.x)),
        float2(dot(pq1, pq1), s * (v1.x * e1.y - v1.y * e1.x))),
        float2(dot(pq2, pq2), s * (v2.x * e2.y - v2.y * e2.x)));
    return -sqrt(d.x) * sign(d.y);
}
#endif

#ifdef SDF_UI_CUT_DISK
/*
* p:
* r:
* h:
*/
inline float sdCutDisk(float2 p, float r, float h)
{
    float w = sqrt(r * r - h * h); // constant for any given shape
    p.x = abs(p.x);
    float s = max((h - r) * p.x * p.x + w * w * (h + r - 2.0 * p.y), h * p.x - w * p.y);
    return (s < 0.0) ? length(p) - r :
        (p.x < w) ? h - p.y :
        length(p - float2(w, h));
}
#endif

#ifdef SDF_UI_SPLINE
inline float sdTriangle(float2 p, float2 p0, float2 p1, float2 p2)
{
    float2 e0 = p1 - p0, e1 = p2 - p1, e2 = p0 - p2;
    float2 v0 = p - p0, v1 = p - p1, v2 = p - p2;
    float2 pq0 = v0 - e0 * clamp(dot(v0, e0) / dot(e0, e0), 0.0, 1.0);
    float2 pq1 = v1 - e1 * clamp(dot(v1, e1) / dot(e1, e1), 0.0, 1.0);
    float2 pq2 = v2 - e2 * clamp(dot(v2, e2) / dot(e2, e2), 0.0, 1.0);
    float s = sign(e0.x * e2.y - e0.y * e2.x);
    float2 d = min(min(float2(dot(pq0, pq0), s * (v0.x * e0.y - v0.y * e0.x)),
        float2(dot(pq1, pq1), s * (v1.x * e1.y - v1.y * e1.x))),
        float2(dot(pq2, pq2), s * (v2.x * e2.y - v2.y * e2.x)));
    return -sqrt(d.x) * sign(d.y);
}
#endif

inline float round(float d, float r)
{
    return d - r;
}

inline float round(float4 d, float r)
{
    return float4(round(d.r, r), round(d.g, r), round(d.b, r), round(d.a, r));
}

inline float onion(float2 d, float r)
{
    return abs(d) - r;
}

inline half4 blend(half4 color0, half4 color1, half delta) {
    //if (delta < 1 && delta > 0.) {
    //    half2 ratio = half2(color0.a + 0.001, color1.a + 0.001);
    //    ratio = ratio.xy / max(ratio.x, ratio.y);
    //    half4 layer2 = half4(lerp(color0.rgb, color1.rgb, ratio.y), color1.a);
    //    half4 layer3 = half4(lerp(color1.rgb, color0.rgb, ratio.x), color0.a);
    //    return lerp(layer3, layer2, delta);
    //}
    half4 layer0 = lerp(color1, color0, color0.a);
    return lerp(layer0, color1, delta);
}