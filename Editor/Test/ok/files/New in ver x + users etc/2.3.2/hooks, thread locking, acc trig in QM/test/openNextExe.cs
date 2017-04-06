function'int $exeName $launch [$params] [runFlags]
;; Global vars to keep track of which window to activate when there are multiple for one executable.
str+ prevExeName ;; exeName from previous call
if (exeName == "ResetPrevExeName")
	prevExeName=""
int+ prevExeCount ;; exeCount from previous call to allow the next of multiple windows to be found

int exeCount exeInd
int validWinCount ;; Count of windows that could be activated (excludes desktop, toolbars, etc)
ARRAY(int) a
int i w
str sc sn se sn1
int found=1

if(empty(exeName)) ret

;; Reset exeName if long delay since last called
def nextExeWindowExpiryMS 5000 ;; millisecs during which repeat calls to openNextExe will activate the next window in the list
long+ openNextExe_PreviousRuntime
if (elapsedms(openNextExe_PreviousRuntime) > nextExeWindowExpiryMS); prevExeName=""

;; Get a list of all windows
win("" "" "" 0x400 0 0 a)
int dbg=0
if(dbg) out "openNextExe %s: a.len=%i" exeName a.len

;; Setup exe name and count to keep track of multiple windows
if (prevExeName != exeName)
	prevExeName = exeName
	prevExeCount = 0
;; out "\nprevExeCount=%i" prevExeCount

for(i 0 a.len)
	sc.getwinclass(a[i])
	sn.getwintext(a[i])
	se.getwinexe(a[i])
	if(dbg) out "ALL: %i %i %i '%s' '%s' '%s'" i validWinCount a[i] sc se sn
	
	;; Skip some windows
	if(sn="") continue ;; Skip windows with no title
	if(matchw(sn1 _s.from("Microsoft Excel - " sn) 1)) continue ;; Skip the second instance of an Excel 2007 window when it was the top window
	;; Skip some window classes. Win7: Button
	if findrx(sc "progman|QM_toolbar|Button" 0 1) >= 0; continue 

	validWinCount+1 ;;
	;; Map some long exe names to short names
	if (matchw(se "Foxit Reader" 1)); se="FoxitR~1"
	
	;; Does this window match?
	if (matchw(se exeName 1))
		if (dbg); out "Mtc: %i %i %i '%s' '%s' '%s'" i validWinCount a[i] sc se sn
		;; exe name matches
		exeInd=i ;; Keep track of last place matching window found
		exeCount+1 ;; Count the number of matches we find
		if (validWinCount=1) 
			;; The desired executable is the top window, so skip past it
			sn1=sn ;; record title when a matching window is already current (for Excel)
			continue

		;; We've found a matching window after the first window
		;; Activate it if we've reached an exe window after the ones recently shown
		if (exeCount > prevExeCount)
			w=a[i]
			 SetForegroundWindow w
			act w;err
			if (min(w))
				err
					out "%s: min win failed" fnName
				res w
				err
					out "%s: res win failed" fnName
			prevExeCount=exeCount
			ret found

;; All windows processed 
;; Activate the last matching window found
if exeCount
	w=a[exeInd]
	 SetForegroundWindow w  ;; act a[exeInd]
	act w;err
	if (min(w)); res w
	prevExeCount=exeCount
	ret found
else
	if !empty(launch)
		_s.format("Starting:[]%s: %s %s" exeName launch params)
		OnScreenDisplay _s 2 0 -50 0 0 0 4 fnName ;; Display at bottom middle, 4: allow click to close + non transparent
		run launch params "" "" 0x100+0x200+runFlags;; 100:ignore errors, 200: wait for input ready, 0x30000:Run as administrator if QM is running as administrator.
		err
			OsdHide fnName
			msg "Not found:" launch params
			ret 0
	else
		_s.format("No windows found: %s" exeName)
		OnScreenDisplay _s 1 0 -50 0 0 0 4 fnName ;; Display at bottom middle, 4: allow click to close + non transparent
		ret 0
