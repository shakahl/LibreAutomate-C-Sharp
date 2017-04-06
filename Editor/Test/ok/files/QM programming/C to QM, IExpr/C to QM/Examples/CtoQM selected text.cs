 Takes selected text that contains C declarations and replaces it with QM declarations.
 Note: The results will not include declarations that are in winapi.txt (WINAPI) and winapiqm_other.txt.

str s.getsel; if(!s.len) ret
 OnScreenDisplay "Please wait..." 1
ConvertCtoQM2 s
if(s.len) s.setsel
else mes "There are no C declarations in the selected text, or failed to convert, or all these declarations are included in winapiqm.txt (WINAPI) and winapiqm_other.txt" "" "i"
