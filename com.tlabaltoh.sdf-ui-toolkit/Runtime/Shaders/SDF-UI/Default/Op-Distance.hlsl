/**
* 
* SDF fragment to determin distance from shape (Op.shader)
* 
*/

//////////////////////////////////////////////////////////////

#ifdef SDF_UI_STEP_SETUP

float2 e_corner0, e_corner1, e_corner2, e_position, e_cossin;
float dist, onion;
Surface shapeA, shapeB;
SdfOp op;

#endif  // SDF_UI_STEP_SETUP

//////////////////////////////////////////////////////////////

#if defined(SDF_UI_STEP_SHAPE_AND_OUTLINE) || defined(SDF_UI_STEP_SHADOW)

op = LoadSdfOp(_OpTex, 0);
shapeA = EvaluateSdfOp(op, p);

for (int idx = 1; idx < _ElemCount; idx++) {
	op = LoadSdfOp(_OpTex, idx);
	shapeB = EvaluateSdfOp(op, p);

	switch (op.boolOp) {
	case SDFBOOLOP_UNION:
		shapeA = opSmoothUnion(shapeA, shapeB, op.boolSmooth);
		break;
	case SDFBOOLOP_SUBTRACT:
		shapeA = opSmoothSubtract(shapeA, shapeB, op.boolSmooth);
		break;
	case SDFBOOLOP_INTERSECT:
		shapeA = opSmoothIntersection(shapeA, shapeB, op.boolSmooth);
		break;
	}
}

// Final result
dist = shapeA.sd;
color = color * shapeA.color;

// When outlineWidth = 0, _OutlineColor is set to m_fillColor. 
// Therefore, SdfOp must determine whether _OutlineColor also needs to be updated when updating color.
if (_OutlineWidth == 0) {
	_OutlineColor = color;
}

onion = abs(dist) - _OnionWidth;
dist = dist * (1. - _Onion) + onion * _Onion;

#endif

//////////////////////////////////////////////////////////////