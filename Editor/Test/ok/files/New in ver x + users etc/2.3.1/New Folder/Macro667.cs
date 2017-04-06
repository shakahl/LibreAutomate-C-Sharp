spe
int w1=win("TOOLBAR" "QM_toolbar") ;;change this

int+ g_tb_follow_toggle; g_tb_follow_toggle^1 ;;just for example, to toggle between follow and not each time you run this macro

if(!CheckQmTbItem(w1 32831 g_tb_follow_toggle)) ret
men 32831 w1 ;;Follow Owner
