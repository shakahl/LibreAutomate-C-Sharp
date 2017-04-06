 Extracts text from an image file.

str img="$desktop$\clipboard.tif" ;;this is an image containing text
out _s.searchpath(img)


typelib MODI {A5EDEDF4-2BBC-45F3-822B-E60C278A1A79} 11.0
MODI.Document doc._create

doc.Create(img.expandpath)
doc.OCR(MODI.miLANG_ENGLISH -1 0)

MODI.Image im=doc.Images.Item(0)
str s=im.Layout.Text

out s ;;and this is the text
