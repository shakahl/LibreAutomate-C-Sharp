 #ret

int+ g_isMainPC
#set g_isMainPC IsMainPC

category tools : ">\System\Tools\private" ">\System\Tools\Windows and controls\private"
 category dlg : ">\System\Dialogs" ">\System\Dialogs\Dialog Functions\private"

def USE_INFO_CONTROL 1

dll "qm.exe"
	[PerfFirst]PF
	[PerfNext]PN
	[PerfOut]PO [flags] [str*sOut]
	__Test

dll kernel32 #IsDebuggerPresent

SetEnvVar "app" iif(_qmdir.begi("c:\program") "Q:\app" "$qm$")
SetEnvVar "com" iif(_winnt<6 "F:\Tools, source, ocx\components" "Q:\Tools, source, ocx\components")
SetEnvVar "downloads" "Q:\Downloads"
SetEnvVar "doc" "Q:\Doc"
SetEnvVar "backup" "E:"
SetEnvVar "VM" "Q:\VM"
SetEnvVar "host" "\\Q7C"
SetEnvVar "catkeys" _s.expandpath(iif(_qmdir~"Q:\app\" "$qm$\catkeys" "$desktop$\catkeys"))
#if _winver>=0x0501
str cd
if(!IsDebuggerPresent and GetCdDrivePath(cd)) SetEnvVar "cd" cd
#endif

def GINDI_TEST 1

 typelib Word {00020905-0000-0000-C000-000000000046} 8.3 1

ref WINAPI2 "$qm$\winapi2.txt" 7 ;;http://www.quickmacros.com/forum/viewtopic.php?f=2&t=1736
 ref WINAPI2 "$qm$\winapi2.txt" 1 ;;with the above declaration, programming is easier because don't need 'WINAPI2.' prefix. However then in forum I post code that does not work for those that don't have WINAPI2 declared in this way.

 test_declare_many_types

 if(_winver=0x601)
	 mac "Keyboard_Detector"

SetProp(_hwndqm "qmtc_debug_output" 1)

 out "note: forum and email notifications are disabled"
 #ret

 InitSplitter

 #ret
if(g_isMainPC and !IsDebuggerPresent)
 if(g_isMainPC)
	mac "SpamFilter"
	mac "RssNotifier"
	mac "timer_fix_Windows_bugs_etc"

def _DEBUG_HELP 1

 typelib WinHttp {662901FC-6951-4854-9EB2-D9A2570F2B2E} 5.1

#compile "____Tcc2"

 ref __pcre2 "$qm$\pcre2.txt" 1
#compile "__Regex"
