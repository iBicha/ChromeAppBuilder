const ChromeAppIdentityPrefix = "ChromeApp_Identity_";

var ChromeHelper = {
	ToCsString: function(str)
    {
		if(typeof str === 'object'){
			str = JSON.stringify(str);
		}
		var buffer = _malloc(lengthBytesUTF8(str) + 1);
		writeStringToMemory(str, buffer);
		return buffer;
    },
	ToJsString: function(ptr)
    {
		return Pointer_stringify(ptr);
    },
	ToJsObject: function(ptr)
    {
		var str = Pointer_stringify(ptr);
		try{
			return JSON.parse(str);
		}catch(e){
			return null;
		}
    },
	FreeMemory: function(ptr)
	{
		_free(ptr);
	}
};

var ChromeBridgeIdentityPlugin = {
	getAuthToken: function(interactive, scopesPtr, callback,callbackId)
    {
		var details = {
			 interactive:Boolean(interactive)
		};
		if(scopesPtr){
			details.scopes=ChromeHelper.ToJsString(scopesPtr).split(",");
		}
		chrome.identity.getAuthToken(details,function(token){
			 token = token || "";
			 token = ChromeHelper.ToCsString(token);
			 Runtime.dynCall('vii', callback, [token,callbackId]);
			 ChromeHelper.FreeMemory(token);
		 });
    },
	getProfileUserInfo: function(callback,callbackId)
    {
		chrome.identity.getProfileUserInfo(function(userInfo){
			userInfo = ChromeHelper.ToCsString(userInfo);
			Runtime.dynCall('vii', callback, [userInfo,callbackId]);
			ChromeHelper.FreeMemory(userInfo);
		});
    },
	launchWebAuthFlow: function(urlPtr, interactive, callback, callbackId)
    {
		 var details = {
			 interactive: Boolean(interactive),
			 url:ChromeHelper.ToJsString(urlPtr)
		 };
		 chrome.identity.launchWebAuthFlow(details,function(responseUrl){
			if (chrome.runtime.lastError) {
				Runtime.dynCall('vii', callback, [0,callbackId]);
				return;
			}
			 responseUrl = ChromeHelper.ToCsString(responseUrl);
			 Runtime.dynCall('vii', callback, [responseUrl,callbackId]);
			 ChromeHelper.FreeMemory(responseUrl);
		 });
    },
	removeCachedAuthToken: function(tokenPtr)
    {
		var token = ChromeHelper.ToJsString(tokenPtr);
		chrome.identity.removeCachedAuthToken({token:token});
    },
	getRedirectURL: function(pathPtr)
    {
		var path = ChromeHelper.ToJsString(pathPtr);
		var redirectURL = chrome.identity.getRedirectURL(path);
		console.log('RedirectURL: ' + redirectURL);
		redirectURL = ChromeHelper.ToCsString(redirectURL);
		return redirectURL;
    },
	onSignInChangedAddListener: function(callback, callbackId)
    {
		chrome.identity.onSignInChanged.addListener(function(account, signedIn){
			account.signedIn = signedIn;
			account = ChromeHelper.ToCsString(account);
			Runtime.dynCall('vi', callback, [account, callbackId]);
			ChromeHelper.FreeMemory(account);
		});
    }
};

function MergePlugins(plugins, prefixes){
	if(!Array.isArray(plugins))
	{
		plugins = [plugins];
	}
	if(!Array.isArray(prefixes))
	{
		prefixes = [prefixes];
	}
	for(var i = 0;i<plugins.length;i++)
	{
		//keys
		for (var key in plugins[i]){
			if (plugins[i].hasOwnProperty(key)) {
				 plugins[i][prefixes[i] + key] = plugins[i][key];
				 delete plugins[i][key];
			}
		}
		//helper
		plugins[i].$ChromeHelper = ChromeHelper;
		autoAddDeps(plugins[i], '$ChromeHelper');
		//merge
		mergeInto(LibraryManager.library, plugins[i]);
	}
} 

MergePlugins(ChromeBridgeIdentityPlugin,ChromeAppIdentityPrefix);