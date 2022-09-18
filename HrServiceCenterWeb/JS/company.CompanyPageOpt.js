var opt = window.NameSpace || {};

function init() {
    if (dataId > 0) {
        opt.query();
    }
    opt.setControlState();
}

opt.positions = new Array();
//查询信息
opt.query = function () {
    
    var url = '../Company/GetCompany';
    var params = { id: dataId };
    $.ajax({
        url: url,
        type: "POST",
        dataType: "json",
        data: params,
        success: function (data) {
            HR.Form.setValues('formCompany', data);
            $('#dg').datagrid('loadData', data.Positions);
            $('#dg').datagrid('loaded');
        },
        error: function () {
            $.messager.alert('提示', '查询出错！');
        }
    });

    url = '../BaseCode/GetPositions';
    $.ajax({
        url: url,
        type: "GET",
        dataType: "json",
        data: null,
        success: function (data) {
            opt.positions = data;
        },
        error: function () {
            $.messager.alert('提示', '查询出错！');
        }
    });
}

opt.add = function () {
    var url = '../Company/CompanyPage?id=0';
    self.location = url;
}
opt.setControlState = function () {
    if (dataId == 0) {
        $('[name="onOldData"]').attr("disabled", true);
    }
    else {
        $('[name="onOldData"]').removeAttr("disabled");

    }
}
// 保存
opt.save = function () {
    var o = HR.Form.getValues('formCompany');
    o.CompanyId = dataId;
    var url = '../Company/SaveCompany';
    HR.Loader.show("loading...");
    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        dataType: "json",
        data: JSON.stringify(o),
        success: function (result) {
            if (result.success) {
                HR.Loader.hide();
                dataId = result.data;
                opt.setControlState();
            }
            else {
                $.messager.alert('提示', '保存失败！');
            }

        },
        error: function () {
            $.messager.alert('提示', '查询出错！');
        }
    });
}
// 充值
opt.recharge = function () {
    $('#divRecharge').show();
}
opt.saveRecharge = function () {
    if (dataId == 0) {
        $.messager.alert('提示', '请先保存公司信息！');
        return;
    }
    var money = $('#nbRecharge').textbox('getValue');
    var url = '../Company/SaveRecharge';
    HR.Loader.show("loading...");
    $('#btnSaveRecharge').attr('disabled', true);
    var param = { companyId: dataId, money: money };
    $.ajax({
        url: url,
        type: "POST",
        dataType: "json",
        data: param,
        success: function (data) {
            if (data.success) {
                HR.Loader.hide();

            }
            else {
                $.messager.alert('提示', '保存失败！');
            }

        },
        error: function () {
            $.messager.alert('提示', '查询出错！');
        }
    });

}
//删除操作
opt.delete = function (id) {

}

opt.editPosition = function(position){
    HR.Form.setValues('frmPosition', position);
    $('#txtPositionCount').textbox('setValue', position.PlanCount);
    $('#dlg').dialog('open');
}
opt.addPosition = function () {
    $('#dlg').dialog('open');
    $('#cmbPosition').combobox('setValue', '');
}
opt.selectPosition = function (position) {
    return;
    this.positions.forEach(function (p, index) {
        if (p.PositionId == position.PositionId) {
            HR.Form.setValues('frmPosition', p);
            $('#txtPositionCount').textbox('setValue', position.PlanCount);
        }
    });
}
opt.savePosition = function () {
    var param = HR.Form.getValues('frmPosition');
    param.CompanyId = dataId;
    var url = '../Company/SavePosition';
    HR.Loader.show();
    $.ajax({
        url: url,
        type: "POST",
        dataType: "json",
        data: param,
        success: function (data) {
            if (data.success) {
                HR.Loader.hide();
                $('#dlg').dialog('close');
                opt.query();
            }
            else {
                $.messager.alert('提示', '保存失败！');
            }

        },
        error: function () {
            $.messager.alert('提示', '查询出错！');
        }
    });
}

opt.removePosition = function () {
    $.messager.confirm('警告', '数据删除无法恢复，您确定要删除?', function (r) {
        if (r) {
            var row = $('#dg').datagrid('getSelected');
            row.CompanyId = dataId;
            if (row == null) {
                $.messager.alert('提示', '未选中任何数据!');
                return;
            }
            var url = '../Company/DeletePosition';
            $.ajax({
                url: url,
                type: "GET",
                dataType: "json",
                data: row,
                success: function (data) {
                    $.messager.show({ title: '提示', msg: '删除成功.', timeout: 2000, showType: 'slide' });
                    opt.query();
                },
                error: function () {
                    $.messager.alert('提示', '删除失败！');
                }
            });
        }
    });
}
