Getting Started
###############

These instructions assume you have a default HomeSeer installation on the same computer that you’ll be developing on. If that is not the case, you can still follow these instructions, and then refer to the remaining HSPI documentation to adapt the project for your environment. 

1. In Visual Studio, open “Tools > Extensions and Updates”

    .. figure:: _static/tools-extensions-and-updates.png

#. Click the “Online” tab, search for “HomeSeer”, and click “Download”. 

    .. figure:: _static/extensions-and-updates-search.png

    Alternatively, download it from https://marketplace.visualstudio.com/items?itemName=thealexdresko.HomeSeerTemplates 

#. Once installed, you can “File > New Project” to create a HomeSeer plugin. The templates can be found under “Templates > Visual C# > HomeSeer”. 
 
    .. figure:: _static/file-new-project.png

    Refer to the :ref:`templates list <the-templates>` to determine which template is most appropriate.  

#. F5 to launch the project and verify connectivity to HomeSeer. 

    If all goes well, you should see a success message in the console window:

    .. figure:: _static/connection-success.png

    Further, you should see your plugin listed under "PLUG-INS > MANAGE > Remote Plug-Ins":

    .. figure:: _static/remote-plugin-connected.png