 MessageBeep MB_ICONASTERISK
 bee 4
 bee "$windows$\Media\Garden\Windows Battery Low.wav" ;;no event

PlaySound("ChangeTheme" 0 SND_ALIAS|SND_ASYNC|SND_SYSTEM)
 PlaySound("Explorer\BlockedPopup" 0 SND_ALIAS|SND_APPLICATION|SND_ASYNC|SND_SYSTEM) ;;does not work
 PlaySound(_s.expandpath("$windows$\Media\Garden\Windows Battery Low.wav") 0 SND_ASYNC) ;;no event
