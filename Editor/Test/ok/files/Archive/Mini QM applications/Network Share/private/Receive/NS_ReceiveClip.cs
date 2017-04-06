 \
function# str'data

if(data.len<4 or !data.beg("ns")) ret ;;check signature
data.get(data 4)

data.setclip

OnScreenDisplay "Received clipboard data from another computer." 3
err+ ret
ret 1
