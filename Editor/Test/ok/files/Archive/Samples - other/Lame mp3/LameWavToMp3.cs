 /
function $wavFile $mp3File

 Converts wav file to mp3.
 Error if fails.

 EXAMPLE
 LameWavToMp3 "$windows$\Media\notify.wav" "$desktop$\notify.mp3"


ref lame "lame_def"

File		pFileIn
File		pFileOut
BE_CONFIG	beConfig

int		dwSamples
int		dwMP3Buffer
int	hbeStream
int		e

ARRAY(byte)		MP3Buffer
ARRAY(word)		WAVBuffer
byte*		pMP3Buffer
word*		pWAVBuffer


// Try to open the WAV file, be sure to open it as a binary file!	
pFileIn.Open( wavFile, "rb" );

// Open MP3 file
pFileOut.Open(mp3File,"wb+");

// use the LAME config structure
beConfig.dwConfig = BE_CONFIG_LAME;

// this are the default settings for testcase.wav
beConfig.format.LHV1.dwStructVersion	= 1;
beConfig.format.LHV1.dwStructSize		= sizeof(beConfig);		
beConfig.format.LHV1.dwSampleRate		= 44100;				;; INPUT FREQUENCY
beConfig.format.LHV1.dwReSampleRate		= 0;					;; DON"T RESAMPLE
beConfig.format.LHV1.nMode				= BE_MP3_MODE_JSTEREO;	;; OUTPUT IN STREO
beConfig.format.LHV1.dwBitrate			= 128;					;; MINIMUM BIT RATE
beConfig.format.LHV1.nPreset			= LQP_R3MIX;		;; QUALITY PRESET SETTING
beConfig.format.LHV1.dwMpegVersion		= MPEG1;				;; MPEG VERSION (I or II)
beConfig.format.LHV1.dwPsyModel			= 0;					;; USE DEFAULT PSYCHOACOUSTIC MODEL 
beConfig.format.LHV1.dwEmphasis			= 0;					;; NO EMPHASIS TURNED ON
beConfig.format.LHV1.bOriginal			= 1;					;; SET ORIGINAL FLAG
beConfig.format.LHV1.bWriteVBRHeader	= 1;					;; Write INFO tag

;;	beConfig.format.LHV1.dwMaxBitrate		= 320;					;; MAXIMUM BIT RATE
;;	beConfig.format.LHV1.bCRC				= 1;					;; INSERT CRC
;;	beConfig.format.LHV1.bCopyright			= 1;					;; SET COPYRIGHT FLAG	
;;	beConfig.format.LHV1.bPrivate			= 1;					;; SET PRIVATE FLAG
;;	beConfig.format.LHV1.bWriteVBRHeader	= 1;					;; YES, WRITE THE XING VBR HEADER
;;	beConfig.format.LHV1.bEnableVBR			= 1;					;; USE VBR
;;	beConfig.format.LHV1.nVBRQuality		= 5;					;; SET VBR QUALITY
beConfig.format.LHV1.bNoRes				= 1;					;; No Bit resorvoir

// Preset Test
//	beConfig.format.LHV1.nPreset			= LQP_PHONE;

// Init the MP3 Stream
e = beInitStream(&beConfig, &dwSamples, &dwMP3Buffer, &hbeStream);

// Check result
if(e != BE_ERR_SUCCESSFUL) end "Error opening encoding stream (%lu)" 0 e

// Allocate MP3 buffer
MP3Buffer.create(dwMP3Buffer);
pMP3Buffer=&MP3Buffer[0]

// Allocate WAV buffer
WAVBuffer.create(dwSamples);
pWAVBuffer=&WAVBuffer[0]

int dwRead
int dwWrite
int dwDone
int dwFileSize

// Seek to end of file
fseek(pFileIn,0,SEEK_END);

// Get the file size
dwFileSize=ftell(pFileIn);

// Seek back to start of WAV file,
// but skip the first 44 bytes, since that's the WAV header
fseek(pFileIn,44,SEEK_SET);


// Convert All PCM samples
int pr ppr(-1)
rep
	dwRead=fread(+pWAVBuffer,sizeof(word),dwSamples,pFileIn)
	if(dwRead<=0) break
	
	// Encode samples
	e = beEncodeChunk(hbeStream, dwRead, pWAVBuffer, pMP3Buffer, &dwWrite);

	// Check result
	if(e != BE_ERR_SUCCESSFUL)
		beCloseStream(hbeStream);
		end "beEncodeChunk() failed (%lu)" 0 e
	
	// write dwWrite bytes that are returned in tehe pMP3Buffer to disk
	if(fwrite(pMP3Buffer,1,dwWrite,pFileOut) != dwWrite)
		beCloseStream(hbeStream);
		end "Output file write error"

	dwDone += dwRead*sizeof(word);
	
	pr=100.0*dwDone/dwFileSize
	if(pr>ppr)
		_s.format("%i%%", pr)
		OnScreenDisplay _s 10 0 0 0 0 0 1|4|8 "osd_lame_mp3"
		ppr=pr

// Deinit the stream
e = beDeinitStream(hbeStream, pMP3Buffer, &dwWrite);

// Are there any bytes returned from the DeInit call?
// If so, write them to disk
if( dwWrite ) fwrite( pMP3Buffer, 1, dwWrite, pFileOut )

// close the MP3 Stream
beCloseStream( hbeStream );

// Close output file
pFileOut.Close

// Write the INFO Tag
beWriteInfoTag( hbeStream, _s.expandpath(mp3File) );
