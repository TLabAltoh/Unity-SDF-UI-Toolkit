/***
* This code is adapted and modified from
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/SDFUtils.cginc
* https://iquilezles.org/articles/distfunctions2d/
* https://www.shadertoy.com/view/7stcR4
**/

#define PI 3.14

/**
*
*
*/

inline float select(bool boolean, float a0, float a1) {
    return boolean * a0 + (1. - boolean) * a1;
}

inline float2 select(bool boolean, float2 a0, float2 a1) {
    return boolean * a0 + (1. - boolean) * a1;
}

inline float3 select(bool boolean, float3 a0, float3 a1) {
    return boolean * a0 + (1. - boolean) * a1;
}

inline float4 select(bool boolean, float4 a0, float4 a1) {
    return boolean * a0 + (1. - boolean) * a1;
}

/**
*
*
*/

inline float select(float3 layer, float a0, float a1, float a2) {
    return layer.x * a0 + layer.y * a1 + layer.z * a2;
}

inline float2 select(float3 layer, float2 a0, float2 a1, float2 a2) {
    return layer.x * a0 + layer.y * a1 + layer.z * a2;
}

inline float3 select(float3 layer, float3 a0, float3 a1, float3 a2) {
    return layer.x * a0 + layer.y * a1 + layer.z * a2;
}

inline float4 select(float3 layer, float4 a0, float4 a1, float4 a2) {
    return layer.x * a0 + layer.y * a1 + layer.z * a2;
}

/**
*
*
*/

inline float select(float4 layer, float a0, float a1, float a2, float a3) {
    return layer.x * a0 + layer.y * a1 + layer.z * a2 + layer.w * a3;
}

inline float2 select(float4 layer, float2 a0, float2 a1, float2 a2, float2 a3) {
    return layer.x * a0 + layer.y * a1 + layer.z * a2 + layer.w * a3;
}

inline float3 select(float4 layer, float3 a0, float3 a1, float3 a2, float3 a3) {
    return layer.x * a0 + layer.y * a1 + layer.z * a2 + layer.w * a3;
}

inline float4 select(float4 layer, float4 a0, float4 a1, float4 a2, float4 a3) {
    return layer.x * a0 + layer.y * a1 + layer.z * a2 + layer.w * a3;
}

/**
*
*
*/

inline float onion(float2 d, float r) {
    return abs(d) - r;
}

inline float round(float d, float r) {
    return d - r;
}

inline float round(float4 d, float r) {
    return float4(round(d.r, r), round(d.g, r), round(d.b, r), round(d.a, r));
}

inline float saturaterange(float a, float b, float x) {
    return saturate((x - a) / (b - a));
}

inline float4 saturaterange(float4 a, float4 b, float4 x) {
    return saturate((x - a) / (b - a));
}

/**
*
*
*/

inline float dot2(float2 v) {
    return dot(v, v);
}

inline float cross(float2 a, float2 b) {
    return a.x * b.y - a.y * b.x;
}

inline float2 rotate(float2 pos, float theta) {
    return float2(pos.x * cos(theta) - pos.y * sin(theta), pos.x * sin(theta) + pos.y * cos(theta));
}

/**
*
*
*/

inline float4 conicalGradation(float2 p, float smooth, float2 range, float4 colorA, float4 colorB) {
    float tmp = 0.0;
    tmp = select(p.y >= 0.0, atan2(p.y, p.x), tmp);
    tmp = select(p.y < 0.0, PI - atan2(p.y, -p.x), tmp);

    tmp = smoothstep(range.x * 2 * PI, range.y * 2 * PI, tmp);
    tmp = 1.0 - tmp;

    return colorA * tmp + colorB * (1. - tmp);
}

inline float4 conicalGradation(float2 p, float angle, float rectAngle, float smooth, float2 offset, float2 range, float4 colorA, float4 colorB) {
    p = rotate(p, rectAngle); p = rotate(p - offset, angle);
    return conicalGradation(p, smooth, range, colorA, colorB);
}

inline float4 radialGradation(float2 p, float radius, float smooth, float4 colorA, float4 colorB) {
    float tmp = sqrt((p.x * p.x) + (p.y * p.y));
    tmp = smoothstep(-smooth + radius, smooth + radius, tmp);
    return colorA * tmp + colorB * (1. - tmp);
}

