;; Return the elapsed time in milliseconds since last called, or zero the first time called
;; Optionally pass the previous time to calculate the elapsed time from
;; - if passed, the previous time variable is updated to the current time, by default
;; Call once to initialise
function'long [long&previousTime] [DoNotUpdatePreviousTime]
;; previousTime is a reference
 msg &previousTime ;; Zero if optional parameter not passed
long+ elaspedmsPrevTime
long elapsed

long now = GetTickCount ;; milliseconds +/- 10
 now=perf ;; microseconds

 & for reference that holds address, but references value
 Make t reference "previousTime" parameter if specified, else "elaspedmsPrevTime"
long& t=iif(&previousTime previousTime elaspedmsPrevTime)

if t=0; t=now ;; Happens the first time called
elapsed=now-t

;; Not sure when I64 is required?
 out "%s %I64i %I64i %i" "elapsedms:" elapsed t now

if DoNotUpdatePreviousTime=0
	t=now ;; update "previous time" variable
ret elapsed
