 rep
	 IntGetFile "http://www.quickmacros.com/index.html" _s
	 1
 out _s

SendMail "support@quickmacros.com" "sssssssss" "*TEST CRASH*"

 SendMail "support@quickmacros.com" "sssssssss" "ttttttttttt" 0 "$desktop$\Desktop Background.bmp"
 5
 out ReceiveMail("<default>" 0 "$desktop$\test")
