var UtilityPlugin =
{
    IsMobileUnityJS: function () {
        return Module.SystemInfo.mobile;
    },
    LogJS: function (msg) {
        console.log(UTF8ToString(msg));
    },
    ErrorJS: function (msg) {
        console.error(UTF8ToString(msg));
    },

    IsAccelerometerJS: function () {
        try {
            navigator.permissions.query({name: 'accelerometer'})
                .then((result) => {
                    if (result.state === "granted") {
						console.log("++ IsAccelerometerJS: true ++");
                        window.unityInstance.SendMessage('UtilityJS', 'OnEndIsAccelerometer', 1);
                    } else {
                        console.log("-- IsAccelerometerJS: false --");
						window.unityInstance.SendMessage('UtilityJS', 'OnEndIsAccelerometer', 0);
                    }
				})
				.catch((message) => {
					console.log("-- IsAccelerometerJS: " + message + " --");
					window.unityInstance.SendMessage('UtilityJS', 'OnEndIsAccelerometer', 0);
				});
        } catch (message) {
            console.log("-- IsAccelerometerJS: " + message + " --");
			window.unityInstance.SendMessage('UtilityJS', 'OnEndIsAccelerometer', 0);
        }
    },

    SetStorageJS: function (key, data) {
        try {
            var sKey = UTF8ToString(key);
            var dt = UTF8ToString(data);
            window.localStorage.setItem(sKey, btoa(dt));
            console.log("++ SetStorage  ++");
            return true;
        }
        catch (e) {
            console.log("-- SetStorage: " + e + " --");
            return false;
        }
    },
    GetStorageJS: function (key) {

        if (window.localStorage.length === 0) {
            console.log("-- GetStorage: Storage Empty --");
            return '';
        }

        var value;
        try {
            var sKey = UTF8ToString(key);
            value = window.localStorage.getItem(sKey);
        }
        catch (e) {
            console.log("-- GetStorages: " + e +" --");
            return '';
        }
        if (func.isEmpty(value)) {
            console.log("-- GetStorages: NULL --");
            return '';
        }
        else {
            console.log("++ GetStorages ++");
            return func.toUnityString(atob(value));
        }
    },
    IsStorageJS: function () {
        var test = 'teststorage';
        try {
            localStorage.setItem(test, test);
            localStorage.removeItem(test);
            console.log("++ IsStorage: true ++");
            return true;
        } catch (e) {
            console.log("-- IsStorage: " + e + " --");
            return false;
        }
    },


    SetCookiesJS: function (key, data) {
        var sKey = UTF8ToString(key);
        var dt = UTF8ToString(data);

        var expires = new Date();
        try {
            expires.setFullYear(expires.getFullYear() + 1);
            document.cookie = sKey + "=" + (dt ? btoa(dt) : "") + ";expires=" + expires.toGMTString();
            console.log("++ SetCookies  ++");
            return true;
        }
        catch (e) {
            console.log("-- SetCookies: " + e + " --");
            return false;
        }
    },
    GetCookiesJS: function (key) {
        var value;
        try {
            var sKey = UTF8ToString(key);
            var cookies = {};
            var items = document.cookie.split("; ");
            for (var ii = 0; ii < items.length; ii++) {
                var keyValuePair = items[ii].split("=");
                cookies[keyValuePair[0]] = keyValuePair[1];
            }
            value = cookies[sKey];
        }
        catch (e) {
            console.log("-- GetCookies: " + e + " --");
            return '';
        }
        if (func.isEmpty(value)) {
            console.log("-- GetCookies NULL --");
            return '';
        }
        else {
            
            console.log("++ GetCookies  ++");
            return func.toUnityString(atob(value));
        }
    },
    IsCookiesJS: function () {
        let isCookieEnabled = (window.navigator.cookieEnabled) ? true : false;
        if (typeof window.navigator.cookieEnabled == "undefined" && !isCookieEnabled) {
            document.cookie = "testcookie";
            isCookieEnabled = (document.cookie.indexOf("testcookie") != -1) ? true : false;
        }
        console.log("== GetCookies: " + isCookieEnabled + " ++");
        return isCookieEnabled;
    },

    QuitJS: function (url) {
        window.unityInstance
            .Quit()
            .then(() => {
                window.location.replace(UTF8ToString(url));
        });
    },
}

mergeInto(LibraryManager.library, UtilityPlugin);

