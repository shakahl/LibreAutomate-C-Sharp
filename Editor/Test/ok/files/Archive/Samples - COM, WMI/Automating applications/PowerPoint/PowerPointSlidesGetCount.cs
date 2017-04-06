 /
function#

 Returns number of slides in PowerPoint.
 Error if failed.


typelib PowerPoint {91493440-5A91-11CF-8700-00AA0060263B} 2.8

PowerPoint.Application ap._getactive; err end "failed to connect to PowerPoint. Try to run this macro in separate process as User."

ret ap.ActivePresentation.Slides.Count

err+ end ES_FAILED
