typelib Office {2DF8D04C-5BFA-101B-BDE5-00AA0044DE52} 2.2 ;;the mso constants are here
typelib PowerPoint {91493440-5A91-11CF-8700-00AA0060263B} 2.8

str strPath.expandpath("$my qm$\")
PowerPoint.Application app._getactive
PowerPoint.Presentation p=app.ActivePresentation
int SlideCount = p.Slides.Count
out SlideCount
str strTemp="copy.jpg"
PowerPoint.Slide oSld = p.slides.Add(SlideCount, ppLayoutBlank)
PowerPoint.Shape oPic=oSld.Shapes.AddPicture(F"{strPath}{strTemp}" msoFalse msoTrue 0 0 0 0)
