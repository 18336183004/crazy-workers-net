/** kit_admin-v1.0.7 MIT License By http://kit/zhengjinfan.cn e-mail:zheng_jinfan@126.com */;
layui.define(["layer"], function (e) {
    var t = layui.jquery;
    layui.layer;
    e("utils", {
        v: "1.0.1",
        getBodyContent: function (e) {
            var t = /<body[^>]*>([\s\S]*)<\/body>/.exec(e);
            return t && 2 === t.length ? t[1] : e
        },
        loadHtml: function (e, n) {
            var i;
            return t.ajax({
                url: e,
                async: !1,
                dataType: "html",
                error: function (e, t, n) {
                    var a = ['<div style="padding: 20px;font-size: 20px;text-align:left;color:#009688;">',
                            "<p>{{msg}}</p>", "</div>"].join("");
                    i = 404 !== e.status ? a.replace("{{msg}}",
                        '<i class="layui-icon" style="font-size:70px;"></i>  未知错误.') : a.replace("{{msg}}",
                        '<i class="layui-icon" style="font-size:70px;"></i>  ' + n)
                },
                success: function (e) {
                    i = e
                },
                complete: function () {
                    "function" == typeof n && n()
                }
            }), i
        }
    })
});