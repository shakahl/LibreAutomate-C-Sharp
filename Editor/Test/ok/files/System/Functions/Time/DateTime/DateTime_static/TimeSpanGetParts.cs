 /
function long'timeSpan [int&days] [int&hours] [int&minutes] [int&seconds] [int&ms] [double&micros]

 Gets time span parts.

 timeSpan - time span.
 days and other parameters - variables for parts. Can be 0.

 REMARKS
 days receives total value. Other variables receive values in part bounds. For example, hours can be from -23 to 23.
 If timeSpan is negative, nonzero parts also will be negative.

 Added in: QM 2.3.2.


if(&days) days=timeSpan/DT_DAY
if(&hours) hours=timeSpan/DT_HOUR%24
if(&minutes) minutes=timeSpan/DT_MINUTE%60
if(&seconds) seconds=timeSpan/DT_SECOND%60
if(&ms) ms=timeSpan/DT_MS%1000
if(&micros) micros=timeSpan%10000; micros/10
