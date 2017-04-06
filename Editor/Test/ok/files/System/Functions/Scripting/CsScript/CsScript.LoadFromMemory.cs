function !*data dataLength

 Loads assembly from memory.

 data - assembly (dll) file data.
 dataLength - data size, bytes.

 REMARKS
 Same as <help>CsScript.Load</help>, but loads assembly from memory instead of file.

 Errors: <.>


opt noerrorshere 1
opt nowarningshere 1

Init

RECT r.bottom=VT_UI1
SAFEARRAY sa.cbElements=1; sa.cDims=1
sa.fFeatures=FADF_HAVEVARTYPE|FADF_FIXEDSIZE
sa.rgsabound[0].cElements=dataLength
sa.pvData=data
x.LoadFromMemory(&sa)
