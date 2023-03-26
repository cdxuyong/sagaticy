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
    var url = '../Company/QueryAccountImports';
    var params = { q: $('#txtQuery').val() };
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
//查看详情
opt.querydetail = function () {
    // TODO 
    var row = $('#dg').datagrid('getSelected');
    if (row == null) {
        $.messager.alert('提示', '未选中任何数据!');
        return;
    }
    debugger
    var id = row.ImportName;
    var url = '../Company/AccountImportDetail?q=' + id;
    self.location = url;
}

//上传文件
opt.import = function () {
    $('#winUpload').upload({
        multiple: false,
        params: {},
        ext: 'xlsx,xls',
        url: '../Company/ImportAccountDetail',
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
        var txtContent = '<div id="importInformation" style="padding-left:20px;color:red;"> * 请按此格式规范文件名称：日期+或单位+结算单（2022年9月结算单）<br/> </div>';
        $('#winUpload').upload('addElement', txtContent);
    }


    $('#winUpload').upload('show');
};
//时间窗口
opt.showDialogByTime = function () {
    $('#inputwindow').dialog('open');
};

