//自定义插件
KindEditor.plugin('certificate', function (K) {
    var editor = this, name = 'certificate';
    // 点击图标时执行
    editor.clickToolbar(name, function () {
        var menu = editor.createMenu({
            name: name,
            width: 150
        });
        menu.addItem({
            title: 'insert [Category]',
            click: function () {
                editor.insertHtml('[category]');
            }
        });
        menu.addItem({
            title: 'insert [CeuHrs]',
            click: function () {
                editor.insertHtml('[ceu_hours]');
            }
        });
        menu.addItem({
            title: 'insert [Instructor]',
            click: function () {
                editor.insertHtml('[instructor_first_name][instructor_last_name]');
            }
        });
        menu.addItem({
            title: 'insert [Student]',
            click: function () {
                editor.insertHtml('[student_first_name][student_last_name]');
            }
        });
        menu.addItem({
            title: 'insert [Date]',
            click: function () {
                editor.insertHtml('[current_date]');
            }
        });
        menu.addItem({
            title: 'insert [CertNum]',
            click: function () {
                editor.insertHtml('[certificate_number]');
            }
        });
    });
});