inline float4 radialGradation(float2 p, float angle, float rectAngle, float radius, float smooth, float2 offset, float4 colorA, float4 colorB) {
    p = rotate(p, rectAngle); p = rotate(p - offset, angle);
    return radialGradation(p, radius, smooth, colorA, colorB);
}

inline float4 linearGradation(float2 p, float smooth, float4 colorA, float4 colorB) {
    float tmp = 0.0;
    tmp = select(smooth > 0.0, smoothstep(-smooth, smooth, p.x), saturaterange(-smooth, smooth, p.x));
    return colorA * tmp + colorB * (1. - tmp);
}

inline float4 linearGradation(float2 p, float angle, float rectAngle, float smooth, float2 offset, float4 colorA, float4 colorB) {
    p = rotate(p, rectAngle); p = rotate(p - offset, angle);
    return linearGradation(p, smooth, colorA, colorB);
}

/**
* HSV to RGB conversion
*/
inline float3 hsv2rgb(float3 hsv) {
    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 p = abs(frac(hsv.xxx + K.xyz) * 6.0 - K.www);
    float3 rgb = hsv.z * lerp(K.xxx, saturate(p - K.xxx), hsv.y);
    if (!IsGammaSpace())
    {
        rgb = UIGammaToLinear(rgb);
    }
    return rgb;
}

/**
* Rainbow gradient functions
*/
inline float4 rainbowLinearGradation(float2 p, float smooth, float saturation, float value, float hueOffset) {
    float tmp = 0.0;
    tmp = select(smooth > 0.0, 
                (p.x + smooth) / (2.0 * smooth), 
                (p.x + smooth) / (2.0 * smooth));
    tmp = frac(tmp + hueOffset);
    float3 hsv = float3(tmp, saturation, value);
    float3 rgb = hsv2rgb(hsv);
    return float4(rgb, 1.0);
}

inline float4 rainbowLinearGradation(float2 p, float smooth, float saturation, float value) {
    return rainbowLinearGradation(p, smooth, saturation, value, 0.0);
}

inline float4 rainbowLinearGradation(float2 p, float angle, float rectAngle, float smooth, float2 offset, float saturation, float value, float hueOffset) {
    p = rotate(p, rectAngle); p = rotate(p - offset, angle);
    return rainbowLinearGradation(p, smooth, saturation, value, hueOffset);
}

inline float4 rainbowLinearGradation(float2 p, float angle, float rectAngle, float smooth, float2 offset, float saturation, float value) {
    return rainbowLinearGradation(p, angle, rectAngle, smooth, offset, saturation, value, 0.0);
}

inline float4 rainbowRadialGradation(float2 p, float radius, float smooth, float saturation, float value, float hueOffset) {
    float tmp = sqrt((p.x * p.x) + (p.y * p.y));
    tmp = (tmp - (radius - smooth)) / (2.0 * smooth);
    tmp = frac(tmp + hueOffset);
    float3 hsv = float3(tmp, saturation, value);
    float3 rgb = hsv2rgb(hsv);
    return float4(rgb, 1.0);
}

inline float4 rainbowRadialGradation(float2 p, float radius, float smooth, float saturation, float value) {
    return rainbowRadialGradation(p, radius, smooth, saturation, value, 0.0);
}

inline float4 rainbowRadialGradation(float2 p, float angle, float rectAngle, float radius, float smooth, float2 offset, float saturation, float value, float hueOffset) {
    p = rotate(p, rectAngle); p = rotate(p - offset, angle);
    return rainbowRadialGradation(p, radius, smooth, saturation, value, hueOffset);
}

inline float4 rainbowRadialGradation(float2 p, float angle, float rectAngle, float radius, float smooth, float2 offset, float saturation, float value) {
    return rainbowRadialGradation(p, angle, rectAngle, radius, smooth, offset, saturation, value, 0.0);
}

