Mass Media Editor

What's New?

The latest realease has added the section headers that fields belong to. Now when you edit a file the fields will be in the correct sections. There was also some backend stuff for versioning so I can easily update versions as well as other fixes.

Upcoming:

I want to add some QoL things like a elapsed timer on commiting changes, and maybe some other things like select field commits or something.

Description:

Mass Media Editor is a tool that it used for editing metadata on Media files in windows. Now you will be able to organize multiple files at a time instead of having to do it manually.


----Changelog version 1.2.3.0

+ Section headers have been added to the edit window. Fields will now appear under their appropriate sections as they do in the Windows file dialog
+ Added a versioning method for easier version updating.

----Changelog version 1.2.1.0

+ Datagrid now expands the entire panel size
	+ Datagrid now doesn't resize based on height and width of entries 
+ Edit Window close now has a close dialog
+ A lot of backend changes.
	+ Media.cs was refactored
		+ There Attributes now have a new custom MediaAttribute class
	+ MessageBoxMgr is now static for more cohesive calling
+ Various bug fixes		

----Changelog version 1.2.0.0

+ Added Error Log access in the menus. Users should now be able to access the log without having to navigate to the MME folder.
     + Error Log entries are a bit more verbose. Added a stack trace for easier troubleshooting. 
+ Added checkboxes to the header columns so that a user can select which fields are edited, instead of all of them.
+ Various bug fixes.

----Changelog version 1.1.0.0

+ Updated the Edit Window UI. 
+ Added a confirmation that changes have been saved when you are editing data.
+ Added a more user friendly way of adding data to multi-value fields. There's now an add/remove button that keeps track of all of them
- In a One step forward two steps back, Empty fields in edit will clear out data instead of being removed. 
+ Various bug fixes.
