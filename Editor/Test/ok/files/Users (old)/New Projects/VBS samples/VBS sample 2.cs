str code=
 set wa=CreateObject("Word.Application")

VbsExec code

IDispatch wa=VbsEval("wa")
wa.Visible=TRUE
1
wa.Quit
