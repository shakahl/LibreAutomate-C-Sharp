\Dialog_Editor

 Records audio input (eg microphone) and saves in file.

function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str e3
e3="$desktop$\recorded.wav"
if(!ShowDialog("dlg_audio_record" &dlg_audio_record &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 135 "Record Audio"
 4 Static 0x54000000 0x0 2 6 14 10 "File"
 3 Edit 0x54030080 0x200 20 4 200 14 ""
 5 Button 0x54032000 0x0 2 30 48 14 "Record"
 6 Button 0x5C032000 0x0 54 30 48 14 "Stop"
 9 msctls_progress32 0x54030000 0x0 112 30 108 12 ""
 7 Button 0x54032000 0x0 2 50 48 14 "Play"
 8 Button 0x54032000 0x0 54 50 48 14 "Stop"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030205 "*" "" ""

ret
 messages
#compile "__WriteWavFile"
WriteWavFile-- t_wav
int-- t_hwi t_nChannels t_sampleRate
ARRAY(WAVEHDR)-- t_a
WAVEHDR& h
int rc i

sel message
	case WM_INITDIALOG
	
	case WM_DESTROY
	if(t_hwi) SendMessage hDlg WM_COMMAND 6 0; 0
	
	 case MM_WIM_OPEN
	 out "open"
	
	 case MM_WIM_CLOSE
	 out "close"
	
	case MM_WIM_DATA
	&h=+lParam
	 out "%i %i %i 0x%X" t_hwi h.dwBytesRecorded h.lpData h.dwFlags
	if(!t_hwi or !h.dwBytesRecorded) ret
	
	 write this buffer to file
	t_wav.Write(+h.lpData h.dwBytesRecorded/2/t_nChannels); err out _error.description
	
	 display peak level
	int dc v peak nsamples
	word* w=h.lpData
	nsamples=h.dwBytesRecorded/2
	 calc direct current level, because on some computers it is not 0
	for(i 0 nsamples) dc+ConvertSignedUnsigned(w[i] 2)
	dc/nsamples
	 calc peak
	for(i 0 nsamples) v=abs(ConvertSignedUnsigned(w[i] 2)-dc); if(v>peak) peak=v
	peak=MulDiv(peak 100 0x8000); if(peak>100) peak=100 ;;%
	SendMessage id(9 hDlg) PBM_SETPOS peak 0
	
	 continue to record
	rc=waveInAddBuffer(t_hwi &h sizeof(h)); if(rc) wave_in_error "waveInAddBuffer" rc
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 5 ;;Record
	if(t_hwi) ret
	
	 you can change these values
	t_nChannels=2 ;;stereo
	t_sampleRate=44100 ;;44.1 kHz
	int nBuffers=4 ;;works with 1 too, but must be at least 2 to record smoothly
	int nBuffersSecond=10 ;;10 buffers/s
	
	t_wav.Begin(_s.getwintext(id(3 hDlg)) t_nChannels t_sampleRate); err out _error.description; ret
	
	WAVEFORMATEX wf.cbSize=sizeof(wf)
	wf.wFormatTag=WAVE_FORMAT_PCM
	wf.nChannels=t_nChannels
	wf.nSamplesPerSec=t_sampleRate
	wf.wBitsPerSample=16
	wf.nBlockAlign=wf.nChannels*wf.wBitsPerSample/8
	wf.nAvgBytesPerSec=t_sampleRate*t_nChannels*2
	
	rc=waveInOpen(&t_hwi WAVE_MAPPER &wf hDlg 0 CALLBACK_WINDOW); if(rc) wave_in_error "waveInOpen" rc; ret
	
	if(!t_a.len) t_a.create(nBuffers)
	for i 0 t_a.len
		&h=t_a[i]
		h.dwBufferLength=t_sampleRate*t_nChannels*2/nBuffersSecond
		h.lpData=q_malloc(h.dwBufferLength)
		rc=waveInPrepareHeader(t_hwi &h sizeof(h)); if(rc) wave_in_error "waveInPrepareHeader" rc; break
		rc=waveInAddBuffer(t_hwi &h sizeof(h)); if(rc) wave_in_error "waveInAddBuffer" rc
	
	if(!rc) rc=waveInStart(t_hwi); if(rc) wave_in_error "waveInStart" rc
	
	if(rc) PostMessage hDlg WM_COMMAND 6 0; ret
	
	EnableWindow id(5 hDlg) 0
	EnableWindow id(6 hDlg) 1
	
	case 6 ;;Stop
	if(!t_hwi) ret
	rc=waveInReset(t_hwi); if(rc) wave_in_error "waveInReset" rc
	
	for i 0 t_a.len
		&h=t_a[i]
		rc=waveInUnprepareHeader(t_hwi &h sizeof(h)); if(rc) wave_in_error "waveInUnprepareHeader" rc; continue
		q_free h.lpData; h.lpData=0
	
	rc=waveInClose(t_hwi); if(rc) wave_in_error "waveInClose" rc
	t_hwi=0
	
	EnableWindow id(5 hDlg) 1
	EnableWindow id(6 hDlg) 0
	SendMessage id(9 hDlg) PBM_SETPOS 0 0
	
	t_wav.End; err out _error.description; ret
	out "<>Saved: <link>%s</link>." _s.getwintext(id(3 hDlg))
	
	case 7 ;;Play
	bee _s.getwintext(id(3 hDlg)); err out _error.description
	
	case 8 ;;Stop
	bee ""
	
	case IDOK
	case IDCANCEL
ret 1
