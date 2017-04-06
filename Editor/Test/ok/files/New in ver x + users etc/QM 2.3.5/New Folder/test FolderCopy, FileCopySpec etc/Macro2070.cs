str from.expandpath("$desktop$\A")
str to.expandpath("$desktop$\test")
 str to.expandpath("$desktop$\test\A\B")
SHFILEOPSTRUCT x.wFunc=FO_COPY
x.pFrom=from
x.pTo=to
x.fFlags=FOF_NOCONFIRMMKDIR|FOF_NOCONFIRMATION|FOF_NO_CONNECTED_ELEMENTS|FOF_NOCOPYSECURITYATTRIBS
out SHFileOperation(&x)
