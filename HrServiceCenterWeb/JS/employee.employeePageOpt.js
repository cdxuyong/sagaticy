var opt = window.NameSpace || {};

function init() {
    if (dataId > 0) {
        opt.query();
    }
    else {
        if (CompanyId>0)
            $('#cmbCompany').combobox('setValue', CompanyId);
    }
}

opt.positions = new Array();
//查询信息
opt.query = function () {

    var url = '../Employee/GetEmployee?personId=' + dataId;
    $.ajax({
        url: url,
        type: "GET",
        dataType: "json",
        data: null,
        success: function (data) {
            HR.Form.setValues('formEmployee', data);
        },
        error: function () {
            $.messager.alert('提示', '查询出错！');
        }
    });
}

opt.add = function () {
    var companyId = $('#cmbCompany').combobox('getValue');
    var url = '../Employee/EmployeePage?id=0&companyId=' + companyId;
    self.location = url;
}
// 保存
opt.save = function () {
    var validate = false;
    var companyId = $('#cmbCompany').combobox('getValue');
    var companyText = $('#cmbCompany').combobox('getText');
    var companyList = $('#cmbCompany').combobox('getData');
    companyList.forEach(function (item, i) {
        if (item.CompanyId == companyId) {
            validate = true;
        }
    });
    if (!validate) {
        $.messager.alert('警告', '请从下拉列表中选择单位！');
        return;
    }
    var validate = $('#formEmployee').form('validate');
    if (!validate) {
        $.messager.alert('警告', '请填写人员必填信息！');
        return;
    }
    var o = HR.Form.getValues('formEmployee');
    o.PersonId = dataId;
    var url = '../Employee/SaveEmployee';
    //HR.Loader.show("loading...");
    $.messager.progress({
        msg: '保存中，请稍候...'
    });
    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        dataType: "json",
        data: JSON.stringify(o),
        success: function (result) {
            //HR.Loader.hide();
            if (result.success) {
                dataId = result.data;
                setTimeout(function () {
                    $.messager.progress('close');
                    if (o.State == 1) self.location.reload(true);
                }, 1000);
            }
            else {
                $.messager.progress('close');
                $.messager.alert('提示', '保存失败：' + result.data);
            }

        },
        error: function () {
            $.messager.alert('提示', '查询出错！');
        }
    });
};
//删除操作
opt.exit = function (id) {
    self.close();
    //self.location.href = '../Employee/EmployeeList';
};
opt.afterChange = function (idString) {
    if (idString.length < 17) return;
    var year = idString.substr(6, 4);
    var date = year + '-' + idString.substr(10, 2) + '-' + idString.substr(12, 2);
    var sex = idString.substr(16, 1) % 2;
    var retireYear = sex == 1 ? 60 : 50;
    var retireDate = (retireYear + parseInt(year)) + '-' + idString.substr(10, 2) + '-' + idString.substr(12, 2);
    $('#dtBirthday').datebox('setValue', date);
    $('#dtRetireTime').datebox('setValue', retireDate);
    $('#cmbSex').combobox("setValue", sex == 1 ? '男' : '女');
};

function myformatter(date) {
    var y = date.getFullYear();
    var m = date.getMonth() + 1;
    var d = date.getDate();
    return y + '-' + (m < 10 ? ('0' + m) : m) + '-' + (d < 10 ? ('0' + d) : d);
}