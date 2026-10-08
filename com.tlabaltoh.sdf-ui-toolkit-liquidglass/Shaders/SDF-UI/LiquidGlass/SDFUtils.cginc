/***
* This code is adapted and modified from
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/SDFUtils.cginc
* https://iquilezles.org/articles/distfunctions2d/
* https://www.shadertoy.com/view/7stcR4
**/

#define PI 3.14159265358979323846

/***
*
* Selects between two float values based on a condition.
*
*/

inline float select(bool boolean, float a0, float a1) {
    return boolean ? a0 : a1;
}

inline float2 select(bool boolean, float2 a0, float2 a1) {
    return boolean ? a0 : a1;
}

inline float3 select(bool boolean, float3 a0, float3 a1) {
    return boolean ? a0 : a1;
}

inline float4 select(bool boolean, float4 a0, float4 a1) {
    return boolean ? a0 : a1;
}

/***
*
* Selects between three float values based on a condition.
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

/***
*
* Selects between four float values based on a condition.
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

/***
*
* Miscellaneous utility functions.
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
    if (b == a) {
        if (x <= a) {
            return 0.0;
        }
        else {
            return 1.0;
        }
    }
    return saturate((x - a) / (b - a));
}

inline float4 saturaterange(float4 a, float4 b, float4 x) {
    float4 result;
    result.a = saturaterange(a.a, b.a, x.a);
    result.r = saturaterange(a.r, b.r, x.r);
    result.g = saturaterange(a.g, b.g, x.g);
    result.b = saturaterange(a.b, b.b, x.b);
    return result;
}

inline float dot2(float2 v) {
    return dot(v, v);
}

inline float cross(float2 a, float2 b) {
    return a.x * b.y - a.y * b.x;
}

inline float2 rotate(float2 pos, float theta) {
    return float2(pos.x * cos(theta) - pos.y * sin(theta), pos.x * sin(theta) + pos.y * cos(theta));
}

/***
*
* Color gradient processing function
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
*
* HSV to RGB conversion
*
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
*
* Rainbow gradient functions
*
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
/***
* p: position
* h: height
*/
inline float sdRectangle(float2 p, float2 h) {
    float2 distanceToEdge = abs(p) - h;
    float outsideDistance = length(max(distanceToEdge, 0));
    float insideDistance = min(max(distanceToEdge.x, distanceToEdge.y), 0);
    return outsideDistance + insideDistance;
}

/***
* p: position
* h: height
* r: radius (x: top right, y: bottom right, z: top left, w: bottom left)
*/
inline float sdRoundedBox(float2 p, float2 b, float4 r) {
    r.xy = (p.x > 0.0) ? r.xy : r.zw;
    r.x = (p.y > 0.0) ? r.x : r.y;
    float2 q = abs(p) - b + r.x;
    return min(max(q.x, q.y), 0.0) + length(max(q, 0.0)) - r.x;
}
#endif

#ifdef SDF_UI_CIRCLE
/***
* p: position
* r: radius
*/
inline float sdCircle(float2 p, float r) {
    return length(p) - r;
}
#endif

#ifdef SDF_UI_PIE
/***
* p: position
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
/***
* p: position
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

#ifdef SDF_UI_VESICA
/***
* p: position
* w: width
* h: height
*/
inline float sdVesica(float2 p, float w, float h)
{
    p = abs(p);

    if (w > h)
    {
        float d = 0.5 * (w * w - h * h) / h;
        float3 c = (w * p.y < d* (p.x - w)) ? float3(0.0, w, 0.0) : float3(-d, 0.0, d + h);
        return length(p - c.yx) - c.z;
    }
    else
    {
        float d = 0.5 * (h * h - w * w) / w;
        float3 c = (h * p.x < d* (p.y - h)) ? float3(0.0, h, 0.0) : float3(-d, 0.0, d + w);
        return length(p - c.xy) - c.z;
    }
}
#endif

