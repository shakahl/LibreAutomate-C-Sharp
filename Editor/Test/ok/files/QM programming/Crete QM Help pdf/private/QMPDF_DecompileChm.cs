 /Create QM Help pdf
function $qmHelpFile $htmlDir

 Decompiles qmHelpFile to htmlDir.


del- htmlDir; err
mkdir htmlDir
str f.dospath(htmlDir)
str c.dospath(qmHelpFile)
run "hh.exe" F"-decompile {f} {c}" "" "" 0x400
