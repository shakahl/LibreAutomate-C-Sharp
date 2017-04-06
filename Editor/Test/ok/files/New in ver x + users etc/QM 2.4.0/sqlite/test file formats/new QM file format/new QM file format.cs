HEADER
	signature
	file version
	password hash
	n streams
INDEX OF STREAMS
	stream 0 offset
	stream 1 offset
	...
STREAM 0
STREAM 1
...
 ________________________________

A stream can be a QM item or anything.

READING ALL STREAMS
Do it when opening file. We need to load most info into memory, but not all.
Read header.
Read whole index.
Sort index (in memory) so that streams would be read sequentially (faster).
Read streams and store in memory. Skip parts that don't need to be always in memory (eg skip item text>8KB, skip item data).


READING SINGLE STREAM


WRITING
When a new stream added, or data of an existing stream changed, write whole stream.
Always write at the end of file or in a free space. Regardless whether it is new stream or not.
When successfully written, update its offset in index, and mark previously used area as empty.
