 str js=
  javascript:(function(){ function zoomImage(image, amt) { if(image.initialHeight == null) { /* avoid accumulating integer-rounding error */ image.initialHeight=image.height; image.initialWidth=image.width; image.scalingFactor=1; } image.scalingFactor*=amt; image.width=image.scalingFactor*image.initialWidth; image.height=image.scalingFactor*image.initialHeight; } var i,L=document.images.length; for (i=0;i<L;++i) zoomImage(document.images[i], 2); if (!L) alert("This page contains no images."); })();
 web js

str js=
 javascript:var count=0, text, dv;text=prompt("Search phrase:", "");if(text==null || text.length==0)return;dv=document.defaultView;function searchWithinNode(node, te, len){var pos, skip, spannode, middlebit, endbit, middleclone;skip=0;if( node.nodeType==3 ){pos=node.data.toUpperCase().indexOf(te);if(pos>=0){spannode=document.createElement("SPAN");spannode.style.backgroundColor="yellow";middlebit=node.splitText(pos);endbit=middlebit.splitText(len);middleclone=middlebit.cloneNode(true);spannode.appendChild(middleclone);middlebit.parentNode.replaceChild(spannode,middlebit);++count;skip=1;}}else if( node.nodeType==1&& node.childNodes && node.tagName.toUpperCase()!="SCRIPT" && node.tagName.toUpperCase!="STYLE"){for (var child=0; child < node.childNodes.length; ++child){child=child+searchWithinNode(node.childNodes[child], te, len);}}return skip;}window.status="Searching for '"+text+"'...";searchWithinNode(document.body, text.toUpperCase(), text.length);window.status="Found "+count+" occurrence"+(count==1?"":"s")+" of '"+text+"'.";
web js


 must not contain comments
 must not contain outer function (like in the commented code).
