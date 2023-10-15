/***
* This code is adapted and modified from
* https://github.com/kirevdokimov/Unity-UI-Rounded-Corners/blob/master/UiRoundedCorners/SDFUtils.cginc
* https://iquilezles.org/articles/distfunctions2d/
**/

inline float sdRectangle(float2 p, float2 halfSize) {
    // X component represents signed distance to closest vertical edge of rectangle
    // Same for Y but for closest horizontal edge
    // HalfSize represents two distances from each axis of 2d space to a „ƒorresponding edge
    float2 distanceToEdge = abs(p) - halfSize;
    // max(n, 0) to remove distances that signed with minus (distances inside rectangle)
    // length to calculate distance from outside (distances that > 0) to rectangle
    float outsideDistance = length(max(distanceToEdge, 0));
    // max(x,y) is a cheap way to calculate distance to closest edge inside rectangle
    // with min we just make sure that inside distances would not impact on outside distances
    float insideDistance = min(max(distanceToEdge.x, distanceToEdge.y), 0);
    return outsideDistance + insideDistance;
}

inline float sdRoundedBox(float2 p, float2 halfSize, float4 r) {
    r.xy = (p.x > 0.0) ? r.xy : r.zw;
    r.x = (p.y > 0.0) ? r.x : r.y;
    float2 q = abs(p) - halfSize + r.x;
    return min(max(q.x, q.y), 0.0) + length(max(q, 0.0)) - r.x;
}

inline float sdCircle(float2 p, float radius) {
    return length(p) - radius;
}

inline float sdCross(float2 p, float2 halfSize, float r)
{
    p = abs(p);
    p = (p.y > p.x) ? p.yx : p.xy;
    float2 q = p - halfSize;
    float k = max(q.y, q.x);
    float2 w = (k > 0.0) ? q : float2(halfSize.y - p.x, -k);
    return sign(k) * length(max(w, 0.0)) + r;
}

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

inline float round(float d, float r)
{
    return d - r;
}

inline float onion(float2 d, float r)
{
    return abs(d) - r;
}

inline float antialiasedCutoff(float distance) {
    float distanceChange = fwidth(distance) * 0.5;
    return smoothstep(distanceChange, -distanceChange, distance);
}

inline float2 translate(float2 p, float2 offset) {
    return p - offset;
}

inline float2 rotate(float2 p, float rotation) {
    const float PI = 3.14159;
    float angle = rotation * PI * 2 * -1;
    float sine, cosine;
    sincos(angle, sine, cosine);
    return float2(cosine * p.x + sine * p.y, cosine * p.y - sine * p.x);
}

inline float2 reflection(float2 p, float2 refMatrix) {
    return p * refMatrix;
}

inline float closer(float a, float b, float v) {
    int weight = abs(a - v) < abs(b - v);
    return weight * a + (1 - weight) * b;
}