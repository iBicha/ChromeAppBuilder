window.onload = function () { 
	var msgP = document.getElementById("msg");
	msgP.innerHTML = msg;
	var okBtn = document.getElementById("okBtn");
	okBtn.onclick = function(){
		alertWindow.close();
	};
	okBtn.focus();
	alertWindow.drawAttention();
};