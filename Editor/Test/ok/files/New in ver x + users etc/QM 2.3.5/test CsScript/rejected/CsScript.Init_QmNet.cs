

if(x) ret

int+ g_csscript_init
if(!g_csscript_init)
#if !EXE
	 __ComActivator_CreateManifest "QmNet.dll" 1|2
	__ComActivator_CreateManifest "QmNet.dll" 1
#endif
	 If cs-script suite installed, error when trying to compile code from memory (ok from file): "Compiler executable file csc.exe cannot be found".
	 Workaround: remove this env var from registry or in this process before using cs-script library.
	SetEnvVar("CSSCRIPT_DIR" "" 1)

m_ca.Activate("QmNet.X.manifest" 1)
 m_ca.Activate("QmNet.dll,2")

x._create("QmNet.CsScript")

if(!g_csscript_init)
	x.RedirectConsoleOutput(0+&CsScript_OutCallback) ;;redirect script Console.Write[Line] for lifetime of this process
	g_csscript_init=1
