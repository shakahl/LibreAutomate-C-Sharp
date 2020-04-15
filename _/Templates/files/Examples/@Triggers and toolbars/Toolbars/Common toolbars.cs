using Au; using Au.Types; using System; using System.Collections.Generic;
using Au.Triggers;

partial class Script {

[Toolbars]
void StartupToolbars() {
	
	//Call toolbar functions here, like in examples.
	
	if (_enableToolbarExamples) {
		//examples
		
		Toolbar_Startup1();
		Triggers.Mouse[TMEdge.TopInRight25] = o => Toolbar_Other1(o);
	}
}

//Add here your toolbars that are common to all windows.
//In the above function call toolbar functions, either directlty (to show at startup) or from a trigger.
//To create toolbars and add buttons can be used snippets. Start typing "tool" and you will see snippets in the completion list.



#region toolbar examples

void Toolbar_Startup1(Au.Triggers.TriggerArgs ta = null) {
	var t = new AToolbar("Toolbar_Startup1");
	if (!t.SettingsModified) { //first time or after deleting the settings file. Later use saved values.
		t.AutoSize = true;
	}
	t.BorderColor = System.Drawing.Color.BlueViolet;
	
	//add buttons
	t["A"] = o => {  };
	t["B"] = o => {  };
	t.MenuButton("C", m => { //drop-down menu
		m["X"] = o => {  };
		m["Y"] = o => {  };
	});
	t.Group("Examples"); //horizontal separator, optionally with text
	t.NoText = true;
	t["Run program"] = o => AExec.Run(AFolders.System + @"notepad.exe");
	t["Run script"] = o => ATask.Run("Script example1.cs");
	t["Copy-paste"] = o => {
		string s = AClipboard.Copy(); //note: to test it, at first select some text somewhere, else it will fail
		s = s.Upper();
		AClipboard.Paste(s);
	};
	
//	//auto-hide. Above is the auto-hide part. Below is the visible part.
//	t = t.CreateSatelliteOwner();
//	if(!t.SettingsModified) {
//		
//	}
	
	t.Show(ta);
}

void Toolbar_Other1(Au.Triggers.TriggerArgs ta = null) {
	var t = new AToolbar("Toolbar_Other1");
	if (!t.SettingsModified) {
		t.AutoSize = true;
	}
	
	t[""] = o => {  };
	t[""] = o => {  };
	t.MenuButton("", m => {
		m[""] = o => {  };
		m[""] = o => {  };
	});
	t.Group();
	t["Close", ""] = o => { t.Close(); };
	
	//auto-hide. Above is the auto-hide part. Below is the visible part.
	t = t.CreateSatelliteOwner();
//	if (!t.SettingsModified) {
	t.Anchor = TBAnchor.TopRight;
	t.Size = (AScreen.Primary.Bounds.Width / 4, 8);
	t.Offsets = new TBOffsets(0, -t.Size.height + 1, 0, 0);
	t.Satellite.Anchor = TBAnchor.BottomLeft | TBAnchor.OppositeToolbarEdgeY;
	t.Satellite.Offsets = new TBOffsets { Bottom = 3 };
//	}
	
	t.Show(ta);
}

#endregion

#region functions


AToolbar CreateToolbarAutoHideAtScreenEdge(AToolbar t, TMEdge edge, TMScreen screen = TMScreen.Primary, Range? range = null) {
	var sat = t;
	t = t.CreateSatelliteOwner();
	
	if(screen == TMScreen.Any) throw new NotSupportedException("TMScreen.Any");
	var sh = screen == TMScreen.OfActiveWindow ? AScreen.Of(AWnd.Active) : AScreen.Index((int)screen);
	var rs = sh.Bounds;
	int x100 = rs.Width, y100 = rs.Height, x25 = x100 / 4, y25 = y100 / 4, x75 = x100 - x25, y75 = y100 - y25;
	
	var se = edge.ToString(); char se0 = se[0];
	
	const int thickness = 8;
	bool vertical = se0 == 'L' || se0 == 'R';
	
	int length = vertical ? y100 : x100, move = 0;
	if(se.Contains("25")) length /= 4; else if(se.Contains("50")) length /= 2;
	if(range.HasValue) {
		var (o, l) = range.Value.GetOffsetAndLength(100);
		length = AMath.Percent(length, l);
		move = AMath.Percent(length, o);
	}
	
	TBAnchor anchor = 0;
	int hidden = thickness - 1;
	TBOffsets k = default;
	switch (se0) {
	case 'T': anchor = TBAnchor.Top; k.Top = -hidden; break;
	case 'R': anchor = TBAnchor.Right; k.Right = -hidden; break;
	case 'B': anchor = TBAnchor.Bottom; k.Bottom = -hidden; break;
	case 'L': anchor = TBAnchor.Left; k.Left = -hidden; break;
	}
	
	switch (edge) {
	case TMEdge.TopInLeft25: k.Right = x75; break;
	case TMEdge.TopInCenter50: k.Left = k.Right = x25; break;
	case TMEdge.TopInRight25: k.Top = -hidden;  break;
	}
	
	t.Anchor = anchor;
	t.Size = vertical ? (thickness, length) : (length, thickness);
	t.Offsets = new TBOffsets(0, -t.Size.height + 1, 0, 0);
	
	sat.Anchor = TBAnchor.BottomLeft | TBAnchor.OppositeToolbarEdgeY;
	sat.Offsets = new TBOffsets { Bottom = 3 };
	return t;
}

#endregion

}
