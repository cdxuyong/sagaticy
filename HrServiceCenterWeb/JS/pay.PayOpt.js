/*
 * pay
 * 
 * 
 */ 
var opt = window.NameSpace || {};
function init() {
    var height = $('.container-layout').height() - 160;
    $('#dgContainer').height(height);
    $('#dg').datagrid('resize');
    opt.query();
};

//查询发放列表
opt.query = function () {
    var url = '../Pay/QueryPayList';
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
};
opt.formatStatus = function (value, row, index) {
    if (value === 2)
        return '已扣款';
    else
        return '未扣款';
}
opt.formatRow = function (index, row) {
    if(row.Status === 2)
        return 'color:#0066cc';
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
                url: "../Pay/DeletePay",
                data: {
                    id: row.PayId
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
};
opt.add = function () {
    // TODO 
    var url = '../Pay/PayEditor?id=0';
    self.location = url;
};
//编辑对象
opt.edit = function (id) {
    // TODO 
    var row = $('#dg').datagrid('getSelected');
    if (row === null) {
        $.messager.alert('提示', '未选中任何数据!');
        return;
    }
    id = row.PayId;
    var url = '../Pay/PayEditor?id=' + id;
    self.location = url;
};
opt.dbClick = function (index, field, value) {
    opt.edit();
};
opt.openBatchDialog = function () {
    $('#dlg').dialog('open');
};
opt.createPayments = function () {
    var payDate = $('#datebox').datebox('getValue');


    $.messager.progress({
        interval : 1000,
        msg: '处理中，请稍候...'
    }); 

    var url = '../Payment/BatchCreatePayment';
    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        dataType: "json",
        data: JSON.stringify({ payDate: payDate }),
        success: function (result) {
            $.messager.progress('close');
            if (result.success) {
                $.messager.alert('提示消息', result.data, 'warning', function () {
                    self.location.reload(true);
                });
            }
            else {
                $.messager.alert('提示', '保存失败！' + result.data);
            }

        },
        error: function () {
            $.messager.progress('close');
            $.messager.alert('提示', '创建发放表出错！');
        }
    });
};