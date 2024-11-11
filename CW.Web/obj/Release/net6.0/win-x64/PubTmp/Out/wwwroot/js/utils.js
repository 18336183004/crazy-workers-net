//浏览器相关信息
var Browser = {
    isIE: function () {
        return window.ActiveXObject ? true : false;
    },
    isChrome: function () {
        return navigator.userAgent.toLowerCase().indexOf("chrome") != -1
    },
    isSafari: function () {
        return navigator.userAgent.toLowerCase().indexOf("safari") != -1
    },
    isFirefox: function () {
        return navigator.userAgent.toLowerCase().indexOf("firefox") != -1
    },
    isOpera: function () {
        return navigator.userAgent.toLowerCase().indexOf("opera") != -1
    }
}

//事件操作
var Event = {
    addHandler: function (element, type, handler) {
        if (element.addEventListener) {
            element.addEventListener(type, handler, false);
        } else if (element.attachEvent) {
            element.attachEvent("on" + type, handler);
        } else {
            element["on" + type] = handler;
        }
    },
    getButton: function (event) {
        if (document.implementation.hasFeature("MouseEvents", "2.0")) {
            return event.button;
        } else {
            switch (event.button) {
                case 0:
                case 1:
                case 3:
                case 5:
                case 7:
                    return 0;
                case 2:
                case 6:
                    return 2;
                case 4: return 1;
            }
        }
    },
    getCharCode: function (event) {
        if (typeof event.charCode == "number") {
            return event.charCode;
        } else {
            return event.keyCode;
        }
    },
    getEvent: function (event) {
        return event ? event : window.event;
    },
    getRelatedTarget: function (event) {
        if (event.relatedTarget) {
            return event.relatedTarget;
        } else if (event.toElement) {
            return event.toElement;
        } else if (event.fromElement) {
            return event.fromElement;
        } else {
            return null;
        }

    },
    getTarget: function (event) {
        return event.target || event.srcElement;
    },
    preventDefault: function (event) {
        if (event.preventDefault) {
            event.preventDefault();
        } else {
            event.returnValue = false;
        }
    },
    removeHandler: function (element, type, handler) {
        if (element.removeEventListener) {
            element.removeEventListener(type, handler, false);
        } else if (element.detachEvent) {
            element.detachEvent("on" + type, handler);
        } else {
            element["on" + type] = null;
        }
    },
    stopPropagation: function (event) {
        if (event.stopPropagation) {
            event.stopPropagation();
        } else {
            event.cancelBubble = true;
        }
    }
};

//Ajax操作
var Ajax = {
    createXHR: function () {
        if (typeof XMLHttpRequest != "undefined") {
            return new XMLHttpRequest();
        } else if (typeof ActiveXObject != "undefined") {
            if (typeof arguments.callee.activeXString != "string") {
                var versions = ["MSXML2.XMLHttp.6.0", "MSXML2.XMLHttp.3.0", "MSXML2.XMLHttp"];
                for (var i = 0, len = versions.length; i < len; i++) {
                    try {
                        var xhr = new ActiveXObject(versions[i]);
                        arguments.callee.activeXString = versions[i];
                        return xhr;
                    } catch (ex) {
                        alert("对不起出错了，请联系我们！");
                    }
                }
            }
            return new ActiveXObject(arguments.callee.activeXString);
        } else {
            throw new Error("没有有效的XHR对象！");
        }
    },
    parseParms: function (parms) {
        if (parms != null) {
            var array = new Array();
            for (var p in parms) {
                array.push(encodeURIComponent(p) + "=" + encodeURIComponent(parms[p]));
            }
            if (array.length > 0)
                return array.join("&");
        }
        return null;
    },
    get: function (url, cache, callback) {

        if (!cache) {
            url = url + (url.indexOf("?") == -1 ? "?" : "&")
            url = url + "ajaxTime=" + new Date();
        }

        var xhr = this.createXHR();
        if (callback) {
            xhr.onreadystatechange = function (event) {
                if (xhr.readyState == 4) {
                    if ((xhr.status >= 200 && xhr.status < 300) || xhr.status == 304) {
                        callback(xhr.responseText);
                    } else {
                        //alert("请求出错,错误号为 [" + xhr.status + "]");
                    }
                }
            }
        }
        xhr.open("get", url, true);
        xhr.send(null);
    },
    post: function (url, parms, cache, callback) {

        if (!cache) {
            url = url + (url.indexOf("?") == -1 ? "?" : "&")
            url = url + "ajaxTime=" + new Date();
        }

        var xhr = this.createXHR();
        if (callback) {
            xhr.onreadystatechange = function (event) {
                if (xhr.readyState == 4) {
                    if ((xhr.status >= 200 && xhr.status < 300) || xhr.status == 304) {
                        callback(xhr.responseText);
                    } else {
                        //alert("请求出错,错误号为 [" + xhr.status + "]");
                    }
                }
            }
        }
        xhr.open("post", url, true);
        xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        xhr.send(this.parseParms(parms));
    },
    postasync: function (url, parms, cache, callback) {

        if (!cache) {
            url = url + (url.indexOf("?") == -1 ? "?" : "&")
            url = url + "ajaxTime=" + new Date();
        }

        var xhr = this.createXHR();
        if (callback) {
            xhr.onreadystatechange = function (event) {
                if (xhr.readyState == 4) {
                    if ((xhr.status >= 200 && xhr.status < 300) || xhr.status == 304) {
                        callback(xhr.responseText);
                    } else {
                        //alert("请求出错,错误号为 [" + xhr.status + "]");
                    }
                }
            }
        }
        xhr.open("post", url, false);
        xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        xhr.send(this.parseParms(parms));
    },
    getasync: function (url, cache, callback) {

        if (!cache) {
            url = url + (url.indexOf("?") == -1 ? "?" : "&")
            url = url + "ajaxTime=" + new Date();
        }

        var xhr = this.createXHR();
        if (callback) {
            xhr.onreadystatechange = function (event) {
                if (xhr.readyState == 4) {
                    if ((xhr.status >= 200 && xhr.status < 300) || xhr.status == 304) {
                        callback(xhr.responseText);
                    } else {
                        //alert("请求出错,错误号为 [" + xhr.status + "]");
                    }
                }
            }
        }
        xhr.open("get", url, true);
        xhr.send(null);
    }
}

