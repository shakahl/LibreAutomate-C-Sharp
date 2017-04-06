 /
function $text [rate] ;;rate: -10 to 10.

 Speaks text.
 You can use hot keys to control it. Shows them when called first time.


AddTrayIcon "sound.ico"

int+ __speak_sel_info2
str si=
 Hot keys:
 
 Left - repeat current sentence.
 Right - next sentence.
 Up - previous sentence.
 Down - pause/resume.
 End - end.
 Home - restart.
if(!__speak_sel_info2) __speak_sel_info2=1; OnScreenDisplay si -1 -1 -1 "" 8 0 4

str s=text
if(!s.len) ret
s.findreplace("..." ".")
s.replacerx("\[\d+\]")
int paused

typelib __SpeechLib {C866CA3A-32F7-11D2-9602-00C04F8EE628} 5.0

__SpeechLib.SpVoice spv._create

spv.Rate=rate
if(_winnt>=6) waveOutSetVolume 0 0xffffffff ;;on Vista, this sets max wave out volume for qm only, because may be 0 initially

spv.Speak(s 1)

__RegisterHotKey k1.Register(0 1 0 VK_LEFT)
__RegisterHotKey k2.Register(0 2 0 VK_RIGHT)
__RegisterHotKey k3.Register(0 3 0 VK_UP)
__RegisterHotKey k4.Register(0 4 0 VK_DOWN)
__RegisterHotKey k5.Register(0 5 0 VK_END)
__RegisterHotKey k6.Register(0 6 0 VK_HOME)
SetTimer(0 1 100 0)

MSG m
rep
	if(GetMessage(&m 0 0 0)<1) break
	
	sel m.message
		case WM_HOTKEY
		err-
		
		if m.wParam<=3
			if(!paused) spv.Pause; 2 ;;make some silence
			paused=0; spv.Resume
		
		sel m.wParam
			case 1 spv.Skip("Sentence" 0)
			case 2 spv.Skip("Sentence" 1)
			case 3 spv.Skip("Sentence" -1)
			case 4 paused^1; if(paused) spv.Pause; else spv.Resume
			case 5 break
			case 6 spv.Speak(s 3)
		err+ bee
		
		case WM_TIMER
		if(spv.Status.RunningState=SRSEDone) break
