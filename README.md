school_wp_myProject
===================

Windows Phone -course project work. To use this code you need to input your own clientId to the Microsoft.Live.Controls.SignInButton at the /UserControls/LoginPrompt.xaml (or in the code behind LoginPrompt.xaml.cs). 


General information
===================

aSkydrive gives you access to your skydrive albums and photos on those albums. You can download your skydrive photos to your phone and upload new photos to your skydrive acocunt with just a few button click.

This version supports now following features:
- View your albums
- Add new albums
- View photos on your albums
- Add new photos to your albums (from current phone photos or take new ones with your phones camera)
- Can view larger version of your photo
- Also views your photo comments
- You can add comments to your photos
- Localized to EN and FI languages
- Pinning to start
- Usable with both dark and light themes
- Every screen (with popup also) is usable with Portrait or Landscape orientation
- It can handle tombstoning but to continue using your application you have to refresh the page to sign in again

![accept login](http://users.metropolia.fi/~anttita/forWpRepo/repo001.PNG)![albums view](http://users.metropolia.fi/~anttita/forWpRepo/repo002.PNG)![photos on single album](http://users.metropolia.fi/~anttita/forWpRepo/repo003.PNG)![single photo with comments](http://users.metropolia.fi/~anttita/forWpRepo/repo004.PNG)![pinned program to start](http://users.metropolia.fi/~anttita/forWpRepo/repo005.PNG)

What has been done and what could be the future plans
===============================
All default projects required features are implemented somehow. Some better than the others.
It is Silverlight app for Windows Phone, localization ready, MVVM architectured. It uses windows phone navigation, tombstoning is handled, live tile can be made and it has application bars.

From UI Baseline it has splash screen, it is tappable, works with both themes and utilizes Sileverlight toolkit for WP. From optionals it supports both orientations. Mandatory functionality can be found also, aSkyDrive utilizes SkyDrives RESTful api and user is able to do interactive action with the cloud service.

From the actual specifications there is login and logout features. User can take, upload and download pictures. User is also able to view single picture and set of pictures. From the other features Commenting (viewing and adding) has been implemented.

First future plan is to solve the problem with the photo uploading with real device. Maybe need to go buy one to solve it actually.

Used Libraries
===============================
Microsoft Live Controls for handling the sesssion to the skydrive.
Microsoft XNA Library is used ONLY for saving downloaded photo to the phone library.
Microsoft toolkit is used for the application bar buttons and menu items
