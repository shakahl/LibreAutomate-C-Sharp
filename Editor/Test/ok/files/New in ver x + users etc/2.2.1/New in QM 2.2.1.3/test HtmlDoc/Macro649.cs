out
ARRAY(POSTFIELD) a
HtmlDoc d.InitFromWeb("https://www.google.com/accounts/Login")
d.GetForm(0 0 0 a)

a[1].value="support@quickmacros.com"
a[2].value="password"

int i
for i 0 a.len
	out a[i].name
	out a[i].value
	out a[i].isfile
	out "---"

Http h.Connect("www.google.com" "" "" 443)
str sr
h.PostFormData("accounts/LoginAuth" a sr) ;;does not work
 out sr
HtmlDoc dd.InitFromText(sr)
out dd.GetText
