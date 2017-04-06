 /
 Allows starting macro only in QM window AND only if Caps Lock is on.

function# iid FILTER&f

iid=FF_A(iid f); if(iid<=0) ret iid

 if need more filter functions:
 iid=FF_C(iid f); if(iid<=0) ret iid
 iid=FF_D(iid f); if(iid<=0) ret iid
 ...

ret FF_B(iid f)
