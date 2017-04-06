 /
function` isTrue

 Returns VARIANT of type boolean (VT_BOOL).

 isTrue - if 0, the boolean value will be False, else True.

 REMARKS
 Use with COM functions, where argument type is boolean stored in VARIANT.
 QM does not have a boolean variable type. This function converts from int to COM boolean.
 Don't use this function when argument is passed not through VARIANT. Then pass 0 for False, -1 for true (or 1 with some functions).
 Even when argument is passed not through VARIANT, most COM functions work even if you simply pass 0 or -1 (or 1), not VariantBool.

 Added in: QM 2.3.5.

 EXAMPLES
 TypeLib.CoClass x._create
 x.Method(a b VariantBool(1) c)

 ExcelSheet es.Init
 es.ws.Range("A1").Value=VariantBool(1)
 if(es.ws.Range("A1").Value) out "True"; else out "False"


VARIANT r.vt=VT_BOOL
r.boolVal=iif(isTrue -1 0)
ret r
