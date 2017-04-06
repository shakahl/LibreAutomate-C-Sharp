 Adds tooltips to controls in a dialog or other window.
 See the samples.

 QM 2.3.4. Instead use __Tooltip class, or set tooltips in dialog editor.

 Insert this in your dialog or in function init2
#compile __CToolTip

 These can be used with Create:
 def TTS_ALWAYSTIP 0x01
 def TTS_NOPREFIX 0x02
 def TTS_NOANIMATE 0x10
 def TTS_NOFADE 0x20
 def TTS_BALLOON 0x40
 def TTS_CLOSE 0x80
