Adding an action to your plugin
===============================

The following functions are used in creating an action:

* **public override int ActionCount()**
    The number of actions found in your plugin.s

* **public override bool ActionConfigured(IPlugInAPI.strTrigActInfo actionInfo)**  
    A verification if an action (given by actionInfo) is configured correctly (all relevant data is set up correctly).

* **public override string ActionFormatUI(IPlugInAPI.strTrigActInfo actionInfo)**
    This function returns the text/UI shown to the user when the action is formatted correctly and collapsed.

* **public override string ActionBuildUI(string uniqueControlId, IPlugInAPI.strTrigActInfo actionInfo)**
    This function builds the html UI where the user can set up the action.

* **public override IPlugInAPI.strMultiReturn ActionProcessPostUI(NameValueCollection postData, IPlugInAPI.strTrigActInfo actionInfo)**
    Processing of any configuring done in the UI which is sent back from Homeseer.

* **public override string get_ActionName(int actionNumber)**
    The name of your action for the actionnumber given. Homeseer will use 1 as the first action.

* **public override bool HandleAction(IPlugInAPI.strTrigActInfo actionInfo)**
    This it the function handling the action when it is triggered in Homeseer. 