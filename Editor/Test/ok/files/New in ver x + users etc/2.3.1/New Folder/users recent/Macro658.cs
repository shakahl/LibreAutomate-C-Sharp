str text="some ''text'' & > < ąč"
HtmlDoc d.InitFromText(text)
str html=d.GetHtml
out html

 some more processing needed, eg replace " to &quot;, specify charset, etc
