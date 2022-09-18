var opt = window.NameSpace || {};

function init() {
    var height = $('.container-layout').height() - 160;
    $('#dgContainer').height(height);
    $('#dg').datagrid('resize');
    $('#dg').datagrid('enableFilter');
    opt.query();
}


//查询列表
opt.query = function () {
    var url = '../Pay/ImportorPayDetailTable?importorId=' + importorId;
    $('#dg').datagrid('loading');
    $.ajax({
        url: url,
        type: "GET",
        dataType: "json",
        data: {},
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

