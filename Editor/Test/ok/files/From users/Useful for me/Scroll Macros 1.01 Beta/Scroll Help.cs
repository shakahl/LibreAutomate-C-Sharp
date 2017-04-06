 OUTLINE
 ========

 A set of macros to make scrolling easier
 
 Handy Features
   - Scroll any window that the mouse is currently over, whether 
     or not it's active
   - Adjust speed of scrolling

 "Katmouse" is a similar utility (http://kickme.to/katmouse)
 Unlike katmouse, however, these macros also work great in Windows 98,
 and you have the source code here for complete flexibility


 USE
 ====
     Macro                 Trigger             Desc.
     -------------------------------------------------------------------------------
     Scroll_Up             #U                  Wheel up
     Scroll_Down           #D                  Wheel down
     Scroll_Up_VS          #CSU                Force use of WM_VSCROLL
     Scroll_Down_VS        #CSD
     Scroll_Up_MW          #CAU                Force use of WM_MOUSEWHEEL
     Scroll_Down_WM        #CAD
     Tilda                 '                   Sets speed factors & scroll type
     Scroll_Init           [none]              Initialize (should be run to set _m.lineDefault)
     Scroll_Input          [none]              Input engine (ESC key resets to default)
     Scroll_Dialog         [none]              Factor dialog
     Scroll_               [none]              Scroll engine
     Needs_WM_MOUSEWHEEL   [none]              Add to the arrays there as you need to...

 GLOBAL VARIABLES:
 ========
   MOUSESCROLL+ _m
	   The defaults will meet most needs, but this global allows great flexibility
       - Enables switching between faster/slower scrolling, and scroll types
       - To be prompted to change the speed of scrolling, first use the Tilda_ macro 
         which sets scrollFactor or deltaFactor via Scroll_Input function.
         (Or, use some other triggered macro to set setScrollFactor and/or setDeltaFactor
         to 1. Or call Scroll_Input yourself.)
       - forceScroll - use Tilda_ or another triggered macro to change this if you want
         (NOT recommended except as a test - SB_LINEUP won't work in many Microsoft Office
         programs, while WM_MOUSEWHEEL won't work in many windows)

       scrollFactor:
           0          - SB_LINEUP * _m.lineDefault (default)
           1 or more  - use SB_LINEUP * scrollFactor
          -1 or less  - use SB_PAGEUP * scrollFactor* -1

       deltaFactor:
           0 or 1	 - single WM_MOUSEWHEEL (default)
           2 or more  - use WHEEL_DELTA * deltaFactor

       forceScroll
           0          - normal operation: SB_LINEUP or WM_MOUSEWHEEL depending on window
           1          - force use of WM_MOUSEWHEEL message in all windows, using deltaFactor
           2          - force use of WM_VSCROLL in all windows, using scrollFactor
