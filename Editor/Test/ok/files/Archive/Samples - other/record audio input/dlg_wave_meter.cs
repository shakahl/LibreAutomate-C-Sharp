\Dialog_Editor

 Records audio input (eg microphone) and displays level.

function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_wave_meter" &dlg_wave_meter 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 31 "Wave In Level"
 3 Static 0x54000000 0x0 2 2 32 12 "Peak"
 4 msctls_progress32 0x54030000 0x0 36 2 184 12 ""
 6 Static 0x54000000 0x0 2 16 32 12 "Average"
 5 msctls_progress32 0x54030000 0x0 36 16 184 12 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030205 "*" "" ""

ret
 messages
int- hwi
WAVEHDR- hd

sel message
	case WM_INITDIALOG
	 start recording
	WAVEFORMATEX wf.cbSize=sizeof(wf)
	wf.wFormatTag=WAVE_FORMAT_PCM
	wf.nChannels=1 ;;mono
	wf.nSamplesPerSec=44100
	wf.wBitsPerSample=16
	wf.nBlockAlign=wf.nChannels*wf.wBitsPerSample/8
	wf.nAvgBytesPerSec=wf.nSamplesPerSec*wf.nBlockAlign
	
	int rc=waveInOpen(&hwi WAVE_MAPPER &wf hDlg 0 CALLBACK_WINDOW); if(rc) out "waveInOpen: %i" rc; ret
	
	hd.dwBufferLength=wf.nAvgBytesPerSec/5 ;;5 buffers/s
	hd.lpData=q_malloc(hd.dwBufferLength)
	rc=waveInPrepareHeader(hwi &hd sizeof(hd)); if(rc) out "waveInPrepareHeader: %i" rc; ret
	
	rc=waveInAddBuffer(hwi &hd sizeof(hd)); if(rc) out "waveInAddBuffer: %i" rc; ret
	rc=waveInStart(hwi)
	if(rc) out "waveInStart: %i" rc; ret
	
	case WM_DESTROY
	 stop recording
	waveInStop(hwi)
	rc=waveInUnprepareHeader(hwi &hd sizeof(hd)); if(rc) out "waveInUnprepareHeader: %i" rc
	q_free hd.lpData
	waveInClose(hwi)
	hwi=0
	0
	
	 case MM_WIM_OPEN
	 out "open"
	
	 case MM_WIM_CLOSE
	 out "close"
	
	case MM_WIM_DATA ;;this runs at 5 Hz frequency
	 out "%i %i 0x%X" hd.dwBytesRecorded hd.lpData hd.dwFlags
	if(!hwi or !hd.dwBytesRecorded) ret
	
	int i dc v peak avg nsamples
	word* w=hd.lpData
	nsamples=hd.dwBytesRecorded/2
	
	 calc direct current level, because on some computers it is not 0
	for(i 0 nsamples) dc+ConvertSignedUnsigned(w[i] 2)
	dc/nsamples
	
	 calc peak and average
	for(i 0 nsamples)
		v=abs(ConvertSignedUnsigned(w[i] 2)-dc)
		if(v>peak) peak=v
		avg+v
	 convert to %
	peak=MulDiv(peak 100 0x8000)
	avg=MulDiv(avg/nsamples 100 0x8000)
	if(peak>100) peak=100
	 display
	SendMessage id(4 hDlg) PBM_SETPOS peak 0
	SendMessage id(5 hDlg) PBM_SETPOS avg 0
	
	 continue to record
	rc=waveInAddBuffer(hwi &hd sizeof(hd))
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
