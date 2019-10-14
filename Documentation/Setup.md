# Setup

This is assuming the Unity game doesn't have Experience Capture in it already, an
upgrade guide will come later. The *SetupTestGame/* folder has an example 
game that can have the client integrated into it for reference.

### Download Asset Package

Get the client [here](https://github.com/jhburns/ExperienceCapture/releases/tag/1.0.0) or in
the same folder as the docs if in Google Drive. 

Click on the first link 'ExperienceCaptureClient.unitypackage' to download it, no extraction needed.

### Import Into Unity Game

![Opening asset menu](images/import_package.png)

In the Unity Editor, go to Assets -> Import Package -> Custom Package... and select it.

That will open a file-browser so you can navigate to where 'ExperienceCaptureClient.unitypackage'
is downloaded and open it. 

![Importing package](images/import_menu.png)

You should now have a pop-up with all of the assets selected by default, click on 
'Import' to add the package. (This may take a while)

**Check:** If everything worked, there should be a new folder called *ExperienceCapture/* in your assets folder.

### Create Setup Scene