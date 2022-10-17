var opt = window.NameSpace || {};

function init() {
    var height = $('.container-layout').height() - 160;
    $('#dgContainer').height(height);
    $('#dg').datagrid('resize');
    //$('#dg').datagrid('enableFilter');
    //var importId = $("#hideview").val();
    opt.query();
}


//查询列表
opt.query = function () {
    var url = '../Work/QueryMonthDetailList';

    $('#dg').datagrid('loading');
    $.ajax({
        url: url,
        type: "POST",
        dataType: "json",
        data: {
            cmpName: companyName,
            month: queryDate
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