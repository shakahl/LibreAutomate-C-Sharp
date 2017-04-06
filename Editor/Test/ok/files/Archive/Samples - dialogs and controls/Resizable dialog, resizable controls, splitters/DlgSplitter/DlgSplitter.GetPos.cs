function# [int&getMaxPos]

 Returns current position. Optionally gets max position.
 The values are relative to the min bound.

 getMaxPos - receives max position. Can be 0.


ret Send(SPM_GETPOS &getMaxPos)
