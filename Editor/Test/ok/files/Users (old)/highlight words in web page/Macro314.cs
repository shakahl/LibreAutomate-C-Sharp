str jshilite=
 javascript:function H(w,s){r=w.document.body.createTextRange();for(i=0;r.findText(s);i++){r.execCommand('BackColor','','yellow');r.collapse(false);}return i;}function G(){if(frames.length==0)return document.selection.createRange().text;else for(k=0;F=frames[k];++k){u=F.document.selection.createRange().text;if(u)return u;}}function P(){var t=0,s=G();if(!s)s=prompt('Find:','');if(s){if(frames.length==0)t+=H(window,s);else for(j=0;F=frames[j];++j)t+=H(F,s);alert(t+' found.');}}P();

 int hwnd = win("Dialog")
int hwnd = win("Internet Explorer")

web jshilite 0 hwnd