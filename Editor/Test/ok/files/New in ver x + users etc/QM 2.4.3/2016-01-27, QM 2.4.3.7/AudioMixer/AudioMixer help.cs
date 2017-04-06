 Gets or sets audio device volume or mute state.
 Uses WINAPI2: http://www.quickmacros.com/forum/viewtopic.php?f=2&t=1736
 Does not work on Windows XP.

 EXAMPLES

#compile "__AudioMixer"
AudioMixer x
sel ListDialog("Get volume (%)[]Set volume 50%[]Set volume 100%[]Get mute[]Mute[]Unmute")
	case 1 out x.GetVolume
	case 2 x.SetVolume(50)
	case 3 x.SetVolume(100)
	case 4 out x.GetMute
	case 5 x.SetMute(1)
	case 6 x.SetMute(0)
