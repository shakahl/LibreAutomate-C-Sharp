 /
function'long [long'days] [long'hours] [long'minutes] [long'seconds] [long'ms] [^micros]

 Creates time span from parts.
 Does not validate arguments. For example, 100 hours adds 4 days and 4 hours.

 Added in: QM 2.3.2.


long mc=micros*10
ret (days*DT_DAY)+(hours*DT_HOUR)+(minutes*DT_MINUTE)+(seconds*DT_SECOND)+(ms*DT_MS)+mc
