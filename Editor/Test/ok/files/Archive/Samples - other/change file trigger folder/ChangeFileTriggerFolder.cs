 /
function $macro $folder

 Changes folder of a file trigger.

 In the trigger, folder must be macro name enclosed in %.
 For example, if the macro is Function15, in the Folder field in Properties type %Function15%.
 The function creates/changes environment variable whose name is macro.

 EXAMPLE
 ChangeFileTriggerFolder "Function168" "$desktop$\folder1"


SetEnvVar macro folder
if(!dis(macro)) dis+ macro; dis- macro
