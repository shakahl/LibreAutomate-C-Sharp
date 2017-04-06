 /
function# hwndTB [&rangeMin] [&rangeMax]

 Returns trackbar control thumb position and optionally gets more info.


if(&rangeMin) rangeMin=SendMessage(hwndTB TBM_GETRANGEMIN 0 0)
if(&rangeMax) rangeMax=SendMessage(hwndTB TBM_GETRANGEMAX 0 0)
ret SendMessage(hwndTB TBM_GETPOS 0 0)
