Change Log
==========

vNext
-----

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

