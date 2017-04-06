 /
function index [flags] ;;flags: 1 in slideshow

 Selects slide in PowerPoint.
 Error if failed.
 By default, works in the main PowerPoint window (editor). If flag 1, works in the slideshow window (player).

 index - 1- based slide index.


typelib PowerPoint {91493440-5A91-11CF-8700-00AA0060263B} 2.8

PowerPoint.Application ap._getactive; err end "failed to connect to PowerPoint. Try to run this macro in separate process as User."


if(flags&1) ap.SlideShowWindows.Item(1).View.GotoSlide(index -1)
else ap.ActiveWindow.View.GotoSlide(index)
 
err+ end ES_FAILED
