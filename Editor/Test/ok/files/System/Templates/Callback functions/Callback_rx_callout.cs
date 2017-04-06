 /
function[c]# CALLOUT&x

 Callout callback function for findrx or str.replacerx.
 More info in QM Help.

 Return:
   0 - matching proceeds as normal.
   > 0 - matching fails at the current point, but backtracking to test other possibilities goes ahead, just as if a lookahead assertion had failed.
   -1 - matching fails (not found).
   < -100 generate error with this error number.
