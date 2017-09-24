# Command UI

Grid GUI Layout
	 - 15 Buttons
	 - Buttons will be en/disabled **OnSelecetionChanged** event from the **SelectionController**
	 - The quantity and types of the current selected  **Selectables** will determine the responding Layout
	 - The Layout is modulised to different abilities / commands
	 - Each command is asigned to one Button at one time
		+ Regulary the asignment stays. In some cases a command can link to a different Button (e. g. overlapping functionallity)
	 
 # Program flow
	1. The **OnSelecetionChanged** callback will be fired
	2. The **CommandUI** gets the list of **Selectables** entities
	3. The types will be checked for some conditions
		- ships and stations
		- different types or only one type
		- nothing selected
	4. The layout for the chosen type will be loaded
	5. The Buttons and functions change to the set layout

# CommandController
	- holds the methods that are executed via the Command action response
	- provides code blocks, variables, states etc.
	- events to be invoked
	
# PlayerInputController ??
	- will be informed about steerable objects
	- Shoots rays to set Positions for commands (e.g. move command)
	- Pointer Entity State Ray from here ??
	- this component in the CommandUI ??
	- steerable objects start / stops listening to commands **OnSelecetionChanged**

# CommandExecuteController ??
	- listeneds to events from the CommandController (hooked up via **OnSelecetionChanged**) and sends the to the actual objects (e.g. the ship that will be moved) ???

# Command Layout
	- Array of Command Button Objects
	
# Command Button Object (Serializable Data class)
		- Reference to a Button via `a enum a`**CommandButtonName**
		- Letter for the keyboard key to invoke the command `a enum a`**KeyCode** / Input Axis button
		- Icon for Button
		- CommandActionResponse asset: A callback to be invoked when hit the button (listener will execute the specific action)
 
# Command Action Response (Method)
	- **ScriptableObject** containing a method
	- write in the class
	- executed in a Command Controller class
	- assets created via Create Asset Menu
	- assets can be hooked up via editor
 
## Ship only Commands
	- Move
	- Stop
	- Hold Position
	- Patrol
	- Attack Move
	
### Ship Type specific
	+ Worker
		- Build Small
		- Build Medium
		- Gather
	 