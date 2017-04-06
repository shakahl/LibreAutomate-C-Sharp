function'str hDlg

int NumberOfReps Seconds Minutes Hours
str SetTime

spe 1
rep
	SetTime = ""
	Seconds = NumberOfReps%60
	Minutes = NumberOfReps/60%60
	Hours = NumberOfReps/3600
	SetTime.formata("%02i H : %02i M : %02i S" Hours Minutes Seconds)
	SetTime.setwintext(id(4 hDlg))
	1
	NumberOfReps+1