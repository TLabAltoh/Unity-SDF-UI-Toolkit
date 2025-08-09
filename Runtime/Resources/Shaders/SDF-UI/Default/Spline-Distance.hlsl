/**
* 
* SDF fragment to determin distance from shape (Spline.shader)
* 
*/

//////////////////////////////////////////////////////////////

#ifdef SDF_UI_STEP_SETUP

float dist, onion, tmp;

int idx = 0;

#ifdef SDF_UI_SPLINE_FILL
float winding = 1.;
#endif

#endif  // SDF_UI_STEP_SETUP

//////////////////////////////////////////////////////////////

#if defined(SDF_UI_STEP_SHAPE_AND_OUTLINE) || defined(SDF_UI_STEP_SHADOW)

dist = 3.402823466e+38F;

for (idx = 0; idx < _SplinesNum; idx += 3) {
    float2 v0 = _Splines[idx + 0];
    float2 v1 = _Splines[idx + 1];
    float2 v2 = _Splines[idx + 2];

    tmp = sdBezier(p, v0, v1, v2);

#ifdef SDF_UI_SPLINE_FILL
    if ((tmp > 0.0) == (cross(v1 - v2, v1 - v0) < 0.0)) {
        winding *= windingSign(p, v0, v1);
        winding *= windingSign(p, v1, v2);
    }
    else {
        winding *= windingSign(p, v0, v2);
    }
#endif
    dist = min(dist, abs(tmp));
}

for (idx = 0; idx < _LinesNum; idx += 2) {
    float2 v0 = _Lines[idx + 0];
    float2 v1 = _Lines[idx + 1];

    tmp = udSegment(p, v0, v1);

#ifdef SDF_UI_SPLINE_FILL
    winding *= windingSign(p, v0, v1);
#endif

    dist = min(dist, abs(tmp));
}

#if SDF_UI_SPLINE_FILL
dist *= winding;
#endif

onion = abs(dist) - _OnionWidth;

dist = dist * (1. - _Onion) + onion * _Onion;

dist = round(dist, _Width);

#endif

//////////////////////////////////////////////////////////////