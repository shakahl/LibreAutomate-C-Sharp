 /
function long'timeSpan [long&days] [long&hours] [long&minutes] [long&seconds] [long&ms] [long&micros]

 Gets total number of days, hours, etc in time span.

 timeSpan - time span.
 days and other parameters - variables. Can be 0.

 Added in: QM 2.3.2.


if(&days) days=timeSpan/DT_DAY
if(&hours) hours=timeSpan/DT_HOUR
if(&minutes) minutes=timeSpan/DT_MINUTE
if(&seconds) seconds=timeSpan/DT_SECOND
if(&ms) ms=timeSpan/DT_MS
if(&micros) micros=timeSpan/DT_MCS
