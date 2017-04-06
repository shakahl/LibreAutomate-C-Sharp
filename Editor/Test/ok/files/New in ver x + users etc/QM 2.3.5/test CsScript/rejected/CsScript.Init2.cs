
 Initializes CsScript engine. Loads CSScriptLibrary.dll etc.
 Optional. Other functions implicitly call this function if still not initialized.


if(x) ret

int+ g_csscript_init
#if !EXE
if !g_csscript_init
	__ComActivator_CreateManifest "CSScriptLibrary.dll" 1
	 __ComActivator_CreateManifest "CSScriptLibrary.dll" 1|2
#endif

m_ca.Activate("CSScriptLibrary.X.manifest" 1)
 m_ca.Activate("CSScriptLibrary.dll,2" 1)

x._create("CSScriptLibrary.QmCsScript")

if !g_csscript_init
	x.RedirectConsoleOutput(0+&CsScript_OutCallback) ;;redirect script Console.Write[Line] for lifetime of this process. Also Trace.WriteX, if TRACE defined.
	x.SetPerfNextCallback(0+&CsScript_PerfNextCallback) ;;allow .NET code call our PerfNext to measure speed of parts of .NET code
	g_csscript_init=1
