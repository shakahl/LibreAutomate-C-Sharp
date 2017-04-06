 /
function $funcName errorCode

str s.all(300)
if(!waveInGetErrorText(errorCode s 300)) s.fix; else s=""
out "%s: error %i, %s" funcName errorCode s
