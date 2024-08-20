/**
* SDF fragment to determin distance from shape (Spline.shader)
*/

//////////////////////////////////////////////////////////////

#ifdef SDF_UI_STEP_SETUP

#ifdef SDF_UI_AA_SUBPIXEL
float4 dist, tmp;
float r, g, b;
#else
float dist, tmp;
#endif

int idx = 0;
float2x2 j;

#ifdef SDF_UI_SPLINE_FILL
float winding = 1.;
#endif

#endif  // SDF_UI_STEP_SETUP

//////////////////////////////////////////////////////////////

#if defined(SDF_UI_STEP_SHAPE_OUTLINE) || defined(SDF_UI_STEP_SHADOW)

dist = 3.402823466e+38F;

j = JACOBIAN(p);

for (idx = 0; idx < (_Num - 2); idx += 2) {
    float2 v0 = _Controls[idx + 0];
    float2 v1 = _Controls[idx + 1];
    float2 v2 = _Controls[idx + 2];

    // Handle cases where points coincide
    bool abEqual = !all(v0 - v1);
    bool bcEqual = !all(v1 - v2);
    bool acEqual = !all(v0 - v2);

    if (abEqual && bcEqual) {
        tmp = distanceAA(p, j, v0);
    }
    else if (abEqual || acEqual) {
        tmp = udSegmentAA(p, j, v1, v2);

#ifdef SDF_UI_SPLINE_FILL
        winding *= windingSign(p, v1, v2);
#endif
    }
    else if (bcEqual) {
        tmp = udSegmentAA(p, j, v0, v2);

#ifdef SDF_UI_SPLINE_FILL
        winding *= windingSign(p, v0, v2);
#endif
    }
    else {
        tmp = sdBezierAA(p, j, v0, v1, v2);

#ifdef SDF_UI_SPLINE_FILL
#ifdef SDF_UI_AA_SUBPIXEL
        if ((tmp.x > 0.0 || tmp.y > 0.0) == (cro(v1 - v2, v1 - v0) < 0.0)) {
#else
        if ((tmp > 0.0) == (cro(v1 - v2, v1 - v0) < 0.0)) {
#endif
            winding *= windingSign(p, v0, v1);
            winding *= windingSign(p, v1, v2);
        }
        else {
            winding *= windingSign(p, v0, v2);
        }
#endif
    }

    dist = min(dist, abs(tmp));
}

if (idx < _Num - 1) {
    float2 v0 = _Controls[idx + 0];
    float2 v1 = _Controls[idx + 1];

    tmp = udSegmentAA(p, j, v0, v1);

#ifdef SDF_UI_SPLINE_FILL
    winding *= windingSign(p, v0, v1);
#endif

    dist = min(dist, tmp);
}

#if SDF_UI_SPLINE_FILL
dist *= winding;
#endif

#ifdef SDF_UI_ONION
dist = abs(dist) - _OnionWidth;
#endif

dist = round(dist, _Width);

#endif

//////////////////////////////////////////////////////////////