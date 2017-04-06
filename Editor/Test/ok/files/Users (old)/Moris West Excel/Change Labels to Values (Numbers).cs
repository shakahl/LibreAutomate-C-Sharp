 if(mes("Change labels and formulas to values?" "Excel" "OC?a")!='O') ret

 ExcelSheet es.Init
 Excel.Range ra=es.ws.Application.Selection 
 ARRAY(VARIANT) a
 a=ra.Value
  a[1 a.redim(-1)]="4"
  int i j
  out a.len
  a[1 1]=4
 ra.Value=a

 invalid interface pointer

str vbs=
 set app = GetObject(,"Excel.Application")
 set ra = app.Selection
 a=ra.Value
 ra.Value=a
VbsExec vbs
