 /

 Scrolls QM output to show the beginning.

 REMARKS
 Usually used after out multiline_text.
 The function waits 0.1 s, because QM displays output text asynchronously.


0.1

int c=id(2201 _hwndqm) ;;QM_Output
SendMessageW(c SCI.SCI_GOTOPOS 0 0)
