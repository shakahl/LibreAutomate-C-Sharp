 /
function# [action] [~description] [$style] [timeout] ;;action: 0 continue, 1 end, 2 ask.

 Shows message box with description of last run-time error, and optionally ends macro.
 For example, can be called when error is catched by <help>err</help>.
 Returns button character ('Y', 'N', etc), same as <help>mes</help>.

 action:
   0 - warning. The message box contains OK button and warning icon.
   1 - error. The message box contains OK button and error icon. The macro ends when the user closes it.
   2 - question. The message box contains Yes/No buttons and warning icon. The macro ends if the user clicks No (or Cancel, Abort, if defined by style).
 description - can be used to replace original error description. If begins with +, it is appended. Supports <help #IDP_SYSLINK>links</help>.
 style - same as with <help>mes</help>. If used, overrides style defined by action (buttons, icon).
 timeout - the number of seconds to show the message. 
 

int i iid
str sm se
if(description.beg("<>")) description.remove(0 2); lpstr tags="<>"

if(description.len and description[0]!='+') se=description
else iid=_error.iid; se=_error.description; if(description.len and description[0]='+') se+"[][]"; se+description+1
if(!iid) iid=getopt(itemid 1)
sm.getmacro(iid 1)

if(empty(style))
	sel(action)
		case 1 style="x"
		case 2 style="!YN"; se+"[][]Continue?"
		case else style="!"

MES m.timeout=timeout; m.style=style
i=mes(F"{tags}Run-time error in {sm}:[][]{se}" "QM error" m)

sel action
	case 1 end
	case 2 sel(i) case ['C','N','A'] end
ret i
