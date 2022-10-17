var opt = window.NameSpace || {};

function init() {
    debugger
    var height = $('.container-layout').height() - 160;
    $('#dgContainer').height(height);
    $('#dg').datagrid('resize');
    $('#dg').datagrid('enableFilter');
    opt.query();
}


//查询列表
opt.query = function () {
    var url = '../Work/QueryDayCheckList';
    debugger
    var pname = $('#txtQuery').val();
    $('#dg').datagrid('loading');
    $.ajax({
        url: url,
        type: "POST",
        dataType: "json",
        data: {
            pName: $('#txtpName').val(),
            cmpName: $('#txtcmpName').val(),
        },
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