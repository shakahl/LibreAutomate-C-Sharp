 /
function# [flags] ;;flags: 1 in slideshow

 Returns 1-based index of current slide in PowerPoint.
 Error if failed.
 By default, works in the main PowerPoint window (editor). If flag 1, works in the slideshow window (player).


typelib PowerPoint {91493440-5A91-11CF-8700-00AA0060263B} 2.8

PowerPoint.Application ap._getactive; err end "failed to connect to PowerPoint. Try to run this macro in separate process as User."

PowerPoint.Slide d
if(flags&1) d=ap.SlideShowWindows.Item(1).View.Slide
else d=ap.ActiveWindow.View.Slide

ret d.SlideIndex
err+ end ES_FAILED
