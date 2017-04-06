 /
function# $className

WNDCLASSEXW c.cbSize=sizeof(c)
ret GetClassInfoExW(0 @className &c)
