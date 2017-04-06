function [int&getMin] [int&getMax]

 Gets bounds of the area in which splitter can be moved.
 The values are in client area.

 getMin - receives min bound. Can be 0.
 getMax - receives max bound. Can be 0.


Send(SPM_GETBOUNDS &getMin &getMax)
