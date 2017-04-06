 /
function !on

 Turns on or off the "Filter keys" Windows accessibility feature.
 Does not change its parameters. You can change it in Control Panel.

 on - 1 on, 0 off.


FILTERKEYS f.cbSize=sizeof(f)
SystemParametersInfo SPI_GETFILTERKEYS 0 &f 0
if on
	if(f.dwFlags&FKF_FILTERKEYSON) ret
	f.dwFlags|FKF_FILTERKEYSON
else
	if(f.dwFlags&FKF_FILTERKEYSON=0) ret
	f.dwFlags~FKF_FILTERKEYSON
SystemParametersInfo SPI_SETFILTERKEYS 0 &f 0
