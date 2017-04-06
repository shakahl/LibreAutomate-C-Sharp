 /Insert Rich Text
function# hwndctrl str'texttofind


def EM_FINDTEXTW (WM_USER + 123)
def FR_DOWN          0x1

TEXTRANGE* tr=share
TEXTRANGE* tr2=share(hwndctrl)
memset(tr 0 sizeof(TEXTRANGE))
int stringoffset=sizeof(TEXTRANGE)+20
lpstr txt = tr+stringoffset
lpstr txt2 = tr2+stringoffset

tr.chrg.cpMin = 0
tr.chrg.cpMax = -1
texttofind.unicode
memcpy(txt texttofind texttofind.len+2);; copy the string into shared space

tr.lpstrText = txt2;; address needs to be in nonowned control's space
int result=SendMessage(hwndctrl EM_FINDTEXTW FR_DOWN tr2)
out result
ret result