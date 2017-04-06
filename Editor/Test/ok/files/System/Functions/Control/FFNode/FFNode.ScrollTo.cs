
 Scrolls web page to make this node visible.
 Does not work in Chrome (error).


if(!node) end ERR_INIT
node.scrollTo(1)

err+ end ERR_FAILED

 note: if placeTopLeft is false, should scroll minimally until visible, and only if not visible. Actually always scrolls so that it is at bottom, maybe partially visible.
