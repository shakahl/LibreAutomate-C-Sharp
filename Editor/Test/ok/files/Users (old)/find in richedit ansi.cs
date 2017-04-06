 /Insert Rich Text
function# hwndctrl str'texttofind


def FR_DOWN          0x1

TEXTRANGE* tr=share
TEXTRANGE* tr2=share(hwndctrl)
memset(tr 0 sizeof(TEXTRANGE))
int stringoffset=sizeof(TEXTRANGE)+20
lpstr txt = tr+stringoffset
lpstr txt2 = tr2+stringoffset

tr.chrg.cpMin = 0
tr.chrg.cpMax = -1
strcpy(txt texttofind);; copy the string into shared space

tr.lpstrText = txt2;; address needs to be in nonowned control's space
int result=SendMessage(hwndctrl EM_FINDTEXT FR_DOWN tr2)
out result
ret result