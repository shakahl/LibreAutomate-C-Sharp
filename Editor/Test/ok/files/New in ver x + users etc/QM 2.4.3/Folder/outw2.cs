 /
function hwnd [$prefix] [str&sOut]

 Displays window handle, class and text (first line) in QM output.

 sOut (QM 2.3.5) - if used, stores text in the variable, else displays in QM output.

 Added in: QM 2.3.2.


if(!hwnd) out "%s0" prefix; ret
if(!IsWindow(hwnd)) out "%s%i <invalid handle>" prefix hwnd; ret
str sc.getwinclass(hwnd); err
str st.getwintext(hwnd); err
st.trim; st.getl(st 0); st.LimitLen(100 1)
str se.getwinexe(hwnd)
int col; if(!IsWindowVisible(hwnd)) col=0x808080; else if(IsWindowCloaked(hwnd)) col=0x8080
str s=F"<><c 0x{col}><_>{prefix}{hwnd}  {sc}</_></c>  <c 0x8000><_>''{st}''</_></c>  <c 0xff0000>{se}</c>"
if(&sOut) sOut=s; else out s
