 /
function# flags $x $v1 [$v2] [$v3] [$v4] [$v5] [$v6] [$v7] [$v8] [$v9] [$v10] [$v11] [$v12] [$v13] [$v14] [$v15] [$v16] [$v17] [$v18] [$v19] [$v20] [$v21] [$v22] [$v23] [$v24] [$v25] [$v26] [$v27] [$v28] [$v29] ;;flags: 1 insens, 2 wildcard, 4 beginning, 8 end, 16 middle, 32 rx

 Compares string with several strings, and returns 1-based index of matching string or 0.
 If x matches v1, returns 1. If x matches v2, returns 2. And so on.
 Returns 0 if x does not match any string.

 flags:
    1 - case insensitive.
    2 - <help #IDP_WILDCARD>wildcard</help> match.
    4 - must match beginning.
    8 - must match end.
    16 - must match anywhere (find).
    32 - regular expression match.
    If flags 2-32 are not used, must match whole.
    Flags 2-32 cannot be used together.

 See also: <sel>.
 Added in: QM 2.3.2.

 EXAMPLE
 str x="September"
 int i=SelStr(1|4 x "jan" "feb" "mar" "apr" "may" "jun" "jul" "aug" "sep" "oct" "nov" "dec")
 out i ;;9


ret __SelStr(((getopt(nargs)-2<<16)|(flags&0xffff)) x +&v1)
