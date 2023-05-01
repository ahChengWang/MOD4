
var InxSSO = {};
$(function () {

    var baseAddr = 'http://psamv4athetn.cminl.oa';     //台南正式
    //var baseAddr = '//tsamv4athe.cminl.oa';       //台南測試

    //var baseAddr = '//psamv4athefs.cminl.oa';     //佛山
    //var baseAddr = '//psamv4athengb.cminl.oa';      //寧波
    //var baseAddr = '//psamv4athenj.cminl.oa';     //南京

    //var baseAddr = '//tw059306s:5011'
    //var baseAddr = '//tw059306s:5010'
    //var baseAddr = '//psamv4athetn1.cminl.oa'          //SAMv4 Athe 備援區

    //var baseAddr = '//tcuxsamv4athe.cminl.oa';         //CarUX測試區
    //var baseAddr = '//pcuxsamv4athetn.cminl.oa';         //CarUX5正式區

    init();

    function init() {
        InxSSO.logout = logout;
        InxSSO.getUserInfo = getUserInfo;
        InxSSO.QueryStringCase = QueryStringCase;
        InxSSO.baseAddr = baseAddr;
    }

    function getUserInfo(setUserInfo, CheckIP, SysID) {
        CheckIP = CheckIP == false ? false : true;
        SysID = SysID == null ? null : SysID;
        console.log("getUserInfo SysID", SysID)
        let apurl = DeleteStringCase(DeleteStringCase(location.href, 'Token'), 'ssoretry');

        var ssoRetry = parseInt(QueryStringCase("ssoretry"));
        if (!ssoRetry) ssoRetry = 0;
        if (ssoRetry >= 4) {
            logout(apurl, true, SysID);
        }
        apurl = AddStringCase(apurl, 'ssoretry', ssoRetry + 1);

        console.log('apurl', apurl);



        var token = QueryStringCase("Token");
        let cookieToken = getCookie("SSOToken");
        token = token ? token : cookieToken;
        //ssLog('01.getUserInfo() SysID=' + SysID + " ,QueryStrToken=" + Token+",Cookie Token="+cookieToken);
        ssLog('01.inxssov4.js :Token=' + token + '  location.href=' + location.href, SysID);
        var logonurl = baseAddr + '/form/Logon.html?SysID=' + SysID + '&url=' + encodeURIComponent(apurl);
        if (token) {
            verifyTicket(token, setUserInfo, CheckIP, SysID, logonurl);
        }
        else {
            //var url = baseAddr + '/form/Logon.html?SysID=' + SysID + '&url=' + encodeURIComponent(DeleteStringCase(location.href, "Token"));
            //console.log('url=', url);
            //ssLog('06.getUserInfo() NoToken,redirect url=' + url);
            window.location.replace(logonurl);
        }
    }


    function verifyTicket(token, setUserInfo, CheckIP, SysID, logonurl) {
        var ssoretry = parseInt(QueryStringCase("ssoretry"), 10);
        if (isNaN(ssoretry))
            ssoretry = 0;
        var data = {
            "Token": token,
            "IsCheckIP": CheckIP,
            "SysID": SysID
        };
        //ssLog('03.verifyTicket() Token=' + key+',SysID='+SysID);

        $.ajax({
            type: "Post",
            url: baseAddr + "/api/SSO/VerifyToken",
            data: JSON.stringify(data),
            contentType: "application/json",
            async: false,
            success: function (userInfo) {
                alert('success');
                //('04.VerifyToken API success userInfo.MemID=' + userInfo.MemID + ' ,MemName=' + userInfo.MemName);
                ssLog('02.inxssov4.js :API VerifyToken 成功， Token=' + token + ',  MemID=' + userInfo.MemID + ', MemName =' + userInfo.MemName, SysID);
                document.cookie = "SSOToken=" + userInfo["SSOToken"] + ";path=/";
                if (window.console)
                    console.log(userInfo);
                HideToken(userInfo);
                setUserInfo(userInfo);
            },
            error: function (e) {
                alert('error');
                ssLog('02.inxssov4.js :API VerifyToken 失敗, Token=' + token + '  msg=' + e.responseJSON.Message, SysID);
                //ssLog('19.VerifyToken API fail e.responseJSON.Message='+e.responseJSON.Message);
                /*
                if (ssoretry < 8) {
                    var url = baseAddr + '/form/Logon.html?url=' + encodeURIComponent(AddStringCase(DeleteStringCase(location.href, "Token"), "ssoretry", ++ssoretry));
                    console.log('url=', url);
                    window.location.href = url;
                }
                else if (e.responseJSON) {
                    alert(e.responseJSON.Message);
                }
                else {
                    alert("verifyTicket Unknown Error!");
                }
                */

                if (e.responseJSON) {
                    if (e.responseJSON.ErrorCode == "9002" || e.responseJSON.ErrorCode == "9003") {
                        //if (ssoretry < 8) {
                        //    var url = baseAddr + '/form/Logon.html?SysID=' + SysID + '&url=' + encodeURIComponent(AddStringCase(DeleteStringCase(location.href, "Token"), "ssoretry", ssoretry+1));
                        //    if (window.console)
                        //        console.log('url=', url);
                        //    window.location.replace(url);
                        //}
                        //else {
                        //    alert(e.responseJSON.Message);
                        //}

                        //var url = baseAddr + '/form/Logon.html?SysID=' + SysID + '&url=' + encodeURIComponent(DeleteStringCase(location.href));
                        console.log('url=', logonurl);
                        window.location.replace(logonurl);



                    }
                    else {
                        alert(e.responseJSON.Message);
                    }
                }
                else {
                    alert("verifyTicket Unknown Error!");
                }
            }
        });

    }




    function newVerifyTicket(token, setUserInfo, CheckIP, SysID, logonurl, sSOTicket4) {
        var ssoretry = parseInt(QueryStringCase("ssoretry"), 10);
        if (isNaN(ssoretry))
            ssoretry = 0;
        var _model = {
            Token: token,
            IsCheckIP: CheckIP,
            SSOTicket4: sSOTicket4,
            SysID: SysID
        };
        //ssLog('03.verifyTicket() Token=' + key+',SysID='+SysID);

        $.ajax({
            type: "POST",
            url: "./Account/VerifyToken",
            data: _model,
            async: false,
            success: function (userInfo) {
                //('04.VerifyToken API success userInfo.MemID=' + userInfo.MemID + ' ,MemName=' + userInfo.MemName);
                ssLog('02.inxssov4.js :API VerifyToken 成功， Token=' + token + ',  MemID=' + userInfo.MemID + ', MemName =' + userInfo.MemName, SysID);
                document.cookie = "SSOToken=" + userInfo["SSOToken"] + ";path=/";
                if (window.console)
                    console.log(userInfo);
                HideToken(userInfo);
                setUserInfo(userInfo);
            },
            error: function (e) {
                ssLog('02.inxssov4.js :API VerifyToken 失敗, Token=' + token + '  msg=' + e.responseJSON.Message, SysID);
                //ssLog('19.VerifyToken API fail e.responseJSON.Message='+e.responseJSON.Message);
                /*
                if (ssoretry < 8) {
                    var url = baseAddr + '/form/Logon.html?url=' + encodeURIComponent(AddStringCase(DeleteStringCase(location.href, "Token"), "ssoretry", ++ssoretry));
                    console.log('url=', url);
                    window.location.href = url;
                }
                else if (e.responseJSON) {
                    alert(e.responseJSON.Message);
                }
                else {
                    alert("verifyTicket Unknown Error!");
                }
                */

                if (e.responseJSON) {
                    if (e.responseJSON.ErrorCode == "9002" || e.responseJSON.ErrorCode == "9003") {
                        //if (ssoretry < 8) {
                        //    var url = baseAddr + '/form/Logon.html?SysID=' + SysID + '&url=' + encodeURIComponent(AddStringCase(DeleteStringCase(location.href, "Token"), "ssoretry", ssoretry+1));
                        //    if (window.console)
                        //        console.log('url=', url);
                        //    window.location.replace(url);
                        //}
                        //else {
                        //    alert(e.responseJSON.Message);
                        //}

                        //var url = baseAddr + '/form/Logon.html?SysID=' + SysID + '&url=' + encodeURIComponent(DeleteStringCase(location.href));
                        console.log('url=', logonurl);
                        window.location.replace(logonurl);



                    }
                    else {
                        alert(e.responseJSON.Message);
                    }
                }
                else {
                    alert("verifyTicket Unknown Error!");
                }
            }
        });

    }

    function HideToken() {
        var url = DeleteStringCase(DeleteStringCase(location.href, "ssoretry"), "Token");
        //ssLog('07.HideToken url=' +url);
        window.history.replaceState(null, null, url);
    }

    function logout(url, IsLogin, SysID) {

        delete_cookie("SSOToken");
        SysID = SysID == null ? null : SysID;

        IsLogin = IsLogin == true ? true : false;
        var param = "";
        param = IsLogin ? "" : "&IsLogin=Y";
        if (window.console)
            console.log("logout-url:" + url + ",isLogin:" + IsLogin);
        if (url)
            location.href = baseAddr + '/form/Logout.html?SysID=' + SysID + '&url=' + encodeURIComponent(url) + param;
        else
            location.href = baseAddr + '/form/Logout.html?SysID=' + SysID + '&url=' + encodeURIComponent(DeleteStringCase(DeleteStringCase(location.href, "Token"), "ssoretry")) + param;
    }

    //function QueryStringCase(name, defaultValue) {
    //    if (defaultValue === undefined) {
    //        defaultValue = null;
    //    }
    //    var reg = new RegExp("(^|&|\\?)" + name + "=([^\&\#]*)(&|$)?"), r;
    //    var s = location.href.split("?");
    //    if (s.length > 0)
    //        s = "?" + s[1];
    //    else
    //        s = "";
    //    r = s.match(reg);
    //    if (r) {
    //        r = unescape(r[2]);
    //        //alert('QueryString:' +r);
    //        return r;
    //    } else {
    //        return defaultValue;
    //    }
    //}

    function QueryStringCase(name, defaultValue) {
        let url = new URL(location.href);
        url.searchParams.has(name);
        let r = url.searchParams.get(name);
        if (r) {
            return r;
        } else {
            return defaultValue;
        }
    }

    function DeleteStringCase(loc, name) {
        var reg = new RegExp("(^|&|\\?)" + name + "=([^\&\#]*)(&|$)?"), r, result;
        var s = loc.split("?");
        if (s.length > 0)
            s = "?" + s[1];
        else
            s = "";
        r = s.match(reg);
        if (r) {
            //r = unescape(r[0]);
            if (r[3] == "" || r[3] == null)
                result = loc.replace(r[0], "");
            else
                result = loc.replace(r[0], r[1]);
            return result;
        } else {
            return loc;
        }
    }



    function MoveHashTag(loc) {
        var reg = new RegExp("#+([^\&\?]*)"), r, result;
        var s = loc.split("?");
        if (s.length > 0)
            s = "?" + s[1];
        else
            s = "";
        r = s.match(reg);
        if (r) {
            result = loc.replace(r[0], "");
            result = result + r[0];
            return result;
        } else {
            return loc;
        }
    }

    function AddStringCase(loc, key, value) {
        var reg = new RegExp("(^|&|\\?)" + key + "=([^\&\#]*)(&|$)?"), r, result;
        var s = loc.split("?");
        if (s.length > 0)
            s = "?" + s[1];
        else
            s = "";
        r = s.match(reg);
        if (r) {
            result = loc.replace(r[0], (r[1] + key + "=" + value));
        } else {
            result = loc + ((loc.indexOf("?") != -1) ? '&' : '?') + key + "=" + value;
        }
        return MoveHashTag(result);
    }

    function getCookie(name) {
        const value = `; ${document.cookie}`;
        const parts = value.split(`; ${name}=`);
        if (parts.length >= 2)
            return parts.pop().split(';').shift();
    }
    function delete_cookie(name) {
        if (getCookie(name)) {
            document.cookie = name + '=; Max-Age=0; path=/; domain=' + location.host;
        }
    }

    function isValidHttpUrl(string) {
        let url;
        try {
            url = new URL(string);
        } catch (_) {
            return false;
        }
        return url.protocol === "http:" || url.protocol === "https:";
    }

});

function sessionLog(key, msg, isReset) {
    let oldMsg = "";
    if (!isReset) {
        oldMsg = sessionStorage.getItem(key)
    }
    oldMsg += msg + "\n";
    sessionStorage.setItem(key, oldMsg);
    //localStorage.setItem(key, oldMsg);
}

function ssLog(msg, sysID) {
    //sessionLog('SSOv4', msg, isReset)
    let data = {
        SysID: sysID,
        Message: msg
    };
    $.ajax({
        type: "Post",
        url: InxSSO.baseAddr + "/api/SSO/LogAPI",
        data: JSON.stringify(data),
        contentType: "application/json",
        success: function (r) {
            console.log('LogAPI success result=' + r);
        }
    });

}