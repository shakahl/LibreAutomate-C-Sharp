 /
function MSHTML.DispHTMLInputElement'btn

 out "btn.onclick"

 get form data on form button click
MSHTML.HTMLDocument doc=btn.document
str s
HD_sample_get_form_data doc s
out s
