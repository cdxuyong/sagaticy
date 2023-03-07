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
    var url = '../Pay/QueryImportorList';
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
    var row = $('#dg').datagrid('getSelected');

    if (row == null) {
        $.messager.alert('提示', '未选中任何数据!');
        return;
    }
    $.messager.confirm('提示窗', '您确认删除吗?', function (event) {
        if (event) {
            $.ajax({
                type: 'POST',
                url: "../Pay/DeleteInsurance",
                data: {
                    id: row.ImportId
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

//查看详情
opt.querydetail = function () {
    // TODO 
    var row = $('#dg').datagrid('getSelected');
    if (row == null) {
        $.messager.alert('提示', '未选中任何数据!');
        return;
    }
    var id = row.ImportId;
    var url = '../Pay/ImportorDetail?importId=' + id;
    self.location = url;
}

//上传文件
opt.import = function () {
    $('#winUpload').upload({
        multiple: false,
        params: {},
        ext: 'xlsx,xls',
        url: '../Pay/Import',
        onAfterUpload: function (result) {
            if (result.success === true) {
                if (result.data != '') {
                    $.messager.alert('提示消息', result.data, 'warning', function () {
                        self.location.reload();
                    });
                }
                else
                    self.location.reload();
            }
            else {
                alert(result.data);
            }
        }
    });

    if (!($('#importInformation').length > 0)) {
        var txtContent = '<div id="importInformation" style="padding-left:20px;color:red;"> * 请按此格式规范文件名称：单位名称+年月+险种名称（人事局2018年7月工伤缴费表）<br/> </div>';
        $('#winUpload').upload('addElement', txtContent);
    }


    $('#winUpload').upload('show');
};
opt.import_1 = function () {
    $('#inputform').form('submit', {
        url: '../Pay/ImportorEditor',
        success: function (result) {
            var retuDa = $.parseJSON(result)
            $('#inputwindow').window('close');
            $.messager.alert('提示', retuDa);
            opt.query();
        },
        error: function () {
            $('#inputwindow').window('close');
            $.messager.alert('提示', "服务器内部错误，请联系管理员");
            opt.query();
        }
    });
};
//时间窗口
opt.showDialogByTime = function () {
    $('#inputwindow').dialog('open');
};
opt.exportByTime = function () {
    var month = $('#datebox').datebox('getText');
    var url = "../Pay/ExportJiaoCun?month=" + month;
    var params = {};
    HR.DownFile(url, params);
};
