 /
function ARRAY(VARIANT)&a col [flags] ;;flags: 1 descending, 2 exclude first row

 Sorts 2-dim ARRAY(VARIANT).

 a - array variable. Not error if empty.
 col - column index (index in first dimension).

 EXAMPLE
 ARRAY(VARIANT) a.create(3 10) ;;3 columns, 10 rows
  ... (populate)
 SortVariantArray2Dim a 1 ;;sort ascending by second column


if(!a.len) ret
if(a.ndim!=2) end "must be 2-dim array"
int lb1=a.lbound(1)
if(col>a.ubound(1) or col<lb1) end "invalid column index"

int- t_sortArrCol
t_sortArrCol=col-lb1
int sizeofrow=a.len(1)*sizeof(VARIANT)
byte* ptr=a.psa.pvData
int nrows=a.len
if(flags&2) ptr+sizeofrow; nrows-1; if(nrows<1) ret
qsort ptr nrows sizeofrow iif(flags&1 &__SortVarArrDesc &__SortVarArrAsc)
