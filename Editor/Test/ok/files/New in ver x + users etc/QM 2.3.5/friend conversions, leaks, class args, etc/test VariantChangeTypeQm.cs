dll "qm.exe" #VariantChangeTypeQm2 VARIANT*dest VARIANT*src vt
out

DATE d
DateTime dt
VARIANT v vv

 dt.FromComputerTime
 v=dt
  out v.vt
 out VariantChangeTypeQm2(&vv &v VT_DATE)
 out vv.vt
 d=vv
 out d

d.getclock
v=d
 out v.vt
out VariantChangeTypeQm2(&vv &v VT_I8)
out vv.vt
dt.t=vv
out dt.ToStr(4)
