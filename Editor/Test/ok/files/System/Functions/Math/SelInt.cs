 /
function# x v1 [v2] [v3] [v4] [v5] [v6] [v7] [v8] [v9] [v10] [v11] [v12] [v13] [v14] [v15] [v16] [v17] [v18] [v19] [v20] [v21] [v22] [v23] [v24] [v25] [v26] [v27] [v28] [v29] [v30]

 Compares number with several numbers, and returns 1-based index of matching number or 0.
 If x = v1, returns 1. If x = v2, returns 2. And so on.
 Returns 0 if x is not match any number.

 Added in: QM 2.3.2.

 EXAMPLE
 int x=4
 int i=SelInt(x 2 4 6)
 out i ;;2


int i
int* p=&x
for i 1 getopt(nargs)
	if(p[i]=x) ret i
