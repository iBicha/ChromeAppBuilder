window.alert = function(msg) {
	chrome.app.window.create('Alert/alert.html', {
		'innerBounds': {
		  'width': 450,
		  'height': 150
		},
		'frame': 'none',
		'resizable': false
	  }, 
	  function(createdWindow) { 
		createdWindow.contentWindow.msg = msg; 
		createdWindow.contentWindow.alertWindow = createdWindow;
	  });			
};