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

![accept login](http://users.metropolia.fi/~anttita/WpRepo/repo001.PNG)![albums view](http://users.metropolia.fi/~anttita/WpRepo/repo002.PNG)![photos on single album](http://users.metropolia.fi/~anttita/WpRepo/repo003.PNG)![single photo with comments](http://users.metropolia.fi/~anttita/WpRepo/repo004.PNG)![pinned program to start](http://users.metropolia.fi/~anttita/WpRepo/repo005.PNG)

What has been done and what could be the future plans
===============================
All default projects required features are implemented somehow. Some better than the others.
It is Silverlight app for Windows Phone, localization ready, MVVM architectured. It uses windows phone navigation, tombstoning is handled, live tile can be made and it has application bars.

From UI Baseline it has splash screen, it is tappable, works with both themes and utilizes Sileverlight toolkit for WP. From optionals it supports both orientations. Mandatory functionality can be found also, aSkyDrive utilizes SkyDrives RESTful api and user is able to do interactive action with the cloud service.

From the actual specifications there is login and logout features. User can take, upload and download pictures. User is also able to view single picture and set of pictures. From the other features Commenting (viewing and adding) has been implemented.

What was written in the project spefication document live tile has better features than expected. Did manage to get that custom image to work. It was quite hard at first.

First future plan is to solve the problem with the photo uploading with real device. Maybe need to go buy one to solve it actually. Then add more features to the application. Example showing users quota status. Deleting albums and photos. Add albums inside albums. Rating photos etc. Lots of more could be done with selected api.

One drawback with the commenting was the situation how commenting works on skydrive. If photos and albums are not added to shared folder the photos cannot be commented. From the web user can share albums to specific people by activating sharing with a link. When that has been done all photos added to that album can be commented. But if the albums is not shared the api cannot change the sharing settings for single photo or single album. That sharing is inheriting to the photos from the album so user has to share only the album to get the commenting to work with photos on that specified album.

Used Libraries
===============================
Microsoft Live Controls for handling the sesssion to the skydrive.
Microsoft XNA Library is used ONLY for saving downloaded photo to the phone library.
Microsoft toolkit is used for the application bar buttons and menu items
