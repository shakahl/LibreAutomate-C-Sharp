 /exe 1
typelib Word {00020905-0000-0000-C000-000000000046} 8.0
Word.Application a._getactive ;;connect to Word. On Vista/7, macro process must run as User. QM normally runs as Admin. The /exe 1 tells QM to run the macro in separate process, as User.
Word.Range r

 get all text
r=a.ActiveDocument.Range
str sAllText=r.Text
out sAllText

 get selected text
r=a.Selection.Range
str sSelText=r.Text
out sSelText

 BEGIN PROJECT
 main_function
 END PROJECT