#ifdef SDF_UI_MOON
/***
* p: position
* d: distance
* ra: radius A
* rb: radius B
*/
inline float sdMoon(float2 p, float d, float ra, float rb)
{
    if (abs(d) < 0.00001)
    {
        return max(length(p) - ra, -(length(p) - rb));
    }

    p.y = abs(p.y);
    float a = (ra * ra - rb * rb + d * d) / (2.0 * d);
    float b = sqrt(max(ra * ra - a * a, 0.0));
    if (d * (p.x * b - p.y * a) > d * d * max(b - p.y, 0.0))
        return length(p - float2(a, b));
    return max((length(p) - ra), -(length(p - float2(d, 0)) - rb));
}
#endif

#ifdef SDF_UI_EGG
/***
* p: position (pixel coordinates to evaluate)
* he: height (distance between the centers of the bottom and top circles)
* ra: radius A (radius of the bottom circle)
* rb: radius B (radius of the top circle)
* bu: bulge (bulge factor of the sides, must be between >0.0 and <=1.0)
*/
inline float sdEgg(float2 p, float he, float ra, float rb, float bu)
{
    // all this can be precomputed for any given shape
    float r = 0.5 * (he + ra + rb) / bu;
    float da = r - ra;
    float db = r - rb;
    float y = (db * db - da * da - he * he) / (2.0 * he);
    float x = sqrt(da * da - y * y);

    // only this needs to be run per pixel
    p.x = abs(p.x);
    float k = p.y * x - p.x * y;
    if (k > 0.0 && k < he * (p.x + x)) {
        return length(p + float2(x, y)) - r;
    }
    return min(length(p) - ra, length(float2(p.x, p.y - he)) - rb);
}
#endif

