/** kit_admin-v1.0.7 MIT License By http://kit/zhengjinfan.cn e-mail:zheng_jinfan@126.com */;
layui.define(["jquery", "nprogress"], function (o) {
    var e = layui.jquery;
    o("loader", {
        version: "1.0.1",
        load: function (o) {
            NProgress.start();
            var n = o.url,
                r = o.name,
                t = o.id,
                i = e(void 0 !== o.elem ? o.elem : "#container");
            i.load(n, function (e, n, c) {
                "error" === n && "function" == typeof o.onError && o.onError(), "success" === n && (i.html(e),
                    "function" == typeof o.onSuccess && o.onSuccess({
                        name: r,
                        id: t
                    })), "function" == typeof o.onComplate && o.onComplate(), NProgress.done()
            })
        },
        getScript: function (o, n) {
            e.getScript(o, n)
        }
    })
});