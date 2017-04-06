int iid=qmitem("")
 _s.getfile("$qm$\qm.ico")
 _qmfile.ResourceAdd(iid "qm.ico" _s _s.len)
 "resource:qm.ico"
 "&resource:qm.ico"
 _s.getfile("$qm$\icons\output.ico")
 _qmfile.ResourceAdd(iid "output.ico" _s _s.len)
 "resource:output.ico"
 "&resource:output.ico"

 _s.getfile("$my qm$\ico\32 2.ico")
 _qmfile.ResourceAdd(iid "test.ico" _s _s.len)
 "resource:test.ico"
 "&resource:test.ico"

 _s.getfile("$qm$\cross_blue.cur")
 "&$qm$\cross_blue.cur"
 _qmfile.ResourceAdd(iid "cross_blue.cur" _s _s.len)
 "resource:cross_blue.cur"
 "&resource:cross_blue.cur"
 "resource:<display images from macro resources - icon, cursor, jpg, png>aero_busy.ani"

 "resource:<Macro2242>test.png"

 _s.getfile("$my qm$\Macro2212.bmp")
 _qmfile.ResourceAdd(iid "test.bmp" _s _s.len)
 "resource:test.bmp"

 _s.getfile("$my qm$\Copy.jpg")
 _qmfile.ResourceAdd(iid "test.jpg" _s _s.len)

 "resource:test.jpg"

 _s.getfile("$my qm$\test\gdip.png")
 _qmfile.ResourceAdd(iid "test.png" _s _s.len)
 "resource:test.png"

 _s.getfile("$my qm$\Macro2197.bmp")
 _qmfile.ResourceAdd(0 "test.bmp" _s _s.len)
 "resource:<>test.bmp"

