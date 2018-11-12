What is it?
===========

HSPI consists of the following:

Project templates for Visual Studio 2015 and Visual Studio 2017
---------------------------------------------------------------

With HSPI, getting started with HomeSeer development is as simple as:

1.	Locate and install the HSPI extension in Visual Studio via “Tools > Extensions and Updates” or download it from https://marketplace.visualstudio.com/items?itemName=thealexdresko.HomeSeerTemplates2
2.	File > New Project > HomeSeer > Select a template
3.	F5 to launch the project and verify connectivity to HomeSeer.

HSPI comes with a growing list of project templates you can use to accelerate your HomeSeer development.

The HSPI framework and nuget package
------------------------------------

I imagine most HomeSeer development efforts end in failure. Mine did. Out of the box, it’s just painful.

So, I scoured the HomeSeer developer forums, collecting all the code and tips possible. I then crammed everything into a nuget package (https://www.nuget.org/packages/HSPI) and used my coder nerd wizardry to bring it all together.

With HSPI, we can continue to refine this framework into something that makes HomeSeer plugin development mind-numbingly simple.

HomeSeerNuget
-------------

A nuget package containing the three assemblies required for HomeSeer development:

* HomeSeerAPI.dll
* HSCF.dll
* Scheduler.dll

Many first time HomeSeer developers get stumped working with the HomeSeer-developed plugin samples (https://forums.homeseer.com/showthread.php?t=160064) due to these assemblies not being included with the samples. **This nuget package is not specific to HSPI and can be used in any HomeSeer plugin, including the HomeSeer-developed plugin samples.** Because of its generic nature, HomeSeerNuget (https://www.nuget.org/packages/Homeseer/) is maintained in a separate repository (https://github.com/alexdresko/HomeSeerNuget).
