 Tilda_: Common two-key macro that waits for key input to change global variables etc.

 Good trigger: ` (the Tilda, a rarely used but conveniently located character,
                 at the top left corner of the keyboard)
 Use:
    - If Tilda is the trigger
        - Hit the Tilda key, then press some other key to perform some action
        - If you need to type out a Tilda
              - Use this case statement: case 192: key((192))
              - Then just hit Tilda twice
    - Add more items here as needed

int x=wait(10 KF)
sel x
	case 192: key((192));; to type a Tilda (use if trigger is Tilda)
	case '4': Scroll_Input("scrollFactor")
	case '5': Scroll_Input("deltaFactor")
	case '6': Scroll_Input("forceScroll")

