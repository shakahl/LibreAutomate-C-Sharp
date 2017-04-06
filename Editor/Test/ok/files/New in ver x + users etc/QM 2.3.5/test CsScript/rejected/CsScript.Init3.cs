
 Initializes CsScript engine. Loads CSScriptLibrary.dll etc.
 Optional. Other functions implicitly call this function if still not initialized.


if(x) ret

int+ ___CsScript_init

m_ca.Activate("CSScriptLibrary.dll,2" 1)

x._create("CSScriptLibrary.QmCsScript")

if !___CsScript_init
	x.RedirectConsoleOutput(0+&CsScript_OutCallback) ;;redirect script Console.Write[Line] for lifetime of this process. Also Trace.WriteX, if TRACE defined.
	x.SetPerfNextCallback(0+&CsScript_PerfNextCallback) ;;allow .NET code call our PerfNext to measure speed of parts of .NET code
	___CsScript_init=1

err+ end _error
