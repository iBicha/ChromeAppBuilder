const ChromeAppPowerPrefix = "ChromeApp_Power_";

var ChromeHelper = {
    ToCsString: function (str) 
    {
        if (typeof str === 'object') {
            str = JSON.stringify(str);
        }
        var bufferLength = lengthBytesUTF8(str) + 1;
        var buffer = _malloc(bufferLength);
        stringToUTF8(str, buffer, bufferLength);
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

var ChromeBridgePowerPlugin = {
	requestKeepAwake: function(levelPtr)
    {
		var level = ChromeHelper.ToJsString(levelPtr);
        chrome.power.requestKeepAwake(level);
    },
	releaseKeepAwake: function()
    {
        chrome.power.releaseKeepAwake();
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
 
MergePlugins(ChromeBridgePowerPlugin,ChromeAppPowerPrefix);