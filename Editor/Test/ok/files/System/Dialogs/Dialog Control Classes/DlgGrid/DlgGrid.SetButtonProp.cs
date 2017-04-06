function [hicon] [$tooltip]

 Sets button icon and/or tooltip.
 Can be called only when received LVN_BEGINLABELEDIT notification. Applies only to that button instance.

 hicon - icon handle. Can be 0.
    The control does not copy the icon, therefore it must be alive for a while.
    If 1, uses standard "open folder" icon.
 tooltip - tooltip text. Can be "".


Send(GRID.LVM_QG_SETBUTTONPROP hicon +tooltip)
