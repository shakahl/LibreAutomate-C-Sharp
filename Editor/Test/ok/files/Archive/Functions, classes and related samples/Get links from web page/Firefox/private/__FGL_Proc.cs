 /
function# FFNode&x level *p FFNODEINFO&ni

ARRAY(str)& aURL=+p[1]
ARRAY(str)& aText=+p[2]
ARRAY(Acc)& aObj=+p[3]

Acc a.FromFFNode(x)

_s=a.Value
if(!_s.len) ret 1

if(&aURL) aURL[]=_s
if(&aText) aText[]=a.Name
if(&aObj) aObj[]=a

err+
ret 1
