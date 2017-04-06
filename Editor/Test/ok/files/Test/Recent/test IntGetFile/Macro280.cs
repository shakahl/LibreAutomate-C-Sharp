out
str s
 IntGetFile "http://www.quickmacros.com/quickm21.exe" s 0 0 1
 IntGetFile "http://www.quickmacros.com/index.html" s 0 0 1
 out s

s="$desktop$\index2.html"
IntGetFile("http://www.quickmacros.com/index.html" s 16 0 1)
 IntGetFile("http://www.quickmacros.com/quickm21.exe" s 0 0 1 &Function39 55)

s="$desktop$\qmsetup.exe"
 IntGetFile("http://www.quickmacros.com/quickm21.exe" s 16 0 1)
 IntGetFile("http://www.quickmacros.com/quickm21.exe" s 16 0 1 &Function39 55)
 IntGetFile("http://www.quickmacros.com/index.html" s 16 0 1 &Function39 55)
