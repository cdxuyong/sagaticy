var opt = window.NameSpace || {};

function init() {
    var height = $('.container-layout').height() - 180;
    $('#dgContainer').height(height);
    $('#dg').datagrid('resize');

    $('#dg').datagrid('enableFilter');

    opt.query();
}


//查询用户列表
opt.selectCompanyId = 0;
opt.query = function () {
    var url = '../Employee/GetEmployeeList';

    var params = { companyId: opt.selectCompanyId };
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

opt.selectCompany = function (company) {
    opt.selectCompanyId = company.CompanyId;
    opt.query();
};


//删除操作
opt.delete = function (id) {
    $.messager.confirm('提示窗', '您确认删除吗?', function (event) {
        if (event) {
            var row = $('#dg').datagrid('getSelected');
            var id = row.PersonId;
            $.ajax({
                type: 'POST',
                url: "../Employee/DeleteEmployee",
                data: {
                    personId: id
                },
                dataType: "json",
                success: function (result) {
                    $.messager.alert('提示', result.data);
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
    var url = '../Employee/EmployeePage?id=0';
    self.location = url;
};
opt.formatRow = function (index, row) {
    if (row.State == 1)
        return 'color:#ffcc00';
};
//编辑用户
opt.edit = function (id) {
    var row = $('#dg').datagrid('getSelected');
    if (row === null) {
        $.messager.alert('提示', '未选中任何数据!');
        return;
    }
    var pid = row.PersonId;
    var url = '../Employee/EmployeePage?id=' + pid;
    window.open(url);
};
//导出详情
opt.export = function () {
    var url = '../Employee/ExportPayDetail';
    var params = { companyId: opt.selectCompanyId };
    HR.DownFile(url, params);
};
//导出花名册
opt.exportSimple = function () {
    var url = '../Employee/exportSimple';
    var params = { companyId: opt.selectCompanyId };
    HR.DownFile(url, params);
};
opt.import = function () {
    $('#winUpload').upload({
        title: '人员档案导入对话框',
        multiple: false,
        params: {},
        ext: 'xlsx',
        url: '../Employee/Import',
        onAfterUpload: function (result) {
            if (result.success === true) {
                self.location.href = '../Employee/EmployeeList';
            }
            else {
                alert(result.data);
            }
        }
    });

    if (!($('#importInformation').length > 0)) {
        var txtContent = '<div id="importInformation" style="padding-left:20px;padding-top:10px; color:red;"> *注意： <br/> 1.编号是不能重复的数字编码；<br/>2.身份证号码不能重复；<br/>3.单位名称不能错误 </div>';
        $('#winUpload').upload('addElement', txtContent);
    }

    $('#winUpload').upload('show');
};

