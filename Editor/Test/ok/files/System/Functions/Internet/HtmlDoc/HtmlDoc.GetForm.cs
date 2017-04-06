function# `form [str&action] [str&data] [ARRAY(POSTFIELD)&a] [ARRAY(MSHTML.IHTMLElement)&a2]

 Gets info about INPUT elements in a form.
 This info can be used with Http.PostFormData.
 Returns 1 if the submit method is "POST", 0 if "GET".
 Error if something fails, eg form not found.

 form - form name or 0-based index. Use 0 if there is only one form. See <help>HtmlDoc.GetHtmlElement</help>. QM 2.4.4: also can be form's MSHTML.IHTMLElement or derived COM interface.
 action - variable that receives "action" attribute. Can be 0 if not needed.
   It is relative (or sometimes full) address (URL) of the file that would process the submit request.
   Note that it may be relative to the source page but not to the web server. With Http.PostFormData, action must be used relative to web server.
 data - variable that receives names and values of INPUT elements in single string. Can be 0 if not needed.
 a - variable that receives names and values of INPUT elements. Can be 0 if not needed.
 a2 - variable that receives objects for INPUT elements. Can be 0 if not needed.

 REMARKS
 data and a receive names and values that would be sent when the form submitted.
   For example, unchecked radio buttons are skipped, etc.
   Values of image elements will be empty.
   The function replaces some characters in values (in data only) with QM escape sequences. For example, " to '', new line to [].
 a2 receives all INPUT elements, including those that would not be sent.
 If method is "GET", on submit the data would be appended to the URL (using certain format). To submit such a form, can be used IntGetFile.
 If method is "POST", the data would be sent hidden. To submit such a form, can be used Http.PostFormData.

 EXAMPLE
 HtmlDoc d.InitFromWeb("http://www.test.com/test.php")
 str action data
 out d.GetForm(0 action data)
 out action
 out "-------"
 out data


ARRAY(MSHTML.IHTMLElement) a3; int i j
if(!&a2) &a2=a3
GetHtmlElements(a2 "INPUT" "FORM" form)
err _error.description.findreplace("container not found" "form not found"); end _error

MSHTML.IHTMLFormElement f=+GetHtmlElement("FORM" form)
if(&action) action=f.action
_s=f.method
int r=_s~"POST"

if(&a or &data)
	ARRAY(POSTFIELD) a4; if(!&a) &a=a4
	a.redim(a2.len)
	if(&data) data.all
	for(i 0 a2.len)
		MSHTML.IHTMLInputElement e=+a2[i]
		POSTFIELD& p=a[j]
		p.name=e.name
		if(!p.name.len) a.remove(j); continue
		p.value=e.value
		p.isfile=0
		str st=e.type
		sel st 1
			case "file" p.isfile=1
			case "checkbox" if(!e.checked) p.value.all
			case "radio" if(!e.checked) a.remove(j); continue
		j+1
		if(&data)
			_s=p.value; _s.escape(1)
			data.formata("name=%s[]value=%s[]type=%s[][]" p.name _s st)

ret r

err+ end _error
