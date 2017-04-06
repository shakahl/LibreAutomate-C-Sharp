 /
function# [flags] [$path] [$clAppend] ;;flags: 1 make console visible, 2 don't wait until ready

 Runs Selenium Server on this computer.
 Returns Selenium Server console window handle.
 Error if fails.

 path - full path of Selenium server .jar file.
   The filename part can contain wildcard characters.
   Example: "Q:\Downloads\selenium-server-standalone-*.jar".
   Default: "$qm$\Selenium\selenium-server-standalone-*.jar".
 clAppend - a string to append to the command line.
   The function also appends -Dwebdriver.chrome.driver=... if finds chromedriver.exe in the same folder. The same for IEDriverServer.exe.

 REMARKS
 Must be installed Java, at least version 1.5.
 If Selenium Server is already running and was started by this function, this function restarts it.
 Waits until the server is ready.


opt noerrorshere 1

str cl sp
if(empty(path)) path="$qm$\Selenium\selenium-server-standalone-*.jar"
sp.expandpath(path)
lpstr fn=dir(sp)
if(!fn) end F"{ERR_FILE}: {sp}"
sp.getpath
cl.from("-jar " sp fn)

if !empty(clAppend)
	cl+" "; cl+clAppend
	if(findrx(clAppend "-port +(\d+)" 0 1 _s 1)>=0) int port=val(_s)
if(FileExists(_s.from(sp "chromedriver.exe"))) cl.formata(" -Dwebdriver.chrome.driver=%s" _s)
if(FileExists(_s.from(sp "IEDriverServer.exe"))) cl.formata(" -Dwebdriver.ie.driver=%s" _s)

int w=Selenium_FindServer; if(w) clo w; err

__Handle hp=run("java.exe" cl "" "" iif(flags&1 0 16))
int pid=ProcessHandleToId(hp); if(!pid) end ERR_FAILED
w=wait(30 WC win("" "ConsoleWindowClass" pid))
_s="Selenium Server"; _s.setwintext(w)

if flags&2=0
	WinHttp.WinHttpRequest r._create
	rep 10
		1
		if !WaitForSingleObject(hp 0) ;;on error server exits after <0.2 s
			end F"{ERR_FAILED}. To debug, try to run from cmd.exe:[][9]java.exe {cl}[][9]"
		r.Open("GET" F"http://localhost:{iif(port port 4444)}/wd/hub")
		r.Send; err continue
		r.WaitForResponse
		break

ret w