inline float4 rainbowConicalGradation(float2 p, float smooth, float2 range, float saturation, float value, float hueOffset) {
    float tmp = 0.0;
    tmp = select(p.y >= 0.0, atan2(p.y, p.x), tmp);
    tmp = select(p.y < 0.0, PI * 2.0 + atan2(p.y, p.x), tmp);
    
    tmp = tmp / (2.0 * PI);
    
    if (range.x != 0.0 || range.y != 1.0) {
        tmp = range.x + tmp * (range.y - range.x);
    }
    
    tmp = frac(tmp + hueOffset);
    
    float3 hsv = float3(tmp, saturation, value);
    float3 rgb = hsv2rgb(hsv);
    return float4(rgb, 1.0);
}

inline float4 rainbowConicalGradation(float2 p, float smooth, float2 range, float saturation, float value) {
    return rainbowConicalGradation(p, smooth, range, saturation, value, 0.0);
}

inline float4 rainbowConicalGradation(float2 p, float angle, float rectAngle, float smooth, float2 offset, float2 range, float saturation, float value, float hueOffset) {
    p = rotate(p, rectAngle); p = rotate(p - offset, angle);
    return rainbowConicalGradation(p, smooth, range, saturation, value, hueOffset);
}

inline float4 rainbowConicalGradation(float2 p, float angle, float rectAngle, float smooth, float2 offset, float2 range, float saturation, float value) {
    return rainbowConicalGradation(p, angle, rectAngle, smooth, offset, range, saturation, value, 0.0);
}

/**
*
*
*/

inline float windingSign(float2 p, float2 a, float2 b) {
    float2 e = b - a;
    float2 w = p - a;

    bool3 cond = bool3(p.y >= a.y,
        p.y < b.y,
        e.x* w.y > e.y * w.x);
    if (all(cond) || all(!(cond))) {
        return -1.0;
    }
    else {
        return 1.0;
    }
}

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
* n: range (cos(theta), sin(theta))
* r: raidus
* th: width
* ru: rounding
*/
inline float sdRing(float2 p, float2 n, float r, float th, float ru)
{
    p.x = abs(p.x);
    float2 t = p;
    p.x = t.x * n.x - t.y * n.y;
    p.y = t.x * n.y + t.y * n.x;
    return round(max(abs(length(t) - r) - th * 0.5, length(float2(p.x, max(0.0, abs(r - p.y) - th * 0.5))) * sign(p.x)), ru);
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

#ifdef SDF_UI_SQUIRCLE
inline float sdSquircle(float2 p, float n, int iteration)
{
    p = abs(p);
    float tmp = p.y > p.x;
    p = tmp * p.yx + (1.0 - tmp) * p;
    n = 2.0 / n;

    float xa = 0.0, xb = 6.283185 / 8.0;
    for (int i = 0; i < iteration; i++)
    {
        float x = 0.5 * (xa + xb);
        float c = cos(x);
        float s = sin(x);
        float cn = pow(c, n);
        float sn = pow(s, n);
        float y = (p.x - cn) * cn * s * s - (p.y - sn) * sn * c * c;

        tmp = y < 0.0;
        xa = tmp * x + (1.0 - tmp) * xa;
        xb = (1.0 - tmp) * x + tmp * xb;
    }

    float2 qa = float2(pow(cos(xa), n), pow(sin(xa), n));
    float2 qb = float2(pow(cos(xb), n), pow(sin(xb), n));
    float2 pa = p - qa, ba = qb - qa;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h) * sign(pa.x * ba.y - pa.y * ba.x);
}
#endif

#ifdef SDF_UI_APPROX_SQUIRCLE
inline float sdApproxSquircle(float2 p, float n)
{
    float tmp = p.y > p.x;
    p = abs(p);
    p = tmp * p.yx + (1.0 - tmp) * p;

    float w = pow(p.x, n) + pow(p.y, n);

    float b = 2.0 * n - 2.0;
    float a = 1.0 - 1.0 / n;
    return (w - pow(w, a)) * rsqrt(pow(p.x, b) + pow(p.y, b));
}
#endif

#ifdef SDF_UI_SPLINE
inline float udSegment(float2 p, float2 a, float2 b) {
    float2 pa = p - a;
    float2 ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h);
}

