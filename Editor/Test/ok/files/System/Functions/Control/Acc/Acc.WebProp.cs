function [str&tag] [str&innerHTML] [str&outerHTML]

 Gets various HTML properties.

 tag, innerHTML, outerHTML - variables that receive the properties.

 REMARKS
 Works with Internet Explorer, Firefox, possibly Chrome. Read more in <help>Acc.WebPageProp</help>.
 In Chrome can get only tag.

 Added in: QM 2.3.3.


if(!a) end ERR_INIT

if(&tag) tag.all
if(&innerHTML) innerHTML.all
if(&outerHTML) outerHTML.all

opt err 1
Htm e; FFNode f
sel __HtmlObj(e f)
	case 1
	if(&tag) tag=e.Tag
	if(&innerHTML) innerHTML=e.HTML(1)
	if(&outerHTML) outerHTML=e.HTML
	
	case 2
	if(&tag) tag=f.Tag
	if(&innerHTML) innerHTML=f.HTML
	if(&outerHTML) outerHTML=f.HTML(1)
	
	case else end ERR_FAILED
