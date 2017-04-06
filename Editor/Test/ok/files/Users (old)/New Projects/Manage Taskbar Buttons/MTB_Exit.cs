function [_atend]

if(!_atend) shutdown -6 0 "MTB_Main"
else g_taskbar.UnhideAll
