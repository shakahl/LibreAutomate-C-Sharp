int hwndTB=id(1001 win("" "Volume Control"))
int rmin rmax
GetTrackbarInfo hwndTB rmin rmax ;;get range
SetTrackbarPos hwndTB rmin+rmax/2 ;;set middle value
bee
1
SetTrackbarPos hwndTB rmin ;;set min value (max sound)
bee
