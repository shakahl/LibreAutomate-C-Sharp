out

DateTime t1 t2
str sStart sEnd sTime
t1.FromComputerTime
sStart=t1.ToStr(8)

#region macro
wait RandomNumber
#endregion

t2.FromComputerTime
sEnd=t2.ToStr(8)

sTime=TimeSpanToStr(t2-t1 2)

out sStart
out sEnd
out sTime
