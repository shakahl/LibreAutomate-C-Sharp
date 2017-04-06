 /
 Allows starting macro only in QM window.

function# iid FILTER&f

if(wintest(f.hwnd "" "QM_Editor")) ret iid
