(function() {
	var currentWindow = chrome.app.window.current();
	var canvas = document.getElementById("canvas");
	
	var originalWidth = %UNITY_WIDTH%;
	var originalHeight = %UNITY_HEIGHT%;
	
	function ResizeCanvas(){
		if(currentWindow.isFullscreen()){
			var screenWidth = currentWindow.outerBounds.width;
			var screenHeight = currentWindow.outerBounds.height;
		}else{
			var screenWidth = currentWindow.innerBounds.width;
			var screenHeight = currentWindow.innerBounds.height;
		}
		if(%UNITY_CHROME_LOCK_ASPECT_RATIO%){			
			var ratio = Math.max(originalWidth/screenWidth,originalHeight/screenHeight);
			var width = originalWidth/ratio;
			var height = originalHeight/ratio;
			var x = screenWidth/2 - width/2;
			var y = screenHeight/2 - height/2;
			
			canvas.style.left = x + 'px';
			canvas.style.top = y + 'px';
			canvas.width = width;
			canvas.height = height;		  
		}else{
			canvas.width = screenWidth;
			canvas.height = screenHeight;		  
		}
	}
	
	currentWindow.onBoundsChanged.addListener(ResizeCanvas);
	currentWindow.onFullscreened.addListener(ResizeCanvas);
	currentWindow.onMaximized.addListener(ResizeCanvas);
	currentWindow.onRestored.addListener(ResizeCanvas);
 	
	ResizeCanvas();
	
})();