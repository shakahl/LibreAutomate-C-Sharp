typelib Word {00020905-0000-0000-C000-000000000046} 8.3 1

str code=
 set wa=CreateObject("Word.Application")
 wa.Documents.Add()

VbsExec code

Word.Application wa=VbsEval("wa")
wa.Visible=TRUE
wa.Activate
