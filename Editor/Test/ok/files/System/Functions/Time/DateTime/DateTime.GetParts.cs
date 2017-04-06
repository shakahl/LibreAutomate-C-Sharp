function [int&year] [int&month] [int&day] [int&hour] [int&minute] [int&second] [int&ms] [double&micros] [int&weekday]

 Gets date/time parts.

 year and other parameters - variables for parts. Can be 0.


SYSTEMTIME s
if(!FileTimeToSystemTime(+&t &s)) end ERR_FAILED

if(&year) year=s.wYear
if(&month) month=s.wMonth
if(&day) day=s.wDay
if(&hour) hour=s.wHour
if(&minute) minute=s.wMinute
if(&second) second=s.wSecond
if(&ms) ms=s.wMilliseconds
if(&micros) micros=t%10000; micros/10
if(&weekday) weekday=s.wDayOfWeek
