function color [style] ;;style: 0 transparent, 1 filled alpha

 Sets color and style of on-screen rectangle.

 color - color in 0xBBGGRR format.

 REMARKS
 Call this before first Show, or after Show(2). Optional.


if(__brush) DeleteObject __brush; __brush=0
if(color) __brush=CreateSolidBrush(color)
__style=style
