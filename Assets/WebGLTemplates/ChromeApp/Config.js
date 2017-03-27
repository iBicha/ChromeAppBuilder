//Old config glue
// var Module = {
// 	TOTAL_MEMORY: %UNITY_WEBGL_TOTAL_MEMORY%,
// 	errorhandler: null,			// arguments: err, url, line. This function must return 'true' if the error is handled, otherwise 'false'
// 	compatibilitycheck: null,
// 	dataUrl: "%UNITY_WEBGL_DATA_FOLDER%/%UNITY_WEBGL_FILE_NAME%.data",
// 	codeUrl: "%UNITY_WEBGL_DATA_FOLDER%/%UNITY_WEBGL_FILE_NAME%.js",
// 	memUrl: "%UNITY_WEBGL_DATA_FOLDER%/%UNITY_WEBGL_FILE_NAME%.mem",
// };

//New Glue
// var Module = {
// 	TOTAL_MEMORY: %UNITY_WEBGL_TOTAL_MEMORY%,
// 	errorhandler: null,			// arguments: err, url, line. This function must return 'true' if the error is handled, otherwise 'false'
// 	compatibilitycheck: null,
// 	backgroundColor: "%UNITY_WEBGL_BACKGROUND_COLOR%",
// 	splashStyle: "%UNITY_WEBGL_SPLASH_STYLE%",
// 	dataUrl: "%UNITY_WEBGL_DATA_FOLDER%/%UNITY_WEBGL_FILE_NAME%.data",
// 	codeUrl: "%UNITY_WEBGL_DATA_FOLDER%/%UNITY_WEBGL_MAIN_MODULE_FILE_NAME%.js",
// 	asmUrl: "%UNITY_WEBGL_DATA_FOLDER%/%UNITY_WEBGL_MAIN_MODULE_FILE_NAME%.asm.js",
// 	memUrl: "%UNITY_WEBGL_DATA_FOLDER%/%UNITY_WEBGL_MAIN_MODULE_FILE_NAME%.mem",%UNITY_WEBGL_WASM_BINARY_FILE%%UNITY_WEBGL_DEBUG_SYMBOLS_URL%%UNITY_WEBGL_DINAMIC_LIBRARIES%%UNITY_WEBGL_BACKGROUND_IMAGE%
// };

//What works
var Module = {
	TOTAL_MEMORY: %UNITY_WEBGL_TOTAL_MEMORY%,
	errorhandler: null,			// arguments: err, url, line. This function must return 'true' if the error is handled, otherwise 'false'
	compatibilitycheck: null,
	dataUrl: "%UNITY_WEBGL_DATA_FOLDER%/%UNITY_WEBGL_FILE_NAME%.data",
	codeUrl: "%UNITY_WEBGL_DATA_FOLDER%/%UNITY_WEBGL_FILE_NAME%.js",
	asmUrl: "%UNITY_WEBGL_DATA_FOLDER%/%UNITY_WEBGL_FILE_NAME%.asm.js",
	memUrl: "%UNITY_WEBGL_DATA_FOLDER%/%UNITY_WEBGL_FILE_NAME%.mem"
};
