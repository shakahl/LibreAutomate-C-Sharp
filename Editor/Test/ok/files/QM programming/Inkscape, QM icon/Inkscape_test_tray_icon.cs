 int w=win("icon.svg - Inkscape" "gdkWindowToplevel")
 act w
 key Cs; 0.5

str svgFile="$qm$\icons\icon.svg"
str icoFile="$qm$\icons\qm-icon.ico"
int+ icoVar; icoVar+1
icoFile.insert(F" ({icoVar})" icoFile.len-4)
svg_2_ico svgFile icoFile "16[]20[]24[]32[]48[]256"
 svg_2_ico svgFile icoFile "16"

AddTrayIcon icoFile icoFile
wait -1

 info: need 20/24 icons in DPI 125/150%.
