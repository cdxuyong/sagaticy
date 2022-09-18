var opt = window.NameSpace || {};

function init() {
    var height = $('.container-layout').height() - 160;
    $('#dgContainer').height(height);
    $('#dg').datagrid('resize');
    opt.query();
}


//查询模板列表
opt.query = function () {
    var url =  '../Pay/GetTemplateList';
    var params = { query: $('#txtQuery').val() };
    $('#dg').datagrid('loading');
    $.ajax({
        url: url,
        type: "POST",
        dataType: "json",
        data: params,
        success: function (data) {
            $('#dg').datagrid('loadData', data);
            $('#dg').datagrid('loaded');
        },
        error: function () {
            $('#dg').datagrid('loaded');
            $.messager.alert('提示', '查询出错！');
        }
    });
}


//删除操作
opt.delete = function (id) {
    // TODO 
    var row = $('#dg').datagrid('getSelected');

    if (row == null) {
        $.messager.alert('提示', '未选中任何数据!');
        return;
    }
    $.messager.confirm('提示窗', '您确认删除吗?', function (event) {
        if (event) {
            $.ajax({
                type: 'POST',
                url: "../Pay/DeleteTemplate",
                data: {
                    id: row.TemplateId
                },
                dataType: "json",
                success: function (result) {
                    $.messager.alert('提示', result);
                    opt.query();
                }
            });
        }
        else {
            return;
        }
    });
}


opt.add = function () {
    // TODO 
    var url = '../Pay/TemplateEditor?id=0';
    self.location = url;
}

//编辑对象
opt.edit = function (id) {
    // TODO 
    var row = $('#dg').datagrid('getSelected');
    if (row == null) {
        $.messager.alert('提示', '未选中任何数据!');
        return;
    }
    var id = row.TemplateId;
    var url = '../Pay/TemplateEditor?id=' + id;
    self.location = url;
}
