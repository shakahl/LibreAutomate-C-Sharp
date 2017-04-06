 /
function ARRAY(str)&a [slide]

 Gets text from a PowerPoint slide.
 Error if fails.
 On Vista/7 must run in separate process as User. Set it in macro Properties dialog.
 Tested with MS Office 2003. Should work with other versions too.

 a - variable that receives text. Each array element will contain text of a text box.
 slide - 1-based slide index. If omitted or 0, uses current slide.


a=0
typelib PowerPoint {91493440-5A91-11CF-8700-00AA0060263B} 2.8

PowerPoint.Application ap._getactive; err end "failed to connect to PowerPoint. Try to run this macro in separate process as User."

PowerPoint.Slide d
if(slide<1) d=ap.ActiveWindow.View.Slide
else d=ap.ActivePresentation.Slides.Item(slide); err end "incorrect slide index"

PowerPoint.Shape h
foreach h d.Shapes
	if(!h.HasTextFrame) continue
	if(!h.TextFrame.HasText) continue
	a[]=h.TextFrame.TextRange.Text

err+ end _error
