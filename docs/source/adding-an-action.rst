Adding an action to your plugin
===============================

The following functions are used in creating an action:

* **public override int ActionCount()**
    This function returns the number of actions found in your plugin.

* **public override bool ActionConfigured(IPlugInAPI.strTrigActInfo actionInfo)**  
    A verification if an action (given by actionInfo) is configured correctly (all relevant data is set up correctly). ActionInfo is a serilalized object which must be deserialized to a HsCollection (Dictionary of string and object).

* **public override string ActionFormatUI(IPlugInAPI.strTrigActInfo actionInfo)**
    This function returns the text/UI shown to the user when the action is formatted correctly (ActionConfigured returns true) and the action is collapsed.

* **public override string ActionBuildUI(string uniqueControlId, IPlugInAPI.strTrigActInfo actionInfo)**
    This function builds the html UI where the user can set up the action in Events.

* **public override IPlugInAPI.strMultiReturn ActionProcessPostUI(NameValueCollection postData, IPlugInAPI.strTrigActInfo actionInfo)**
    Processing of any configuring done in the UI which is sent back from Homeseer.

* **public override string get_ActionName(int actionNumber)**
    The name of your action for the actionnumber given. Homeseer will use 1 as the first action.

* **public override bool HandleAction(IPlugInAPI.strTrigActInfo actionInfo)**
    This it the function handling the action when it is triggered in Homeseer.

General functions that you will use
-----------------------------------
* **public override string InitIO(string port)**
	This is where the initialization of the plugin happens. Keep it quick since Homeseer only waits a couple of seconds at the most during init. Any slower and the plugin is listed as not functioning.

* **public override void ShutdownIO()**
	This is where actions before shutdown and disconnect are done. Make sure all your stuff is ended correctly so noting is left "haning" during shutdown.

Also you might want to add a config page which involves adding a webpage. More of this in later documentation.