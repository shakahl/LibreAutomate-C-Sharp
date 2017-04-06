out
 PsCl "-help"
 PsCl "get-date"
 PsCl "-command &''get-date''"
 PsFile "$documents$\Untitled 1.ps1"
 PsFile "$documents$\Untitled 1.ps1" "" "-OutputFormat XML" _s; out _s.len
 PsCmd "get-date"
 PsCmd "get-date" "-OutputFormat XML"
PsCmd "get-process" "" _s; out _s
 PsCmd "get-date[]get-date;get-date"
 PsCmd "Get-EventLog -LogName system" "-WindowStyle Maximized"
