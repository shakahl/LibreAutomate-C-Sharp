out
int w=wait(3 WV win("Edit Control Styles - Microsoft Windows SDK - Microsoft Document Explorer" "wndclass_desked_gsk"))
Htm e=htm("link" "CreateWindow" "" w "0" 3 0x21 3)
 act w

 e.Click

 e.SetFocus

 e.Scroll

 int x y cx cy
 e.Location(x y cx cy)
 out F"{x} {y} {cx} {cy}"

 str text=e.Text
 out text

 e.SetText("new")

 str html=e.HTML
 str html1=e.HTML(1)
 out html
 out html1

 str tag=e.Tag
 out tag

 str href=e.Attribute("keywords")
 out href

 e.CbSelect("ttt")
 e.CbSelect(5 1)

 str selItemText
 int selItem=e.CbItem(selItemText)

 int checked=e.Checked

 e.ClickAsync

 e.Mouse(1)
 e.Mouse(2 5 2)

str url title html text
e.DocProp(0 url title html text)
out F"'{url}'[]'{title}'[]'{html}'[]'{text}'"

