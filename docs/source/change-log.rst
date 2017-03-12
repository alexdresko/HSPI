Change Log
==========

vNext
-----

Contributers
^^^^^^^^^^^^

* 3/11/2017 10:27:59 AM by AD:   Switched to more of a gitflow (http://nvie.com/posts/a-successful-git-branching-model/) based branching strategy. The biggest reason for the switch is because I'm trying to make better use of the build system -- Basically, I don't want to continuously publish an updated nuget package and VSIX extension for every arbitrary change we make. Having a ``dev`` branch allows us to stage blocks of changes before merging ``dev`` into ``master``. It's the merging of ``dev`` into ``master`` that triggers the nuget & VSIX publishing, so you can see why merging directly into ``master`` often would be problematic. Maybe it's just me, but I think it's annoying with a nuget package or VS extension is being updated multiple times a day. 
* 3/11/2017 10:28:05 AM by AD:   I closed out the "Good enough to use" milestone (https://github.com/alexdresko/HSPI/milestone/1) (because it is), and started "Definitive" https://github.com/alexdresko/HSPI/milestone/2

Templates
^^^^^^^^^

* 3/11/2017 7:35:13 PM by AD: The original HomeSeer provided VB.NET templates are included in the VS extension! For the most part, they are completely unmodified. I simply included them in the extension as a reference. For most cases, you'll want to start with the custom HSPI templates that come with the extension. 
* 3/11/2017 7:37:30 PM by AD:   The binary files created by the templates automatically conform to the standard HS plugin structure wherein the plugin's dependencies are in a subdirectory called ``bin/<plugin name>``. This directory structure enables plugins to avoid version conflicts with other plugins that depend on different versions of the same dependencies. Installing your plugin into HomeSeer is as simple as copying the entire output directory (``/bin/(debug|release)``) into your HomeSeer directory (typically ``C:\Program Files (x86)\HomeSeer HS3``). 

3/10/2017
---------

Extension
^^^^^^^^^

* Long story short: **YOU MAY HAVE TO UNINSTALL THE OLD PLUGIN AND INSTALL THE NEW PLUGIN TO CONTINUE GETTING UPDATES** I thought I configured the original extension incorrectly in the Visual Studio marketplace, and Microsoft doesn't provide a way to change the setting I thought I messed up. So I removed the old extension from the gallery. That's why you get a 404 when you to go https://marketplace.visualstudio.com/items?itemName=thealexdresko.HomeSeerTemplates. The new location's forever home is at https://marketplace.visualstudio.com/items?itemName=thealexdresko.HomeSeerTemplates-18379 (arrrggg). I'm pretty frustrated by the whole experience and -- worse -- it doesn't even look like the setting makes a difference. I'm trying to automate publishing of the VSIX to the marketplace from the CI build, but I just don't think it's possible anymore. 

Contributers
^^^^^^^^^^^^
* Contributing to the HSPI .sln now requires Visual Studio 2017 (You can install install the templates and consume the HSPI nuget package on VS 2015)
* All C# builds in HSPI are validated against a fairly strict set of static code analysis rules in an effort to automate code quality checks. 
* A Resharper "Code Cleanup" setting called "HSPI" helps to ensure all code follows the same standards.
* ``HspiBase`` was updated to eliminate many of HS's poor naming conventions from ``IPluginAPI``. It's quite a bit easier to understand the interface now. 
* HSPI: Converted all string concatenation to interpolation. All the manually constructed HTML is much easier to read now. 
* HSPI: massive Resharper cleanup on the entire HSPI codebase. The code is so much easier to read. 
* HSPI: removed some ``ref`` code that simply wasn't necessary. 
* HSPI: First external contribution everrrrr! https://github.com/alexdresko/HSPI/pull/43 Thanks @dmurphy
* Contributers: Testing the extension is now much easier. A super fancy Powershell script does all the work of copying the ".Dev" code into the project template, and makes sure requisite nuget packages are updated in the correct location. You just press F5 and an experimental instance of VS will start. From there, you can "File > New Project" and verify everything works. 