//给firefox定义contains()方法
if (typeof (HTMLElement) != "undefined") {
    HTMLElement.prototype.contains = function (obj) {
        //通过循环对比来判断是不是obj的父元素
        while (obj != null && typeof (obj.tagName) != "undefind") {
            if (obj == this) return true;
            obj = obj.parentNode;
        }
        return false;
    };
}

//trim操作
function trim(val) {
    return this.replace(/^\s\s*/, '').replace(/\s\s*$/, '');
}

//html编码
function htmlEncode(val) {
    return val.replace(/&/g, '&amp;').replace(/"/g, '&quot;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
}

//设置HTML元素属性
function setAttribute(o, a, v) {
    if (typeof o != 'object' || typeof a != 'string') return;
    a == 'class' ? o.className = v : o.setAttribute(a, v);
}

//获取HTML元素属性值
function getAttribute(o, a) {
    if (typeof o != 'object' || typeof a != 'string') return;
    return a == 'class' ? o.className : o.getAttribute(a);
}

//移除HTML元素属性
function removeAttribute(o, a) {
    if (typeof o != 'object' || typeof a != 'string') return;
    o.removeAttribute(a);
    if (a == 'class') o.removeAttribute('className');
}

//获取选中的单选框
function getSelectedRadio(radioList) {
    for (var i = 0, len = radioList.length; i < len; i++) {
        var radio = radioList[i];
        if (radio.checked) {
            return radio;
        }
    }
    return null;
}

//设置选中的单选框
function setSelectedRadio(radioList, value) {
    for (var i = 0, len = radioList.length; i < len; i++) {
        var radio = radioList[i];
        if (radio.value == value) {
            radio.checked = true;
            break;
        }
    }
}

//获取选中的选择项(单选)
function getSelectedOption(selectbox) {
    return selectbox.options[selectbox.selectedIndex];
}

//获取选中的选择项(多选)
function getSelectedOptions(selectbox) {
    var result = new Array();
    var option = null;
    for (var i = 0, len = selectbox.options.length; i < len; i++) {
        option = selectbox.options[i];
        if (option.selected) {
            result.push(option);
        }
    }
    return result;
}

//设置选中的选择项
function setSelectedOptions(selectbox, value) {
    for (var i = 0, len = selectbox.options.length; i < len; i++) {
        var option = selectbox.options[i];
        if (option.value == value) {
            option.selected = true;
        }
        else {
            option.selected = false;
        }
    }
}

//移除选择框中全部的选择项
function removeAllOptions(selectbox) {
    for (i = selectbox.options.length - 1; i >= 0; i--) {
        selectbox.options.remove(i);
    }
}

//读取cookie
function getCookie(name) {
    if (document.cookie.length > 0) {
        var cookieItemList = document.cookie.split(";");
        for (var i = 0; i < cookieItemList.length; i++) {
            var keyValuePair = cookieItemList[i].split("=");
            if (keyValuePair[0] = name) {
                return decodeURIComponent(keyValuePair[1]);
            }
        }
    }
    return ""
}

//设置cookie
function setCookie(name, value, expires) {
    var cookie = name + "=" + encodeURIComponent(value);
    if (expires != undefined && expires != null) {
        cookie += ";expires=" + expires;
    }
    document.cookie = cookie;
}

//删除cookie
function delCookie(name) {
    var expires = new Date();
    expires.setTime(expires.getTime() - 1);
    document.cookie = name + "=;expires=" + expires.toGMTString();
}

//判断是否是CSS 16进制颜色值
function isCssColor(val) {
    var regex = /^#[0-9a-fA-F]{6}$/;
    return regex.test(val);
}

//判断是否是RGB颜色值
function isRGBColor(val) {
    var regex = /^[rR][gG][Bb][\(]([\\s]*(2[0-4][0-9]|25[0-5]|[01]?[0-9][0-9]?)[\\s]*,){2}[\\s]*(2[0-4]\\d|25[0-5]|[01]?\\d\\d?)[\\s]*[\)]{1}$/;
    return regex.test(val);
}

//判断是否是数字
function isNumber(val) {
    var regex = /^[\d|\.]+$/;
    return regex.test(val);
}

//判断是否是 正负数
function isNumberTwo(val) {
    var regex = /^(\-|\+)?[\d|\.]+$/;
    return regex.test(val);
}

//判断是否是百分比(0.00 ~ 0.99)
function isRatio(val) {
    if (val == 0 || val == 1 || val == "1.00")
        return true;
    var regex = /^0\.([0-9]){1,2}$/;
    return regex.test(val);
}

//判断是否为整数
function isInt(val) {
    var regex = /^\d+$/;
    return regex.test(val);
}

//判断是否为邮箱
function isEmail(val) {
    var regex = /^[\w!#$%&'*+/=?^_`{|}~-]+(?:\.[\w!#$%&'*+/=?^_`{|}~-]+)*@(?:[\w](?:[\w-]*[\w])?\.)+[\w](?:[\w-]*[\w])?$/;
    return regex.test(val);
}

//判断是否为手机号
function isMobile(tel) {
    var regex = /^1([3-9][0-9])[0-9]{8}$/;
    return regex.test(tel);
}

//判断是否是汉字和字母
function isCharacter(val) {
    var regex = /^[a-zA-Z\u4e00-\u9fa5]+$/;
    return regex.test(val);
}

//判断是否是汉字
function isChinese(val) {
    //var regex = /^[1][0-9][0-9]{9}$/;
    var regex = new RegExp("[\\u4E00-\\u9FFF]+", "g");
    return regex.test(val);
}

function setdecimal(val) {
   return  parseFloat(val).toFixed(2);
}

function formatMoney(s, n) {
    n = n > 0 && n <= 20 ? n : 2;
    s = parseFloat((s + "").replace(/[^\d\.-]/g, "")).toFixed(n) + "";
    var l = s.split(".")[0].split("").reverse(),
    r = s.split(".")[1];
    t = "";
    for (i = 0; i < l.length; i++) {
        t += l[i] + ((i + 1) % 3 == 0 && (i + 1) != l.length ? "," : "");
    }
    return t.split("").reverse().join("") + "." + r;
}


function formatDate(val) {
    if (val) {
        var date = val.replace("/Date(", "").replace(")/", "");
        if (date < 0) {
            return "";
        }
        return new Date(parseInt(date)).format("yyyy-MM-dd hh:mm:ss");
    }
    return '';
}

function formatDate2(val) {
    if (val) {
        var date = val.replace("/Date(", "").replace(")/", "");
        if (date < 0) {
            return "";
        }
        return new Date(parseInt(date)).format("yyyy-MM-dd");
    }
    return '';
}

function formatDate3(val) {
    if (val) {
        var date = val.replace("/Date(", "").replace(")/", "");
        if (date < 0) {
            return "";
        }
        return new Date(parseInt(date)).format("yyyy-MM-dd hh:mm");
    }
    return '';
}

Date.prototype.format = function (format) {
    /* 
     * eg:format="yyyy-MM-dd hh:mm:ss"; 
     */
    var o = {
        "M+": this.getMonth() + 1, // month  
        "d+": this.getDate(), // day  
        "h+": this.getHours(), // hour  
        "m+": this.getMinutes(), // minute  
        "s+": this.getSeconds(), // second  
        "q+": Math.floor((this.getMonth() + 3) / 3), // quarter  
        "S": this.getMilliseconds()
        // millisecond  
    }

    if (/(y+)/.test(format)) {
        format = format.replace(RegExp.$1, (this.getFullYear() + "").substr(4
                        - RegExp.$1.length));
    }

    for (var k in o) {
        if (new RegExp("(" + k + ")").test(format)) {
            format = format.replace(RegExp.$1, RegExp.$1.length == 1
                            ? o[k]
                            : ("00" + o[k]).substr(("" + o[k]).length));
        }
    }
    return format;
}

//js 仿guid格式
function GetGuid() {
    var s = [];
    var hexDigits = "0123456789abcdef";
    for (var i = 0; i < 36; i++) {
        s[i] = hexDigits.substr(Math.floor(Math.random() * 0x10), 1);
    }
    s[14] = "4";  // bits 12-15 of the time_hi_and_version field to 0010
    s[19] = hexDigits.substr((s[19] & 0x3) | 0x8, 1);  // bits 6-7 of the clock_seq_hi_and_reserved to 01
    //添加“-”间隔
    //s[8] = s[13] = s[18] = s[23] = "-";
    s[8] = s[13] = s[18] = s[23] = "";
 
    var uuid = s.join("");
    return uuid;
}