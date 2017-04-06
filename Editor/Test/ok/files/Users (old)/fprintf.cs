File f.Open("$my qm$\my out.txt" "w")
fprintf f "%s[9]" "1"
fprintf f "%i[9]" 2
fprintf f "%g[]" 3.45

 f.SetPos(0)
 f.ReadToStr(_s -1)
 out _s
