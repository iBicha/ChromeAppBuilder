##### Disclaimer: Google is trying to get rid of Chrome apps on Windows, OS X and Linux in favor of webapps and Chrome extensions. The best use of this editor extension is for targeting ChromeOS which "*will remain supported and maintained for the foreseeable future.*"
###### read more about it here: https://blog.chromium.org/2016/08/from-chrome-apps-to-web.html

###### Since Google is killing chrome apps for Chrome on Windows, OS X and Linux, and now supporting Android and Linux apps on ChromeOS, I don't see the value in maintaining this repo.


![Logo](https://raw.githubusercontent.com/iBicha/ChromeAppBuilder/master/Assets/ChromeAppBuilder/Editor/Resources/logo.png)
# Chrome App Builder
Chrome app builder is an editor extension and API for Unity3D to export games as google chrome apps. It is designed to look just like the build settings of other platforms.

<img src="https://raw.github.com/iBicha/ChromeAppBuilder/master/Screenshots/screen1.png" height="420">    <img src="https://raw.github.com/iBicha/ChromeAppBuilder/master/Screenshots/screen2.png" height="420">    <img src="https://raw.github.com/iBicha/ChromeAppBuilder/master/Screenshots/screen3.png" height="420">

### Known issues
Bad builds with pre-built engine option.

### How does it work?
Mostly, it is based on the webgl player of unity, which means the **WebGL module for unity needs to be installed**. Combined with a template, and and an extension to fix the chrome related stuff, and it's all good.
In addition, it offers an API to access chrome functions from within unity.
### Installation
Just add the content of the assets folder into the assets folder of your project. Choose "window -> chrome app builder" to access the different settings and the build button!
### Is it on the asset store?
No, but it should be. I guess because of the nature of the constant change of the way the WebGL player works, it is hard to maintain this project for many Unity versions.

### Requirement
You need unity3d installed (~~5.3 and above recommended~~ you probably need the latest version, since Chrome app builder uses private unity apis that keep changing all the time, and we try to keep up and update it. so best way is to test.) with unity webgl module. And of course, you need google chrome on your computer.

**Last commit worked with: Unity 2017.3.0f2**

### API

Calling Chrome API from Unity can be done from the following classes:
- `Chrome.App.Browser` //To open a web page in a new tab [ref](https://developer.chrome.com/apps/browser)
- `Chrome.App.Identity` //To get information about the currently connected user on Chrome [ref](https://developer.chrome.com/apps/identity)
- `Chrome.App.Notification` //To display notifications [ref](https://developer.chrome.com/apps/notifications)
- `Chrome.App.Power` //For power management [ref](https://developer.chrome.com/apps/power)
- `Chrome.App.Window` //For window related operations (maximize, minimize, fullscreen, etc.) [ref](https://developer.chrome.com/apps/app_window)

The `Chrome.Social` namespace can be used to use OAuth on various social media (Facebook, Linkedin, Instagram)

### Todos

 - Fix template manager so it works EXACTLY as the internal unity template manager
 - Add demo scene (one that doesn't suck)
 - Implement getAuthToken
 - Add more chrome APIs (Native sockets, In-app purchase and so on)
 - Add a setting for facebook and instagram client id (currently need to set in the code) and maybe add more social media and such 
 - Create good documentation
 - Create a permission and a minimum chrome version detector
 - Other fixes are to be made. TL;DR

License
----

MIT

### Development

Pull requests are welcome.
