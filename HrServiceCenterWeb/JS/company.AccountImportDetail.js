var opt = window.NameSpace || {};

function init() {
    var height = $('.container-layout').height() - 160;
    $('#dgContainer').height(height);
    $('#dg').datagrid('resize');
    opt.query();
    $('#upload').filebox({

    })
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

//时间窗口
opt.showDialogByTime = function () {
    $('#inputwindow').dialog('open');
};

