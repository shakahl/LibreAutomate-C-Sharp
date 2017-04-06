 executes script existing in the page

Htm el=htm("A" ...) ;;find element
BSTR savehref=el.el.getAttribute("href" 0) ;;save
el.el.setAttribute("href" "javascript:show(8,3)" 1) ;;change
el.Click ;;execute
el.el.setAttribute("href" savehref 1) ;;restore
