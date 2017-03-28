# Chrome App Builder
Chrome app builder is an editor extension and API for Unity3D to export games as google chrome apps. It is designed to look just like the build settings of other platforms.

![](https://s31.postimg.io/ht2kbugaz/image.png)      ![](https://s31.postimg.io/ol7qgfyij/image.png)
### How does it work?
Mostly, it is based on the webgl player of unity. Combined with a template, and and an extension to fix the chrome related stuff, and it's all good.
In addition, it offers an API to access chrome functions from within unity.
### Installation
Just add the content of the assets folder into the assets folder of your project. Choose "window -> chrome app builder" to access the different settings and the build button!
### Is it on the asset store?
No, but it should be. I guess i should do that, soon?
### Requirement
You need unity3d installed (5.3 and above recommended) with unity webgl module. And of course, you need google chrome on your computer.
### Todos

 - Fix template manager so it works EXACTLY as the internal unity template manager
 - Add demo scene (one that doesn't suck)
 - Implement getAuthToken
 - Add more chrome APIs (Notifications, Native sockets, In-app purchase and so on)
 - Add a setting for facebook and instagram client id (currently need to set in the code) and maybe add more social media and such 
 - Create good documentation
 - Create a permission and a minimum chrome version detector
 - Other fixes are to be made. TL;DR

License
----

MIT

### Development

Pull requests are welcome.
