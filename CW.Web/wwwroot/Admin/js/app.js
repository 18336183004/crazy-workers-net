/** kit_admin-v1.0.7 MIT License By http://kit/zhengjinfan.cn e-mail:zheng_jinfan@126.com */
 ;/**
 * Name:app.js
 * Author:Van
 * E-mail:zheng_jinfan@126.com
 * Website:http://kit.zhengjinfan.cn/
 * LICENSE:MIT
 */
var tab;
layui.define(['element', 'nprogress', 'form', 'table', 'loader', 'tab', 'navbar', 'onelevel', 'laytpl'], function(exports) {
    var $ = layui.jquery,
        element = layui.element,
        layer = layui.layer,
        _win = $(window),
        _doc = $(document),
        _body = $('.kit-body'),
        form = layui.form,
        table = layui.table,
        loader = layui.loader,
        navbar = layui.navbar;
    tab = layui.tab;
    var app = {
        hello: function(str) {
            layer.alert('Hello ' + (str || 'test'));
        },
        config: {
            type: 'iframe',
            tabMainUrl: 'main.html'
        },
        set: function(options) {
            var that = this;
            $.extend(true, that.config, options);
            return that;
        },
        init: function() {
            var that = this,
                _config = that.config;
            if (_config.type === 'page') {
                tab.set({
                    renderType: 'page',
                    mainUrl: _config.tabMainUrl,
                    elem: '#container',
                    onSwitch: function(data) { //选项卡切换时触发
                        //console.log(data.layId); //lay-id值
                        //console.log(data.index); //得到当前Tab的所在下标
                        //console.log(data.elem); //得到当前的Tab大容器
                    },
                    closeBefore: function(data) { //关闭选项卡之前触发
                        // console.log(data);
                        // console.log(data.icon); //显示的图标
                        // console.log(data.id); //lay-id
                        // console.log(data.title); //显示的标题
                        // console.log(data.url); //跳转的地址
                        return true; //返回true则关闭
                    }
                }).render();
                //navbar加载方式一，直接绑定已有的dom元素事件                
                navbar.bind(function(data) {
                    tab.tabAdd(data);
                });
            }
            if (_config.type === 'iframe') {
                tab.set({
                    //renderType: 'iframe',
                    mainUrl: _config.tabMainUrl,
                    //openWait: false,
                    elem: '#container',
                    onSwitch: function(data) { //选项卡切换时触发
                        //console.log(data.layId); //lay-id值
                        //console.log(data.index); //得到当前Tab的所在下标
                        //console.log(data.elem); //得到当前的Tab大容器
                    },
                    closeBefore: function(data) { //关闭选项卡之前触发
                        // console.log(data);
                        // console.log(data.icon); //显示的图标
                        // console.log(data.id); //lay-id
                        // console.log(data.title); //显示的标题
                        // console.log(data.url); //跳转的地址
                        return true; //返回true则关闭
                    }
                }).render();
                //navbar加载方式一，直接绑定已有的dom元素事件                
                navbar.bind(function(data) {
                    tab.tabAdd(data);
                });

                //navbar加载方式二，设置远程地址加载
                navbar.set({
                    remote: {
                        url: '/Settings/MenuDataList?parentId=' + id'
                    }
                }).render(function(data) {
                    tab.tabAdd(data);
                });

                ////处理顶部一级菜单
                //var onelevel = layui.onelevel;
                //if (onelevel.hasElem()) {
                //    onelevel.set({
                //        remote: {
                //            url: '/Admin/Settings/NavDataList' //远程地址
                //        },
                //        onClicked: function (id) {
                //            navbar.set({
                //                remote: {
                //                    url: '/Admin/Settings/MenuDataList?parentId=' + id
                //                }
                //            }).render(function (data) {
                //                tab.tabAdd(data);
                //            });
                //        },
                //        renderAfter: function (elem) {
                //            elem.find('li').eq(0).click(); //模拟点击第一个
                //        }
                //    }).render();
                //}
            }
            return that;
        }
    };

    //输出test接口
    exports('app', app);
});