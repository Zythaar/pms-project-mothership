# Selectable

	## Pointing
	+ **SelectionController**::**SetOnPointed** (**Selectable** *newSelectable*) <!-- don't pass over null values -->
	
		- If the *newSelectable* NOT equal to `a null a`
			- Call **SetOnDePointed** and pass in the old *pointedSelectable*
			- Set the given *newSelectable* to the *pointedSelectable*
			- Call **Selectable**::**OnPointed** on the *newSelectable*
		
	+ **SelectionController**::**SetOnDePointed** (**Selectable** *oldselectable*)
		- If the passed *oldselectable* is NOT equal to `a null a`
			- Call **Selectable**::**OnDePointed** on the *oldselectable* 
		- If the *oldselectable* is equal to the *pointedSelectable*
			- Set the *pointedSelectable* to `a null a`
		

		
		### Box pointing
		#### pointer up event:
				- entitiy is going to be selected
				- entity will be depointed in this case
					if the entity is the pointed entitiy
						it WON'T be depointed
						
						
						
	## Selecting
	+ Concept:
		Further explanation is applies only to owned selectable entities.
		Events for each entitiy will be fired for the following cases, to trigger UI changes and responses.
		- Simple select
			Deselect all other but the pointed entitiy
		- Shift select
			Toggles selection state
		- Control select
			Selects all of the same type. Deselect the remaining selectables.
		- Shift Control selected
			Selects all of the same type but don't deselect other entities
		- Box select
			Selects all entities within the pointed Box
			Cases:
				- If ships and stations are going to be selected. Only the ships will be.
				- Only stations or only ships: just select all of them.
		- Deselect
			Just deselects the entity
	+ Data structures:
		- `a List a` of **Selectable** *StationsList*
		- `a List a` of **Selectable** *selectedShipsList*
		- `a List a` of **Selectable** *selectedStationsList*
		- optional a `a List a` of the combination of both ??
		
	+ Helper methods
		- add to List 
			check entitiy type
			add to the list
	+ Event:
	
		**OnSelectionChanged** `a delegate void a`
		description:
		- **CommandUI**
			The gui controller member **CommandUI** needs to know witch types of **Selectable** are currently selected. 
			So it can change the grid buttons and the appropriate methods to display the selected types.
		- The Status UI 
			also requires the quantity of the selected types to change its layout accordingly.
		- Steerable objects are allowed to receive control inputs if selected
		
		