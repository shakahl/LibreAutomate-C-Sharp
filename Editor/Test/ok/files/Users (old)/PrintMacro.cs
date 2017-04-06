 Prints current macro using WordPad


spe 20

QMITEM q
int iid=qmitem("" 0 q 1|64) ;;get current macro properties

act id(2210 _hwndqm)
key Ca CSc CH ;;copy rich text

run "wordpad" "" "" "" 0x2800 "WordPad"

key Y Cv CH ;;paste

 header
outp "Macro: %s" q.name
outp "[]Trigger: %s  (%s)" q.triggerdescr _s.getmacro(iid 2)
outp "[]Code:"
key H; rep(3) key CS{RR}Cb HU ;;bold

key Cp ;;print
