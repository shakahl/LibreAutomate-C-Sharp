 Writes wav file.
 1. Call Begin to create and open empty file.
 2. Call Write one or more times to write audio data.
 3. Call End to finish and close file.

 EXAMPLE

str sFile="$desktop$\square.wav"
int nChannels=1
int sampleRate=44100

#compile "__WriteWavFile"
WriteWavFile x

x.Begin(sFile nChannels sampleRate)

 create square wave, 1 loop
ARRAY(word) data.create(200)
int i
for(i 0 data.len/2) data[i]=0x4000
for(i data.len/2 data.len) data[i]=0xC000

 write 1000 loops
for(i 0 1000) x.Write(&data[0] data.len/nChannels)

x.End

out "<>Saved to <link>%s</link>" sFile
