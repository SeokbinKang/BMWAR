# BMWAR

An Android/AR application that allows for projecting a virtual 3D trunk into the environment and testing if physical items can be put in the trunk.

[Project Requirement](https://github.com/BMWGroupTechnologyOfficeUSA/dli-coding-challenge/tree/master/ar-experience#design-guidelines)

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

### Prerequisites

* Unity 2019.2.8f1 or later + Android SDK v26 or later
* [ARCore SDK for Unity/Android](https://developers.google.com/ar/develop/unity/quickstart-android)
* Visual Studio 2017 or later
* An Android device that support Android v26 or later and ARCore.Best display resolution: 2560 x 1600.

### Installing

* Connect the Android device to the development machine and set up [ADB connection](https://developer.android.com/studio/command-line/adb). 
* Open the project in Unity
* Open the scene at Assets/Scenes/BMW_ARProject.unity
* File > Build Setting > Build and Run

OR 

* Install export\bmwar.apk directly to the device

## Usase

### User Interface Overview

![UI Overview](https://github.com/SeokbinKang/BMWAR/blob/master/Screenshots/UIOverview2.png)

### Select a car model
User can select a model by tapping it on the screen.
![Select a car](https://github.com/SeokbinKang/BMWAR/blob/master/Screenshots/UICarSelection.png)

### Place a trunk on the surface
User can place a virtual trunk by tapping a point on the surface. The detected surface is visualized with white polygons

### Measure physical items
User can measure an item by tapping its four bottom corners. The current implementation does not support estimating the height. The height is fixed as 0.3 meter. Once measured, each item is augmented with a 3D bounding box (red cube)

### Auto Fit
User can test whether the items fit into the trunk by tapping "Auto Fit" button. The trunk (blue cube) is filled with individual items (red cbues)

## Built With

* [Unity3D for Android](https://docs.unity3d.com/Manual/android-GettingStarted.html) - 3D App
* [ARCore](https://developers.google.com/ar) - AR Support
* [3DContainerPacking](https://github.com/davidmchapman/3DContainerPacking) - Automatic Packing Algorithm

## Tested With
* Galaxy Tab S5e (Android v26)

## Authors

* **Seokbin Kang** - *Initial work* - [BMWAR](https://github.com/SeokbinKang/BMWAR)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments


 