#ifdef SDF_UI_ELLIPSE
/***
* p: position
* ab: (width, height)
*/
float sdEllipse(float2 p, float2 ab)
{
    if (ab.x <= 0.00001 || ab.y <= 0.00001) return length(p);

    float k1 = length(p / ab);
    float k2 = length(p / (ab * ab));

    return k1 * (k1 - 1.0) / k2;
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

/**
* 
* Liquid Glass
* 
*/

// Thickness is the t in the doc.
float3 getNormal(float sd, float thickness)
{
    float dx = ddx(sd);
    float dy = ddy(sd);

    // The cosine and sine between normal and the xy plane.
    float n_cos = max(thickness + sd, 0.0) / thickness;
    float n_sin = sqrt(1.0 - n_cos * n_cos);

    return normalize(float3(dx * n_cos, dy * n_cos, n_sin));
}

// The height (z component) of the pad surface at sd.
float height(float sd, float thickness)
{
    if (sd >= 0.0)
    {
        return 0.0;
    }
    if (sd < -thickness)
    {
        return thickness;
    }

    float x = thickness + sd;
    return sqrt(thickness * thickness - x * x);
}

/**
* 1-pass-blur (The 1-pass blur algorithm is heavy process, so plan to replace it with a 2-pass algorithm in the futhre.
* However in order to implement 2-pass gaussian blur, it is necessary to generate GrabTexture off-screen, and it may 
* only be possible to implement this with a custom SRP (which is likely to be hard to implement), so it is likely to 
* take a log time).
*/

fixed4 blur(float4 uv, float blur, float sigma) {
      float weight_total=0, const_v0 = 2.0 * sigma * sigma;
      fixed4 col = fixed4(0,0,0,0);

      blur = max(1, blur);

      [loop]
      for (float x = -blur; x <= blur; x++) {
        [loop]
        for (float y = -blur; y <= blur; y++) {
          float distance_normalized = dot(float2(x,y), float2(y,x));
          float weight = 1.0 / (PI * const_v0) * exp(-(x * x + y * y) / const_v0);
          weight_total += weight;
          col += tex2D(_GrabTexture, uv.xy + float2(x, y) * _GrabTexture_TexelSize.xy) * weight;
        }
      }
      return col / weight_total;
}

/**
*
* Boolean
*
*/

struct Surface {
    float sd;
    float4 color;
};

Surface opSmoothUnion(Surface a, Surface b, float k) {
    float h = clamp(0.5 + 0.5 * (b.sd - a.sd) / k, 0.0, 1.0);
    Surface res;
    res.sd = lerp(b.sd, a.sd, h) - k * h * (1.0 - h);
    res.color = lerp(b.color, a.color, h); // GLSL: mix(b.color, a.color, h)
    return res;
}

Surface opSmoothSubtract(Surface base, Surface cutter, float k) {
    float h = clamp(0.5 - 0.5 * (base.sd + cutter.sd) / k, 0.0, 1.0);
    Surface res;
    res.sd = lerp(base.sd, -cutter.sd, h) + k * h * (1.0 - h);
    res.color = lerp(base.color, cutter.color, h);
    return res;
}

Surface opSmoothIntersection(Surface a, Surface b, float k) {
    float h = clamp(0.5 - 0.5 * (b.sd - a.sd) / k, 0.0, 1.0);
    Surface res;
    res.sd = lerp(b.sd, a.sd, h) + k * h * (1.0 - h);
    res.color = lerp(b.color, a.color, h);
    return res;
}

struct SdfOp
{
    int shape;
    int boolOp;
    float onion;
    float boolSmooth;
    float2x2 invTransform;
    float4 position;
    float4 parameters;
    float4 color;
};

SdfOp LoadSdfOp(Texture2D<float4> opTex, int index)
{
    SdfOp op;

    int texY = index;
    int startX = 0;

    float4 data0 = opTex.Load(int3(startX + 0, texY, 0));
    op.shape = asint(data0.x);
    op.boolOp = asint(data0.y);
    op.onion = data0.z;
    op.boolSmooth = data0.w;

    op.invTransform = opTex.Load(int3(startX + 1, texY, 0));
    op.position = opTex.Load(int3(startX + 2, texY, 0));
    op.parameters = opTex.Load(int3(startX + 3, texY, 0));
    op.color = opTex.Load(int3(startX + 4, texY, 0));

    return op;
}

#define SDFSHAPE_CIRCLE        0
#define SDFSHAPE_ARC           1
#define SDFSHAPE_TRIANGLE      2
#define SDFSHAPE_QUAD          3
#define SDFSHAPE_PARALLELOGRAM 4
#define SDFSHAPE_VESICA        5
#define SDFSHAPE_MOON          6
#define SDFSHAPE_EGG           7
#define SDFSHAPE_ELLIPSE       8

#define SDFBOOLOP_UNION        0
#define SDFBOOLOP_SUBTRACT     1
#define SDFBOOLOP_INTERSECT    2

Surface EvaluateSdfOp(SdfOp op, float2 p) {

    Surface result;
    float dist, e_onion;
    float2 e_corner0, e_corner1, e_corner2, e_position, e_cossin;

    e_onion = op.onion > 1.;
    e_position = mul(op.invTransform, p - op.position.xy);

    switch (op.shape) {
    case SDFSHAPE_CIRCLE:
        /***
        * x: Radius
        * y: None
        * z: None
        * w: None
        */
        dist = length(e_position) - op.parameters.x;
        dist = dist * (1. - e_onion) + (abs(dist) - op.onion) * e_onion;
        break;

#ifdef SDF_UI_ARC
    case SDFSHAPE_ARC:
        /***
        * x: Theta
        * y: Radius
        * z: Width
        * w: CircleBorder
        *
        * position.z: CornerRounding
        */
        // Since 'parameters' (float4) alone cannot store all the data required to draw the Arc,
        // the remaining parameters are exceptionally packed into position.z.
        if (op.parameters.x >= PI) {
            dist = abs(length(e_position) - op.parameters.y) - op.parameters.w;
        }
        else {
            e_cossin = float2(cos(op.parameters.x), sin(op.parameters.x));
            dist = sdRing(e_position, e_cossin, op.parameters.y, op.parameters.z, op.position.z);
        }
        dist = dist * (1. - e_onion) + (abs(dist) - op.onion) * e_onion;
        break;
#endif

#ifdef SDF_UI_TRIANGLE
    case SDFSHAPE_TRIANGLE:
        /***
        * x: Base of a triangle
        * y: Height of a triangle
        * z: Roundness
        * w: AnchorY
        */
        e_corner0 = float2(+op.parameters.x * 0.5, op.parameters.w);
        e_corner1 = float2(-op.parameters.x * 0.5, op.parameters.w);
        e_corner2 = float2(0.0, -op.parameters.y + op.parameters.w);
        dist = sdTriangle(e_position, e_corner0.xy, e_corner1.xy, e_corner2.xy);
        dist = dist * (1. - e_onion) + (abs(dist) - op.onion) * e_onion;
        dist = round(dist, op.parameters.z);
        break;
#endif

#ifdef SDF_UI_QUAD
    case SDFSHAPE_QUAD:
        /***
        * x: Top right corner radius
        * y: Bottom right corner radius
        * z: Top left corner radius
        * w: Bottom left corner radius
        *
        * position.z: Width
        * position.w: Height
        */
        // Since 'parameters' (float4) alone cannot store all the data required to draw the Quad,
        // the remaining parameters are exceptionally packed into position.z and position.w.
        dist = sdRoundedBox(e_position, op.position.zw, op.parameters);
        dist = dist * (1. - e_onion) + (abs(dist) - op.onion) * e_onion;
        break;
#endif

#ifdef SDF_UI_PARALLELOGRAM
    case SDFSHAPE_PARALLELOGRAM:
        /***
        * x: Width
        * y: Height
        * z: Slide
        * w: Roundness
        */
        dist = sdParallelogram(e_position, op.parameters.x - abs(op.parameters.z) - op.parameters.w, op.parameters.y - op.parameters.w, op.parameters.z);
        dist = dist * (1. - e_onion) + (abs(dist) - op.onion) * e_onion;
        dist = round(dist, op.parameters.w);
        break;
#endif

#ifdef SDF_UI_VESICA
    case SDFSHAPE_VESICA:
        /***
        * x: Width
        * y: Height
        * z: Roundness
        * w: None
        */
        dist = sdVesica(e_position, op.parameters.x, op.parameters.y);
        dist = dist * (1. - e_onion) + (abs(dist) - op.onion) * e_onion;
        dist = round(dist, op.parameters.z);
        break;
#endif

#ifdef SDF_UI_MOON
    case SDFSHAPE_MOON:
        /***
        * x: Radius A
        * y: Radius B
        * z: Slide
        * w: Roundness
        */
        dist = sdMoon(e_position, op.parameters.z, op.parameters.x, op.parameters.y);
        dist = dist * (1. - e_onion) + (abs(dist) - op.onion) * e_onion;
        dist = round(dist, op.parameters.w);
        break;
#endif

#ifdef SDF_UI_EGG
    case SDFSHAPE_EGG:
        /***
        * x: Bluge
        * y: Height
        * z: Radius A
        * w: Radius B
        */
        dist = sdEgg(e_position, op.parameters.y, op.parameters.z, op.parameters.w, op.parameters.x);
        dist = dist * (1. - e_onion) + (abs(dist) - op.onion) * e_onion;
        break;
#endif

#ifdef SDF_UI_ELLIPSE
    case SDFSHAPE_ELLIPSE:
        /***
        * x: Width
        * y: Height
        * z: None
        * w: None
        */
        dist = sdEllipse(e_position, op.parameters.xy);
        dist = dist * (1. - e_onion) + (abs(dist) - op.onion) * e_onion;
        break;
#endif
    }
    result.sd = dist;
    result.color = op.color;

    return result;
}