out
typelib EnvDTE {80CC9F66-E7D8-4DDD-85B6-D9E6CD0E93E2} 7.0 0 1

EnvDTE.DTE dte
if(!GetWindowThreadProcessId(win("" "" "devenv") &_i)) ret
dte._getactive(0 0 _s.from("!VisualStudio.DTE.7.1:" _i))

ARRAY(EnvDTE.Window) rgDocuments
EnvDTE.Window w
foreach w dte.Windows
	_s=w.Kind; if(!(_s="Document")) continue
	out w.Document.Name
	rgDocuments[]=w
 out rgDocuments.len

out "--- sort ---"
rgDocuments.sort(0 dtesort)

int i
for i 0 rgDocuments.len
	out rgDocuments[i].Document.Name
