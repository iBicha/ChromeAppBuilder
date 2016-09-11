var Module = {
	TOTAL_MEMORY: %UNITY_WEBGL_TOTAL_MEMORY%,
	errorhandler: null,			// arguments: err, url, line. This function must return 'true' if the error is handled, otherwise 'false'
	compatibilitycheck: null,
	dataUrl: "%UNITY_WEBGL_DATA_FOLDER%/%UNITY_WEBGL_FILE_NAME%.data",
	codeUrl: "%UNITY_WEBGL_DATA_FOLDER%/%UNITY_WEBGL_FILE_NAME%.js",
	memUrl: "%UNITY_WEBGL_DATA_FOLDER%/%UNITY_WEBGL_FILE_NAME%.mem",
};