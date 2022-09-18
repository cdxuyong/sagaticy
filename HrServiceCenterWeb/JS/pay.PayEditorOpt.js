var opt = window.NameSpace || {};

function init() {
    opt.init_Buttons();
    if (dataId > 0) {
        opt.loadPayment();
    }
}

opt.init_Buttons = function () {
    if (dataId === 0) {
        $('#btnSave').attr('disabled', true);
        $('#btnExport').attr('disabled', true);
        $('#btnImport').attr('disabled', true);
        $('#btnSubmit').attr('disabled', true);
    }
    else {
        $('#btnCreate').attr('disabled', true);
    }
};
opt.disable = function () {
    $('#btnSave').attr('disabled', true);
    $('#btnExport').attr('disabled', true);
    $('#btnImport').attr('disabled', true);
    $('#btnSubmit').attr('disabled', true);
    $('#btnCancel').attr('disabled', false);
};
opt.autoName = function (company) {
    var date = $('#datebox').datebox('getText');
    var cmp = $('#cmpname').combobox('getText');
    var name = cmp + date + '工资表';
    $('#tempname').val(name);
    $('#cmpMoney').html('当前余额：' + company.AccountBalance + '元');
};

opt.save = function () {
    var o = HR.Form.getValues('formPayment');
    o.PayId = dataId;
    var url = '../Payment/SavePayment';
    HR.Loader.show("loading...");
    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        dataType: "json",
        data: JSON.stringify(o),
        success: function (result) {
            HR.Loader.hide();
            $.messager.show({
                title: '提示',
                msg: '保存成功.',
                timeout: 2000,
                showType: 'slide'
            });
        },
        error: function () {
            $.messager.alert('提示', '保存失败！');
        }
    });
}

opt.submitPayment = function () {
    $.messager.confirm('提示窗', '提交扣款后则无法修改，确定要提交扣款？', function (event) {
        if (event) {
            var url = '../Payment/SubmitPayment?paymentId=' + dataId;
            HR.Loader.show("loading...");
            $.ajax({
                url: url,
                type: "POST",
                contentType: "application/json",
                dataType: "json",
                data: null,
                success: function (result) {
                    HR.Loader.hide();
                    $.messager.alert('提示', result.message);
                    if (result.success) {
                        opt.disable();
                    }
                },
                error: function () {
                    HR.Loader.hide();
                    $.messager.alert('提示', '保存失败！');
                }
            });
        }
        else {
            return;
        }
    });
};

opt.createPayment = function () {
    var o = HR.Form.getValues('formPayment');
    var url = '../Payment/CreatePayment';
    HR.Loader.show("loading...");
    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        dataType: "json",
        data: JSON.stringify(o),
        success: function (result) {
            HR.Loader.hide();
            if (result.success) {
                dataId = result.data;
                self.location = "../Pay/PayEditor?id=" + dataId;
            }
            else {
                $.messager.alert('提示', '保存失败！');
            }

        },
        error: function () {
            $.messager.alert('提示', '创建发放表出错！');
        }
    });

};

opt.loadPayment = function () {
    var url = '../Payment/LoadPayment?payId=' + dataId;
    HR.Loader.show("loading...");
    $.ajax({
        url: url,
        type: "GET",
        contentType: "application/json",
        dataType: "json",
        data: null,
        success: function (payment) {
            HR.Loader.hide();
            if (payment.Status > 0)
                opt.disable();
            HR.Form.setValues('formPayment',payment);
            opt.createGrid(payment.Items, payment.Sheet);
            // 查询公司余额 
            var companys = $('#cmpname').combobox('getData');
            companys.forEach(function (o, index) {
                if (o.CompanyId === payment.CompanyId) {
                    $('#cmpMoney').html('当前余额：' + o.AccountBalance + '元');
                }
            });
        },
        error: function () {
            HR.Loader.hide();
            $.messager.alert('提示', '生成发放表出错！');
        }
    });
}
opt.createGrid = function (items, table) {
    var obj = { "total": 2, "rows": table };
    var head1 = [
        { field: 'PersonId', title: 'ID', rowspan: 2, width: 0 },
        { field: 'PersonName', title: '姓名', rowspan: 2, width: 60 },
        { field: 'PersonCode', title: '身份证', rowspan: 2, width: 100 }
    ];
    var head2 = [];
    for (var i = 0; i < items.length; i++) {
        var item = items[i];
        var column = {};
        column.field = item.ItemName;
        column.title = item.ItemCaption;
        if (item.ParentId > 0) {
            column.width = 50;
            head2.push(column);
        }
        else {
            var colSpan = 1;
            for (var j = 0; j < items.length; j++) {
                if (items[j].ParentId === item.ItemId)
                    colSpan++;
            }
            if (colSpan === 1)
                column.rowspan = 2;
            else
                column.colspan = colSpan - 1;
            head1.push(column);
        }

    }
    var fixColumns = head1;
    var columns = [head1, head2];

    $('#dg').datagrid({
        frozenColumns: [],
        columns: columns,
        data: table,
        nowrap: false,
        rownumbers: false,
        singleSelect: true,
        collapsible: true,
        autoRowHeight: false,
        fitColumns: false,
        showFooter: true,
        onClickRow: function () {
        },
        onClickCell: function (rowIndex, field, value) {

        },
        striped: true
    });
};

opt.export = function () {
    var title = $('#dirTitle').val();
    var url = "../Payment/Export?payId=" + dataId;
    var params = {};

    HR.DownFile(url, params);
}
opt.import = function () {
    if (dataId === 0) return false;
    $('#winUpload').upload({
        multiple: false,
        params: { payId: dataId},
        ext: 'xlsx',
        url: '../Payment/Import?payId='+dataId,
        onAfterUpload: function (result) {
            if (result.success === true) {
                self.location.href = '../Pay/PayEditor?id=' + dataId;
            }
            else {
                alert(result.data);
            }
        }
    });

    $('#winUpload').upload('show');
}

opt.fullWindows = function () {
    $('.container').css('width', '100%');
    $('#dg').datagrid('resize');
};

opt.cancle = function () {
    $.messager.confirm('提示窗', '撤销归档，将退回单位扣款，确定要撤销归档？', function (event) {
        if (event) {
            var url = '../Payment/CancelPayment?paymentId=' + dataId;
            HR.Loader.show("loading...");
            $.ajax({
                url: url,
                type: "POST",
                contentType: "application/json",
                dataType: "json",
                data: null,
                success: function (result) {
                    HR.Loader.hide();
                    $.messager.alert('提示', result.message);

                    if (result.success) {
                        self.location.reload(true);
                    }
                },
                error: function () {
                    HR.Loader.hide();
                    $.messager.alert('提示', '保存失败！');
                }
            });
        }
        else {
            return;
        }
    });
};
/*
 * 
$.fn.datebox.defaults.formatter = function (date) {
    var y = date.getFullYear();
    var m = date.getMonth() + 1;
    var d = date.getDate();
    return y + '-' + m + '-1';
}
*/
