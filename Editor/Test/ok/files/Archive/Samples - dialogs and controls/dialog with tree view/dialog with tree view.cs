 declare type for item data
type MYTVITEMDATA str'label str'data htvitem !issubitem
 this array will hold all items
ARRAY(MYTVITEMDATA)- t_a
t_a.create(3)
t_a[0].label="folder"
t_a[1].label="item 1"; t_a[1].data="a[]b[]c"; t_a[1].issubitem=1
t_a[2].label="item 2"; t_a[2].data="d[]e[]f"; t_a[2].issubitem=1
 show dialog
str controls = "4 5"
str e4Tex e5Ind
if(!ShowDialog("dialog_with_tree_view" &dialog_with_tree_view &controls)) ret
 index of selected item is
int i=val(e5Ind)
out i
out t_a[i].label
