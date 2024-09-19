/**
* Output result step of SDF fragment
*/

i.color.rgb *= i.color.a;
effects *= i.color;

#ifdef UNITY_UI_CLIP_RECT
//effects.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
//effects *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(i.mask.xy)) * i.mask.zw);
effects *= (m.x * m.y);
#endif

#ifdef UNITY_UI_ALPHACLIP
clip(effects.a - 0.001 > 0.0 ? 1 : -1);
#endif

return effects;