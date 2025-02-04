/**
* 
* SDF fragment to determin distance from shape (Arc.shader)
* 
*/

//////////////////////////////////////////////////////////////

#ifdef SDF_UI_STEP_SETUP

float dist, radius, onion;
float2 cossin, t;

#endif  // SDF_UI_STEP_SETUP

//////////////////////////////////////////////////////////////

#if defined(SDF_UI_STEP_SHAPE_AND_OUTLINE) || defined(SDF_UI_STEP_SHADOW)

t = p;
p.x = t.x * _AngleOffset.x - t.y * _AngleOffset.y;
p.y = t.x * _AngleOffset.y + t.y * _AngleOffset.x;

if (_Theta >= 3.14) {
    dist = abs(length(p) - _Radius) - _CircleBorder;
}
else {
    cossin = float2(cos(_Theta), sin(_Theta));
    dist = sdRing(p, cossin, _Radius, _Width, _CornersRounding);
}

onion = abs(dist) - _OnionWidth;

dist = dist * (1. - _Onion) + onion * _Onion;

#endif

//////////////////////////////////////////////////////////////