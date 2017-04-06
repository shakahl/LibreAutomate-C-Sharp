SetProp(_hwndqm "qmtc_debug_output" 0)

int w=win("Quick Macros - ok - [Macro1820]" "QM_Editor")
w=id(2050 w)

 WindowText wt.Init(w)
 Acc a=wt.GetAcc(wt.Find("H" 0x1000))
 out a.Name

 WindowText wt.Init(w)
 WTI* t=wt.Find("H" 0x1000)
 out t.txt

WindowText wt.Init(w)
Acc a=wt.GetAcc(wt.Find("H" 0x1000))

TO_VN