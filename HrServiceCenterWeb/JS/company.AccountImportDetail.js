var opt = window.NameSpace || {};

function init() {
    var height = $('.container-layout').height() - 160;
    $('#dgContainer').height(height);
    $('#dg').datagrid('resize');
    opt.query();
}


//查询列表
opt.query = function () {
    debugger
    var url = '../Company/QueryAccountDetail';
    var params = { query: importName };
    $('#dg').datagrid('loading');
    $.ajax({
        url: url,
        type: "POST",
        dataType: "json",
        data: params,
        success: function (result) {
            var data = eval(result.data);
            $('#dg').datagrid('loadData', data);
            $('#dg').datagrid('loaded');
        },
        error: function () {
            $('#dg').datagrid('loaded');
            $.messager.alert('提示', '查询出错！');
        }
    });
}
//提交结算
opt.exec = function () {
    var url = '../Company/SubmitImportDetail';
    var params = { importName: importName };
    $.messager.progress({
        msg: '处理中，请稍候...'
    });
    $.ajax({
        url: url,
        type: "POST",
        dataType: "json",
        data: params,
        success: function (result) {
            debugger
            $.messager.progress('close');
            if (!result.data) {
                $.messager.alert('提示', '提交失败！');
            }
            else {
                $.messager.alert('提示', '提交成功，请刷新页面！');
            }
        },
        error: function () {
            $.messager.alert('提示', '提交失败！');
        }
    });
}
//删除操作
opt.delete = function () {
    $.messager.confirm('提示窗', '您确认删除吗?', function (event) {
        if (event) {
            $.ajax({
                type: 'GET',
                url: "../Company/DeleteAccountImportByName?importName=" + importName,
                dataType: "json",
                success: function (result) {
                    debugger
                    $.messager.alert('提示', result.data);
                    if (result.success) {
                        opt.query();
                    }
                }
            });
        }
        else {
            return;
        }
    });
}
//时间窗口
opt.showDialogByTime = function () {
    $('#inputwindow').dialog('open');
};
// 导出结算明细
// 20230319 add
opt.export = function () {
    var url = '../Company/ExportAccountPayDetail';
    var params = { Id: 0, ImportName: importName };
    HR.DownFile(url, params);
};

