chrome.app.runtime.onLaunched.addListener(function() {
	var nWindows = chrome.app.window.getAll().length;
	if (nWindows === 0) {
		var windowSettings =  {
			'innerBounds': {
			  'width': %UNITY_WIDTH%,
			  'height': %UNITY_HEIGHT%
			},
			'frame': '%UNITY_CHROME_FRAMELESS%',
			'state': '%UNITY_CHROME_WINDOW_STATE%',
			'resizable':%UNITY_CHROME_WINDOW_RESIZABLE%
		};
		
		if(%UNITY_CHROME_CONSTRAINTS%){
			windowSettings.innerBounds.minWidth =  %UNITY_CHROME_MIN_WIDTH%;
			windowSettings.innerBounds.minHeight =  %UNITY_CHROME_MIN_HEIGHT%;
			windowSettings.innerBounds.maxWidth =  %UNITY_CHROME_MAX_WIDTH%;
			windowSettings.innerBounds.maxHeight =  %UNITY_CHROME_MAX_HEIGHT%;
		}
		
		chrome.app.window.create('index.html',windowSettings);		
	}else{
		chrome.app.window.getAll()[0].focus();
	}

});