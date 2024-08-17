/**
* Output result step of SDF fragment
*/

effects *= i.color;

#ifdef UNITY_UI_CLIP_RECT
effects.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
#endif

#ifdef UNITY_UI_ALPHACLIP
clip(effects.a - 0.001 > 0.0 ? 1 : -1);
#endif

return effects;