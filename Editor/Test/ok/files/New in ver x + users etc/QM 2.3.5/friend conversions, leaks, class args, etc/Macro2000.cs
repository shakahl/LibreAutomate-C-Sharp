 dll oleaut32 [SafeArrayCreate]ARRAY(byte)SafeArrayCreate2 @vt cDims SAFEARRAYBOUND*rgsabound
 
 SAFEARRAYBOUND b.cElements=5
 ARRAY(byte) a=SafeArrayCreate2(VT_UI1 1 &b)
 out a.len
 out a.psa.cbElements
