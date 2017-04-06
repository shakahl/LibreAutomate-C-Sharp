 Select a function declaration in QmDll, and run this macro.
 It copies html and processes it to be pasted in qm help source.

spe -1
str s
men 33354 id(2213 _hwndqm) ;;Copy Html
s.getclip
if(numlines(s)>10) mes- "failed to select function declaration"

FuncHelpFomat s
s.setclip
