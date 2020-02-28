# Setup

This is assuming the Unity game doesn't have already Experience Capture installed.

## Download Asset Package

Get the latest client [by looking through here](https://github.com/jhburns/ExperienceCapture/releases).

Click on the first link 'ExperienceCaptureClient.unitypackage' to download it, no extraction needed.

## Remove Old Package

Delete the folder `ExperienceCapture/` from your game.

## Import Into Unity Game

![Opening asset menu](images/import_package.png)

In the Unity Editor, go to Assets -> Import Package -> Custom Package... and select it.

That will open a file-browser so you can navigate to where 'ExperienceCaptureClient.unitypackage'
is downloaded and open it. 

![Importing package](images/import_menu.png)

You should now have a pop-up with all of the assets selected by default. Install **everything** by clicking on
'Import' to add the package.

**Check:** If everything worked, there should be a new folder called `ExperienceCapture/` inside your assets folder.

## Configure Setup Scene

- Go into the `ExperienceCapture/` folder.
- Select the called 'SetupEC'.
- Click on the `SetupCapture` object in the Unity Hierarchy.
- Change the `Scene To Load` value to whatever scene you want loaded first.

![Scene to load](images/scene_to_load.png)

**Check:** Pressing the Start Session button in the 'SetupEC' scene should load the scene you provided.
There should also be a game object with the name `HandleCapturing` under the 'DontDestroyOnLoad' portion of the Unity Hierarchy.