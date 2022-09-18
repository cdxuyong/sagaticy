var opt = window.NameSpace || {};

function init() {
    var height = $('.container-layout').height() - 160;
    $('#dgContainer').height(height);
    $('#dg').datagrid('resize');
    opt.query();
}


//查询用户列表
opt.query = function () {
    var url =  '../Company/GetCompanyList';
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
opt.delete = function () {
    $.messager.confirm('提示窗', '您确认删除吗?', function (event) {
        if (event) {
            var row = $('#dg').datagrid('getSelected');
            var id = row.CompanyId;
            $.ajax({
                type: 'GET',
                url: "../Company/DeleteCompany?id=" + id,
                dataType: "json",
                success: function (result) {
                    $.messager.alert('提示', result.data);
                    if (result.success) {
                        opt.query();
                    }
                }
            });
        }
        else {
            return;
        }
    });
}


opt.add = function () {
    var url = '../Company/CompanyPage?id=0';
    self.location = url;
}

//编辑用户
opt.edit = function (id) {
    var row = $('#dg').datagrid('getSelected');
    if (row == null) {
        $.messager.alert('提示', '未选中任何数据!');
        return;
    }
    var id = row.CompanyId;
    var url = '../Company/CompanyPage?id=' + id;
    self.location = url;
};

opt.export = function () {
    var url = "../Company/Export";
    var params = {};

    HR.DownFile(url, params);
};

opt.import = function () {
    $('#winUpload').upload({
        multiple: false,
        params: {},
        ext: 'xlsx',
        url: '../Company/Import',
        onAfterUpload: function (result) {
            if (result.success === true) {
                self.location.href = '../Company/CompanyList';
            }
            else {
                alert(result.data);
            }
        }
    });

    $('#winUpload').upload('show');
};


function grid_ondblclick(index, field, value) {

    var id = value.CompanyId;
    opt.edit(id);
}