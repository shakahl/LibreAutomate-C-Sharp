/exe 1
act "Word"
 #opt dispatch 1
 typelib Word {00020905-0000-0000-C000-000000000046} 8.5
typelib Word {00020905-0000-0000-C000-000000000046} 8.0
Word.Application app._getactive
app.Activate
key CH ;;Ctrl+Home

app.Selection.Find.ClearFormatting()
app.Selection.Find.Text = "find me"
app.Selection.Find.Replacement.ClearFormatting()
app.Selection.Find.Replacement.Text = "Found"
VARIANT vReplace=Word.wdReplaceAll
app.Selection.Find.Execute(@ @ @ @ @ @ @ @ @ @ vReplace)


 BEGIN PROJECT
 main_function  Macro1961
 exe_file  $my qm$\Macro1961.qmm
 flags  6
 guid  {9B9056FA-AA80-4230-B32F-D7CFE96B0668}
 END PROJECT
