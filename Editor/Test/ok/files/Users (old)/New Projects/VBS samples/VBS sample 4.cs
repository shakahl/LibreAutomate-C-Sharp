IDispatch wa._create("Word.Application")

str code=
 Sub ShowWordApp(wa)
 wa.Visible=True
 End Sub

VbsAddCode code
VbsFunc "ShowWordApp" wa

1
wa.Quit
