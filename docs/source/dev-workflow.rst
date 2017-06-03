Developer Workflow
==================

Creating issues and pull requests
---------------------------------

Use this information to determine if creating an issue is the correct thing to do: 

* **Have a question about how HSPI works?** Ask on Stack Overflow and tag with “HSPI". This is for questions about how HSPI works, not for reporting bugs or feature requests. If you have an opinion, or want someone else's opinion, it's probably better to use the HSPI chat link. 
 http://stackoverflow.com/questions/ask?tags=HSPI 

    *Using Stack Overflow, as opposed to the HomeSeer forums, allow us to more easily track questions related to HSPI and their status.*
* **Github issues** This is not for asking questions, providing an opinion, or requesting an opinion. For questions about how HSPI works, use the Stack Overflow link. For opinion stuff, use the HSPI chat link. 
 https://github.com/alexdresko/HSPI/issues 

    *Using the Github issues, as opposed to the HomeSeer forums, allows us to more easily track issues and their status.*
* **HSPI chat**  This channel is great for giving and requesting feedback. Use Stack Overflow for questions about how HSPI works, and Github issues for bugs and feature requests. 
 https://gitter.im/HSPI/Lobby

When you *do* need to create an issue, it should be easy now. Just create an issue or pull request as your normally would in Github and you'll be prompted for any required information. 

ZenHub
------

We use ZenHub_ to enhance the Github issue management process. It's free for public Github repos, so grab the extension if you want to help maintain HSPI. 

Our ZenHub configuration uses the default pipeline:

#. **New Issues** This is where new issues go. 
#. **Icebox** This is for issues that need a little more time to simmer. 
#. **Backlog** This is for approved issues. Generally, I try to only work on issues that are in the Backlog _and_ assigned to the current milestone. Any issues in the Backlog with the "up for grabs" tag can be tackled by any community contributer at any time. 
#. **Blocked** This is for issues that can't be worked on for whatever reason. The reason should be detailed in the issue description. 
#. **In Progress** This is for issues that are in progress. 
#. **Review/Q** This is for issues associated to a PR that is being reviewed. 
#. **Done** This is for issues where the corresponding code has been merged into the Dev branch. 
#. **Closed** This is for issues where the corresponding code has been merged into Master. 

The following link might prove helpful in understanding how we use ZenHub:

ZenHub - Agile GitHub Project Management: https://www.zenhub.com/guides/agile-concepts-in-github


I also super highly recommend reading this book if you have the time -- It's not at all necessary -- It's just a really good book, and it further explains how ZenHub integrates with HSPI.

ZenHub - GitHub Project Management Book - Download Free: https://www.zenhub.com/book/github-project-management


Obivously, the HSPI project isn't Agile in the truest sense, but it's easier for me if we stick with the terminology as much as possible. 

Git branching strategy
----------------------

We use a sloppy rendition of `Gitflow <http://datasift.github.io/gitflow/IntroducingGitFlow.html>`_. Basically, all new work should generally branch from `Dev`. When we feel like we've got enough work to consitute a release, we'll merge `Dev` into `Master`. The primary reason for using this workflow is to prevent frequent builds on the master branch. Every time we build on the master branch, the public nuget package and Visual Studio template are updated. It would be very annoying to HSPI users if they were constantly asked to upgrade HSPI. 