inline float sdBezier(float2 pos, float2 A, float2 B, float2 C) {

    float2 a = B - A;
    float2 b = A - 2.0 * B + C;
    float2 c = a * 2.0;
    float2 d = A - pos;

    float kk = 1.0 / dot(b, b);
    float kx = kk * dot(a, b);
    float ky = kk * (2.0 * dot(a, a) + dot(d, b)) / 3.0;
    float kz = kk * dot(d, a);

    float res = 0.0;
    float sgn = 0.0;

    float p = ky - kx * kx;
    float q = kx * (2.0 * kx * kx - 3.0 * ky) + kz;
    float p3 = p * p * p;
    float q2 = q * q;
    float h = q2 + 4.0 * p3;

    if (h >= 0.0) { // 1 root
        h = sqrt(h);
        float2 x = (float2(h, -h) - q) / 2.0;

        // When p≈0 and p<0, h - q has catastrophic cancelation. So, we do
        // h=√(q² + 4p³)=q·√(1 + 4p³/q²)=q·√(1 + w) instead. Now we approximate
        // √ by a linear Taylor expansion into h≈q(1 + ½w) so that the q's
        // cancel each other in h - q. Expanding and simplifying further we
        // get x=float2(p³/q, -p³/q - q). And using a second degree Taylor
        // expansion instead: x=float2(k, -k - q) with k=(1 - p³/q²)·p³/q
        if (abs(abs(h / q) - 1.0) < 0.0001) {
            float k = (1.0 - p3 / q2) * p3 / q;  // quadratic approx
            x = float2(k, -k - q);
        }

        float2 uv = sign(x) * pow(abs(x), float2(1.0 / 3.0, 1.0 / 3.0));
        float t = clamp(uv.x + uv.y - kx, 0.0, 1.0);
        float2  q = d + (c + b * t) * t;
        res = dot2(q);
        sgn = cross(c + 2.0 * b * t, q);
    }
    else { // 3 roots
        float z = sqrt(-p);
        float v = acos(q / (p * z * 2.0)) / 3.0;
        float m = cos(v);
        float n = sin(v) * 1.732050808;
        float3  t = clamp(float3(m + m, -n - m, n - m) * z - kx, 0.0, 1.0);
        float2  qx = d + (c + b * t.x) * t.x;
        float dx = dot2(qx), sx = cross(c + 2.0 * b * t.x, qx);
        float2  qy = d + (c + b * t.y) * t.y;
        float dy = dot2(qy);
        float sy = cross(c + 2.0 * b * t.y, qy);
        if (dx < dy) {
            res = dx;
            sgn = sx;
        }
        else {
            res = dy;
            sgn = sy;
        }
    }

    return sqrt(res) * sign(sgn);
}
#endif

#ifdef SDF_UI_PARALLELOGRAM
float sdParallelogram(float2 p, float wi, float he, float sk)
{
    float2 e = float2(sk, he);
    p = (p.y < 0.0) ? -p : p;
    float2  w = p - e; w.x -= clamp(w.x, -wi, wi);
    float2  d = float2(dot(w, w), -w.y);
    float s = p.x * e.y - p.y * e.x;
    p = (s < 0.0) ? -p : p;
    float2  v = p - float2(wi, 0); v -= e * clamp(dot(v, e) / dot(e, e), -1.0, 1.0);
    d = min(d, float2(dot(v, v), wi * he - abs(s)));
    return sqrt(d.x) * sign(-d.y);
}
#endif

#if defined(SDF_UI_OUTLINE_EFFECT_SHINY) || defined(SDF_UI_GRAPHIC_EFFECT_SHINY)
inline float shiny(float2 p, float width, float angle, float blur) {
    float fill = width >= PI; float empty = width == 0;
    if (fill || empty) {
        return 1.0 * fill;
    }

    p = rotate(p, angle);
    p.x = abs(p.x);
    p.y = abs(p.y);

    float2 c = float2(sin(width), cos(width));
    float m = length(p - c * max(dot(p, c), 0.0));
    float dist = m * sign(c.y * p.x - c.x * p.y);

#ifdef SDF_UI_AA
    float delta = fwidth(dist) * .5;
#else
    float delta = 0.0;
#endif

    float isBlur = blur > 0.0;
    float smooth0 = smoothstep(-delta, blur + delta, dist);
    float smooth1 = saturaterange(-delta, delta, dist);

    return 1. - (isBlur * smooth0 + (1. - isBlur) * smooth1);
}
#endif