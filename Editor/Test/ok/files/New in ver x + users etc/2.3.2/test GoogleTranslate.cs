 out
str t r
t=GoogleTranslate("test translation" "lt" "" 0 r)
if(t) out t
else out r
 out GoogleTranslate("birhs lanhuage" "lt")
 out GoogleTranslate("Šeštadienis" "en")
 out GoogleTranslate("test" "lt" "en" 1)
 out GoogleTranslate("battyto" "lt")
 out GoogleTranslate("test ''this''" "lt")
 out GoogleTranslate("one[]\two/[]''three''" "lt")
 out GoogleTranslate("test > < ''this''" "lt")
 out GoogleTranslate("" "lt")
 str s.all(127 2); for(_i 0 s.len) s[_i]=_i+1
 out s
 out GoogleTranslate(s "lt")
 out GoogleTranslate(s "lt" "" 1)
 out GoogleTranslate("one[9]tw'o" "lt")
