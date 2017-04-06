 /
function $chmFile $destFolder

 EXAMPLE
 DecompileChmFile "$qm$\qm2help.chm" "$desktop$\qm2help"


mkdir destFolder

str f.dospath(destFolder)
str c.dospath(chmFile)
str cl.format("-decompile %s %s" f c)
run "hh.exe" cl "" "" 0x400
