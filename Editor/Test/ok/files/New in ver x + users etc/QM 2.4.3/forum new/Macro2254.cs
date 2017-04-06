 /exe 1
typelib Word {00020905-0000-0000-C000-000000000046} 8.0
Word.Application a._getactive ;;connect to Word. On Vista/7, macro process must run as User. QM normally runs as Admin. The /exe 1 tells QM to run the macro in separate process, as User.
 a.Run("Macro1")
Word.Find f=a.Selection.Find
f.ClearFormatting
f.Replacement.ClearFormatting
f.Text="x1"
f.Replacement.Text = "test"
f.Forward = TRUE
f.Wrap = wdFindContinue
f.Format = 0
 ...
f.Execute

 BEGIN PROJECT
 main_function  Macro2254
 exe_file  $my qm$\Macro2254.qmm
 flags  6
 guid  {B52B1A11-6D00-4A77-8AF6-1C6498133915}
 END PROJECT

    Sub Macro1()
    '
    ' Macro1 Macro
    ' Macro recorded 27.04.2014 by Sergiu
    '
        Selection.Find.ClearFormatting
        Selection.Find.Replacement.ClearFormatting
        With Selection.Find
            .Text = "x1"
            .Replacement.Text = "test"
            .Forward = True
            .Wrap = wdFindContinue
            .Format = False
            .MatchCase = False
            .MatchWholeWord = False
            .MatchWildcards = False
            .MatchSoundsLike = False
            .MatchAllWordForms = False
        End With
        Selection.Find.Execute
    End Sub


