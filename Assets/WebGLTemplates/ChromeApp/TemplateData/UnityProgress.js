function UnityProgress (dom) {
	NProgress.start();
	
	this.progress = 0.0;
	this.message = "";
	this.dom = dom;

	var parent = dom.parentNode;

	var background = document.createElement("div");
	background.style.background = "#4D4D4D";
	background.style.position = "absolute";
	parent.appendChild(background);
	this.background = background;

	var logoImage = document.createElement("img");
	logoImage.src = "TemplateData/progresslogo.png"; 
	logoImage.style.position = "absolute";
	parent.appendChild(logoImage);
	this.logoImage = logoImage;

	var messageArea = document.createElement("p");
	messageArea.style.position = "absolute";
	parent.appendChild(messageArea);
	this.messageArea = messageArea;

	
	
	this.SetProgress = function (progress) { 
		if (this.progress < progress){
			this.progress = progress;
			NProgress.set(progress);
		}		
		this.messageArea.style.display = "none";
		this.Update();
	}

	this.SetMessage = function (message) { 
		this.message = message; 
		this.background.style.display = "inline";
		this.logoImage.style.display = "inline";
		this.Update();
	}

	this.Clear = function() {
		this.background.style.display = "none";
		this.logoImage.style.display = "none";
		NProgress.done();
	}

	this.Update = function() {
		this.background.style.top = this.dom.offsetTop + 'px';
		this.background.style.left = this.dom.offsetLeft + 'px';
		this.background.style.width = this.dom.offsetWidth + 'px';
		this.background.style.height = this.dom.offsetHeight + 'px';

		var logoImg = new Image();
		logoImg.src = this.logoImage.src;
 
		this.logoImage.style.top = this.dom.offsetTop + (this.dom.offsetHeight * 0.5 - logoImg.height * 0.5) + 'px';
		this.logoImage.style.left = this.dom.offsetLeft + (this.dom.offsetWidth * 0.5 - logoImg.width * 0.5) + 'px';
		this.logoImage.style.width = logoImg.width+'px';
		this.logoImage.style.height = logoImg.height+'px';

		this.messageArea.style.left = 0;
		this.messageArea.style.width = '100%';
		this.messageArea.style.textAlign = 'center';
		this.messageArea.innerHTML = this.message;
	}

	this.Update ();
}