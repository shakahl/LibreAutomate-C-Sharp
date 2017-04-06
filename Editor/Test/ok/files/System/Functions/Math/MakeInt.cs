 /
function# word'loword word'hiword

 Stores two 16-bit values into a 32-bit value, like C++ macros MAKELONG, MAKEWPARAM and MAKELPARAM do.

 loword - will go to the least significant 16 bits.
 hiword - will go to the most significant 16 bits.

 Added in: QM 2.3.2.


ret hiword<<16|loword
