str files=
 $system$/notepad.exe
 $system$/calc.exe

str warnings
zip "$temp$\EZ_AddFiles.zip" files 0 warnings
if(warnings.len) out warnings; ret
ret 1
