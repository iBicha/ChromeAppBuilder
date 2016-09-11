const ChromeAppWindowPrefix = "ChromeApp_Window_";

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

var ChromeBridgeWindowPlugin = {
	addListener: function(eventNamePtr, callback)
    {
		var eventName = ChromeHelper.ToJsString(eventNamePtr);
		var allowedEvents = ["onBoundsChanged", "onClosed", "onFullscreened", "onMaximized", "onMinimized", "onRestored"];
		var eventIndex = allowedEvents.indexOf(eventName);
		if(eventIndex!=-1){
			chrome.app.window.current()[eventName].addListener(function(){
				Runtime.dynCall('v', callback, 0);
			});
		}
    },
    focus: function()
    {
        chrome.app.window.current().focus();
    },
	fullscreen: function()
    {
        chrome.app.window.current().fullscreen();
    },
	isFullscreen: function()
    {
        return chrome.app.window.current().isFullscreen();
    },
	minimize: function()
    {
        chrome.app.window.current().minimize();
    },
	isMinimized: function()
    {
        return chrome.app.window.current().isMinimized();
    },
	maximize: function()
    {
        chrome.app.window.current().maximize();
    },
	isMaximized: function()
    {
        return chrome.app.window.current().isMaximized();
    },
	restore: function()
    {
        chrome.app.window.current().restore();
    },
	drawAttention: function()
    {
        chrome.app.window.current().drawAttention();
    },
	clearAttention: function()
    {
        chrome.app.window.current().clearAttention();
    },
	close: function()
    {
        chrome.app.window.current().close();
    },
	show: function(focused)
    {
        chrome.app.window.current().show(focused);
    },
	hide: function()
    {
        chrome.app.window.current().hide();
    },
	isAlwaysOnTop: function()
    {
        return chrome.app.window.current().isAlwaysOnTop();
    },
	setAlwaysOnTop: function(alwaysOnTop)
    {
        chrome.app.window.current().setAlwaysOnTop(alwaysOnTop);
    },
	setVisibleOnAllWorkspaces: function(alwaysVisible)
    {
        chrome.app.window.current().setVisibleOnAllWorkspaces(alwaysVisible);
    },
	id: function()
    {
		var id = chrome.app.window.current().id;
		id = ChromeHelper.ToCsString(id);
        return id;
    },
	canSetVisibleOnAllWorkspaces: function()
    {
        return chrome.app.window.canSetVisibleOnAllWorkspaces();
    },
	getBounds: function(inner, key)
    {
		var bounds = Boolean(inner) ? chrome.app.window.current().innerBounds : chrome.app.window.current().outerBounds;
		key = ChromeHelper.ToJsString(key);
        return bounds[key];
    },
	setBounds: function(inner, key, value)
    {
		var bounds = Boolean(inner) ? chrome.app.window.current().innerBounds : chrome.app.window.current().outerBounds;
		key = ChromeHelper.ToJsString(key);
		bounds[key] = value;
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
 
MergePlugins(ChromeBridgeWindowPlugin,ChromeAppWindowPrefix);