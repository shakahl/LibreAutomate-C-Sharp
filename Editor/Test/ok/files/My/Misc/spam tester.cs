Htm el=htm("BODY" "" "" win("Outlook Express" "Outlook Express Browser Class") 0 0 0x20)
str body=el.Text
 out body
body.replacerx("[^[:alnum:]]+" "")
int score
ARRAY(str) a
lpstr spamwords="advert|alert|announce|breakout|business|campaign|company|expected|forecast|hot|investor|market|news|offer|opportunit|price|potent|product|profit|promot|provid|recent|stock|strong|super|week"
 Q &q
score+findrx(body spamwords 0 1|4|16 a)
 Q &qq
 outq
out score
for(_i 0 a.len) out a[0 _i]

