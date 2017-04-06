out
str s rh
int usefile=16

s="$temp$\qm.exe"
 IntGetFile("http://www.quickmacros.com/quickmac.exe" s usefile 0 1)
 IntGetFile("http://www.quickmacros.com/quickmac.exe" s usefile 0 1 0 0 rh)
 IntGetFile("http://www.quickmacros.com/test/test.txt" s usefile 0 1 0 0 rh)
 IntGetFile("http://www.quickmacros.com/test/test.php" s usefile 0 1 0 0 rh)
IntGetFile("http://www.quickmacros.com/quickmac.exe" s usefile 0 1 &wininet_cb 5 rh)

out "file size: %i" iif(usefile GetFileOrFolderSize(s) 0L+s.len)
out "responseheaders:[]%s" rh
