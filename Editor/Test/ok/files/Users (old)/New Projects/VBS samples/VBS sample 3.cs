typelib Word {00020905-0000-0000-C000-000000000046} 8.3 1

Word.Application wa._create

str code=
 Sub ShowWordApp(wa)
 wa.Visible=True
 End Sub

VbsAddCode code
VbsFunc "ShowWordApp" wa

1
wa.Quit
