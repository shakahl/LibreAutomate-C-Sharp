int c=id(2201 _hwndqm) ;;QM_Output
 SendMessage c SCI.SCI_SETVIEWWS SCI.SCWS_VISIBLEAFTERINDENT 0
SendMessage c SCI.SCI_SETVIEWWS SCI.SCWS_VISIBLEALWAYS 0
SendMessage c SCI.SCI_SETWHITESPACESIZE 2 0
SendMessage c SCI.SCI_SETWHITESPACEFORE 1 0xa0a0a0
 SendMessage c SCI.SCI_SETWHITESPACEFORE 1 0xff00
 SendMessage c SCI.SCI_SETWHITESPACEBACK 1 0xffffff
out " Some text to[9]test.  "
out "[9]Some text to[9]test.  "
