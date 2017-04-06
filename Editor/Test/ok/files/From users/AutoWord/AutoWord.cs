typelib Word {00020905-0000-0000-C000-000000000046} 8.0 0x409
class JaconWord VARIANT'jaw_path Word.Document'jaw_doc Word.Range'jaw_rng Word.Range'jaw_f_rng Word.Selection'jaw_sel
def JAW_REPLACE 0x8000

Word.Application app._create

JaconWord jw.new(app "$desktop$\test.doc")

str s1="This is a Word test 123456789[]{Image1}[][]"
str s2="One Two Three Four Five[][]"
str s3="Replace text:[]{Text1}[][]End of document.[]"
str s4="abcdefghijklmnopqrstuvwxyz"
jw.font("Arial" 24 1)
jw.appendtext(s1)

jw.appendtext(s2 "Arial" 24 2)

if jw.find("{Image1}")
	jw.insertpicture("C:\Qu\bmp\Jbem0.bmp")

jw.appendtext(s3 "Arial" 14)

app.Visible = TRUE
wait 2

if jw.find("{Text1}" s4 JAW_REPLACE)
	out "Text replaced"



jw.jaw_doc.SaveAs(jw.jaw_path)
