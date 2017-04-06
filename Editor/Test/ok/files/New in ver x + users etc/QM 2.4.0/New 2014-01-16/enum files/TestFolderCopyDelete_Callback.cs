function[c] param $sFrom $sTo !isFolder

out "%i  '%s'  '%s'  %i" param sFrom sTo isFolder
if(isFolder) ret 2
 if sTo
	 sel sFrom 3
		 case "*.qml" ret 2
		 case "*\admin" ret 2
ret 1
