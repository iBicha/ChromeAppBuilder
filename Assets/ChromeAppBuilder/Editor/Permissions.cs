/* https://developer.chrome.com/apps/permissions
 * https://developer.chrome.com/apps/declare_permissions
 * https://developer.chrome.com/apps/permission_warnings
 */
using UnityEditor;
using UnityEngine;

namespace ChromeAppBuilder
{
    public enum Permissions : int
    {
        videoCapture,
        audioCapture,
        fullscreen,
        storage,
        alwaysOnTop,
        pointerLock,
        browser,
        power,
        clipboard,
        identity,
        notifications,
        facebook,
        instagram,
        Count //The number of permissions in the enum
    }

    public static class PermissionsExtension
    {
        public static string ToPermission(this Permissions me)
        {
            switch (me)
            {
                case Permissions.audioCapture:
                    return "audioCapture";
                case Permissions.videoCapture:
                    return "videoCapture";
                case Permissions.fullscreen:
                    return "app.window.fullscreen";
                case Permissions.storage:
                    return "storage";
                case Permissions.alwaysOnTop:
                    return "app.window.alwaysOnTop";
                case Permissions.pointerLock:
                    return "pointerLock";
                case Permissions.browser:
                    return "browser";
                case Permissions.power:
                    return "power";
                case Permissions.clipboard:
                    return "clipboardRead";
                case Permissions.identity:
                case Permissions.facebook:
                case Permissions.instagram:
                    return "identity";
                case Permissions.notifications:
                    return "notifications";
                default:
                    return string.Empty;
            }
        }
        public static string[] ToPermissionsExtra(this Permissions me)
        {
            switch (me)
            {
                case Permissions.clipboard:
                    return new string[] { "clipboardWrite" };
                case Permissions.fullscreen:
                    return new string[] { "app.window.fullscreen.overrideEsc" };
                case Permissions.identity:
                    return new string[] { "identity.email" };
                case Permissions.facebook:
                    return new string[] { "https://www.facebook.com/", "https://graph.facebook.com/" };
                case Permissions.instagram:
                    return new string[] { "https://api.instagram.com/" };
                default:
                    return null;
            }
        }
        public static string Name(this Permissions me)
        {
            switch (me)
            {
                case Permissions.alwaysOnTop:
                    return "Always On Top";
                case Permissions.videoCapture:
                    return "Camera (WebCamTexture)";
                case Permissions.audioCapture:
                    return "Record audio (Microphone)";
                case Permissions.fullscreen:
                    return "Fullscreen";
                case Permissions.storage:
                    return "Storage";
                case Permissions.pointerLock:
                    return "Cursor Lock";
                case Permissions.browser:
                    return "Browser (OpenTab)";
                case Permissions.power:
                    return "Power (Keep Awake)";
                case Permissions.clipboard:
                    return "ClipBoard";
                case Permissions.identity:
                    return "Identity (User ID and Email)";
                case Permissions.facebook:
                    return "Facebook Connect";
                case Permissions.instagram:
                    return "Instagram";
                case Permissions.notifications:
                    return "Notifications";
                default:
                    return string.Empty;
            }

        }
    }

    public static class StringExtension
    {
        public static string[] ToArray(this string str, params string[] others)
        {
            return new string[] { str };
        }
    }
}