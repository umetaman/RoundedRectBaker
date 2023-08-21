void RoundedRectDistance_float(float2 centerPosition, float2 rectSize, float radius, float margin, out float distance)
{
	distance = length(max(abs(centerPosition) - rectSize + radius, 0.0)) - radius + margin;
}