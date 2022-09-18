var opt = window.NameSpace || {};

function init() {
    var height = $('.container-layout').height() - 160;
    $('#dgContainer').height(height);
    $('#dg').datagrid('resize');
    $('#dg').datagrid('enableFilter');
    var importId = $("#hideview").val();
    opt.query(importId);
}


//查询列表
opt.query = function (importId) {
    var url = '../Pay/QueryInsuranceDetail';

    $('#dg').datagrid('loading');
    $.ajax({
        url: url,
        type: "POST",
        dataType: "json",
        data: {
            importId: importId
        },
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