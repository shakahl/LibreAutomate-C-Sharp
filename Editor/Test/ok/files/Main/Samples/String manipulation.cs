 run Notepad and type some text
run "notepad.exe" "" "" "" 0x2800 "Notepad"
key "soMe teXt" YY  ;; "soMe teXt" Enter Enter
1 ;;wait 1 s

 create variables
str s s2 s3 s4 ;;4 string variables
int i ;;integer variable
ARRAY(str) a ;;array of strings

 select text and split into words
key A{ea} ;; Alt+{E A}  (select all)
s.getsel ;;store selected text into variable s
key CE ;; Ctrl+End  (move text cursor to the end)
tok s a ;;split, and store words into array a

 repeat for each word
for i 0 a.len
	s2.left(a[i] 1) ;;get first character into s2
	s3.get(a[i] 1) ;;get remaining part into s3
	s2.ucase ;;make uppercase
	s3.lcase ;;make lowercase
	s4.from(s2 s3) ;;join
	s4.setsel ;;paste
	key Y ;; Enter
