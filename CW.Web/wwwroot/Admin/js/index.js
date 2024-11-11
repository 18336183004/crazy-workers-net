$(function() {
    $("#lock").mouseover(function() {
        layer.tips("点击Alt + L快速锁屏！", "#lock", {
            tips: [1, "#FF5722"],
            time: 2000
        });
    });
    
    // 锁屏状态检测
    function checkLockStatus(locked) {
        // 锁屏
        if (locked == 1) {
            $(".lock-screen").show();
            $("#locker").show();
            $("#layui_layout").hide();
            $("#lock_password").val("");
        } else {
            $(".lock-screen").hide();
            $("#locker").hide();
            $("#layui_layout").show();
        }
    }
   
    checkLockStatus("0");

    function startTimer() {
        var today = new Date();
        var h = today.getHours();
        var m = today.getMinutes();
        var s = today.getSeconds();
        m = m < 10 ? "0" + m : m;
        s = s < 10 ? "0" + s : s;
        $("#time").html(h + ":" + m + ":" + s);
        setTimeout(function() { startTimer() }, 500);
    }

    // 锁定屏幕
    function lockSystem() {
        checkLockStatus(1);
        startTimer();
    }

    // 快捷键锁屏设置
    $(document).keyup(function (e) {
        if (e.altKey && e.which == 76) {
            lockSystem();
        }
    });
    
    //解锁屏幕
    function unlockSystem(url, pwd) {
        $.post(url, {
            pwd: pwd
        },
        function(data) {
            if (data.state == "true") {
                checkLockStatus("0");
            } else {
                layer.msg("解锁失败，请重试！", { icon: 2, time: 3000 });
            }
        }, "json");
    }

    // 点击锁屏
    $("#lock").click(function() {
        lockSystem();
    });
    // 解锁进入系统
    $("#unlock").click(function () {
       var url= $("#lock_url").val();
       var pwd = $("#lock_password").val();
       unlockSystem(url, pwd);
    });
    // 监控lock_password 键盘事件
    $("#lock_password").keypress(function (e) {
        var url = $("#lock_url").val();
        var pwd = $("#lock_password").val();
        var key = e.which;
        if (key == 13) {
            unlockSystem(url, pwd);
        }
    });

    //全屏
    function openFullScreen() {
        var docElm = document.documentElement;
        if (docElm.requestFullscreen) {
            docElm.requestFullscreen();
        }
        else if (docElm.msRequestFullscreen) {
            docElm.msRequestFullscreen();
        }
        else if (docElm.mozRequestFullScreen) {
            docElm.mozRequestFullScreen();
        }
        else if (docElm.webkitRequestFullScreen) {
            docElm.webkitRequestFullScreen();
        }
    }
    //退出全屏
    function closeFullScreen() {
        if (document.exitFullscreen) {
            document.exitFullscreen();
        } else if (document.msExitFullscreen) {
            document.msExitFullscreen();
        } else if (document.mozCancelFullScreen) {
            document.mozCancelFullScreen();
        } else if (document.webkitCancelFullScreen) {
            document.webkitCancelFullScreen();
        }
    }
    //判断是否是全屏
    function isFullScreen() {
        return document.fullscreenElement ||
               document.msFullscreenElement ||
               document.mozFullScreenElement ||
               document.webkitFullscreenElement || false;
    }

    // 点击锁屏
    $("#fullscreen").click(function () {
        var bl = isFullScreen();
        if (bl) {
            $("#fullscreen").html('<i class="fa fa-window-maximize"></i> 全屏');
            closeFullScreen();
        } else {
            $("#fullscreen").html('<i class="fa fa-window-restore"></i> 退出全屏');
            openFullScreen();
        }
    });
});