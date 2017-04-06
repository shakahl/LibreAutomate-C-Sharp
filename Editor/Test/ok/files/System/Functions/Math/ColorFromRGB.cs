 /
function# red green blue

 Returns color value in format 0xBBGGRR.

 red, green and blue - color components. Must be between 0 and 255.


ret (red&255) | (green&255<<8) | (blue&255<<16)
