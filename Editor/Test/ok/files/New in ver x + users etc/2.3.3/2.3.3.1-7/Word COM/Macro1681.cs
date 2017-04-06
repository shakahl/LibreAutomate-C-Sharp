 /exe 1
str vbs=
 '
 ' TitleCapitalization Macro
 '
 '
     Set a=GetObject(,"Word.Application")
     a.Selection.HomeKey wdLine
     a.Selection.EndKey wdLine, wdExtend
     a.Selection.Range.Case = wdTitleWord
VbsExec vbs

 BEGIN PROJECT
 main_function
 END PROJECT
