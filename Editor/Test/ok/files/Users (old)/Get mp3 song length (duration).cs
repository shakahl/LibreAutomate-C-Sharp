str s="Q:\mp3\Soma Sonic\06 - road to nowhere.mp3"

 str s="Q:\mp3\x.mp3"

typelib WMPLib {6BF52A50-394A-11D3-B153-00C04F79FAA6} 1.0
WMPLib.WindowsMediaPlayer w._create
WMPLib.IWMPMedia m=w.newMedia(s)
out m.duration ;;seconds
out m.durationString
