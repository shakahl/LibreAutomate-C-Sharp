out
 GetActiveDTE
 #if 0

typelib EnvDTE {80CC9F66-E7D8-4DDD-85B6-D9E6CD0E93E2} 7.0
 typelib EnvDTE80 {1A31287A-4D7D-413E-8E32-3B374931BD89} 8.0 ;;does not have Document interface, and fails to retrieve from dte80a.olb
 typelib EnvDTE {80CC9F66-E7D8-4DDD-85B6-D9E6CD0E93E2} 8.0


 EnvDTE.DTE dte=GetActiveDTE
 EnvDTE80.DTE2 dte=GetActiveDTE(win("Visual Studio"))
 if( !dte ) ret

 EnvDTE80.DTE2 dte._getactive(0 16 "!VisualStudio.DTE.8.0:4444")

if(!GetWindowThreadProcessId(win("" "" "devenv") &_i)) ret
 EnvDTE.DTE dte._getactive(0 0 _s.from("!VisualStudio.DTE.8.0:" _i))
EnvDTE.DTE dte._getactive(0 0 _s.from("!VisualStudio.DTE.7.1:" _i))

mes dte.ActiveDocument.FullName
 mes dte.ActiveDocument
 EnvDTE80.D
