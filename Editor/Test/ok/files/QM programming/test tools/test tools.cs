int i
test_tool_button "Text"
test_tool_button "Keys"
test_tool_button "Mouse"
test_tool_button "Wait"
for(i 1 11) test_tool_button "Windows, controls" i
for(i 1 16) test_tool_button "Files, web" i
for(i 1 7) test_tool_button "Dialogs, other" i

#sub Test
