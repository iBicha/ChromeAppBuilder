const ChromeAppNotificationsPrefix = "ChromeApp_Notifications_";

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
    ToJsString: function (ptr) {
        return Pointer_stringify(ptr);
    },
    ToJsObject: function (ptr) {
        var str = Pointer_stringify(ptr);
        try {
            return JSON.parse(str);
        } catch (e) {
            return null;
        }
    },
    FreeMemory: function (ptr) {
        _free(ptr);
    }
};

var ChromeBridgeNotificationsPlugin = {
    create: function (notificationId, options, callback, callbackId) {
        if (notificationId) {
            notificationId = ChromeHelper.ToJsString(notificationId);
        } 
        notificationId = notificationId || "";
        options = ChromeHelper.ToJsObject(options);
        options.iconUrl = options.iconUrl || '/TemplateData/progresslogo.png';
        chrome.notifications.create(notificationId, options, function (notificationId) {
            if(callback) {
                notificationId = ChromeHelper.ToCsString(notificationId);
                Runtime.dynCall('vii', callback, [notificationId, callbackId]);
                ChromeHelper.FreeMemory(notificationId);
            }
        })
    },
    update: function (notificationId, options, callback, callbackId) {
        notificationId = ChromeHelper.ToJsString(notificationId);
        options = ChromeHelper.ToJsObject(options);
        chrome.notifications.update(notificationId, options, function (wasUpdated) {
            if(callback) {
                Runtime.dynCall('vii', callback, [wasUpdated ? 1 : 0, callbackId]);
            }
        })
    },
    clear: function (notificationId, callback, callbackId) { //function(boolean wasCleared) {...};
        chrome.notifications.clear(notificationId, function (wasCleared) {
            if(callback) {
                Runtime.dynCall('vii', callback, [wasCleared ? 1 : 0, callbackId]);
            }
        })
    },
    clearAll: function (notificationId, callback) {
        //TODO
    },
    getAll: function (callback) {
        chrome.notifications.getAll(function (notifications) {
            console.log('getAll', notifications)
            if(callback) {
                notifications = ChromeHelper.ToCsString(notifications)
                Runtime.dynCall('vi', callback, [notifications]);
                ChromeHelper.FreeMemory(notifications);
            }
        })
    },
    getPermissionLevel: function (callback) {
        chrome.notifications.getPermissionLevel(function (level) {
            level = ChromeHelper.ToCsString(level);
            Runtime.dynCall('vi', callback, [level]);
            ChromeHelper.FreeMemory(level);
        })
    },
    addListener: function (eventNamePtr, callback) {
        var eventName = ChromeHelper.ToJsString(eventNamePtr);

        switch (eventName) {
            case "onClosed":
                chrome.notifications.onClosed.addListener(function (notificationId, byUser) {
                    notificationId = ChromeHelper.ToCsString(notificationId);
                    Runtime.dynCall('vii', callback, [notificationId, byUser ? 1 : 0]);
                    ChromeHelper.FreeMemory(notificationId);
                });
                break;
            case "onClicked":
                chrome.notifications.onClicked.addListener(function (notificationId) {
                    notificationId = ChromeHelper.ToCsString(notificationId);
                    Runtime.dynCall('vi', callback, [notificationId]);
                    ChromeHelper.FreeMemory(notificationId);
                });
                break;
            case "onButtonClicked":
                chrome.notifications.onButtonClicked.addListener(function (notificationId, buttonIndex) {
                    notificationId = ChromeHelper.ToCsString(notificationId);
                    Runtime.dynCall('vii', callback, [notificationId, buttonIndex]);
                    ChromeHelper.FreeMemory(notificationId);
                });
                break;
            case "onPermissionLevelChanged":
                chrome.notifications.onPermissionLevelChanged.addListener(function (level) {
                    level = ChromeHelper.ToCsString(level);
                    Runtime.dynCall('vi', callback, [level]);
                    ChromeHelper.FreeMemory(level);
                });
                break;
            case "onShowSettings":
                chrome.notifications.onShowSettings.addListener(function () {
                    Runtime.dynCall('v', callback, 0);
                });
                break;
        }
    }


};

function MergePlugins(plugins, prefixes) {
    if (!Array.isArray(plugins)) {
        plugins = [plugins];
    }
    if (!Array.isArray(prefixes)) {
        prefixes = [prefixes];
    }
    for (var i = 0; i < plugins.length; i++) {
        //keys
        for (var key in plugins[i]) {
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

MergePlugins(ChromeBridgeNotificationsPlugin, ChromeAppNotificationsPrefix);
