var CodeManageOpt = window.NameSpace || {};

//初始化页面参数
function init() {
    CodeManageOpt.queryCodes();
    $('#edit').window({
        onBeforeClose: function () {
            // $("#txtId").val("");
        },
        onOpen: function () {
        }
    })

}

//查询用户列表
CodeManageOpt.queryCodes = function () {
    var ukey = $('#ukey').val();
    $('#dg').datagrid({
        url: '../BaseCode/GetAllCodes?query=' + ukey,
        fitColumns: true,
        rownumbers: true,
        singleSelect: true,
        autoRowHeight: false,
        pagination: true,
        nowrap: false,
        pageSize: 20,
        striped: true,
        idField: 'Id',
        queryParams: null,
        type: 'json',
        loadFilter: pagerFilter,
        method: 'get',
        onLoadSuccess: function (data) {
            //self.parent.after_loadIframe();
        },
        onLoadError: function (data) {
            $.messager.alert('错误提示', '加载数据失败，请重试！');
        }
    });
}

CodeManageOpt.saveBtn = function () {
    var success = $("#edit").form('validate');//验证非空
    if (success) {
        var o = HR.Form.getValues('edit');
        $.ajax({
            url: '../BaseCode/SaveCode',
            type: "POST",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(o),
            success: function (result) {
                $('#edit').window('close');
                if (result.success) {
                    CodeManageOpt.queryCodes();
                    $.messager.alert('提示', '保存成功！');
                }
                else {
                    $.messager.alert('提示', '保存失败！');
                }

            }
        });
    }
}

CodeManageOpt.addCode = function () {
    // 选择上级后再新增
    $('#edit').panel({ title: "新增编码" });
    $('#edit').window('open');
    // $('#cmbCategoryId').enable();
    $.ajax({
        type: 'get',
        url: "../BaseCode/NewCode?parentId=4",
        dataType: "json",
        success: function (result) {
            result.Text = '';
            HR.Form.setValues('edit', result);
        }
    });
}

//编辑
CodeManageOpt.editCode = function (id) {
    $('#edit').panel({ title: "基础编码维护窗" });
    $('#edit').window('open');
    $.ajax({
        type: 'get',
        url: "../BaseCode/GetCode?id=" + id,
        dataType: "json",
        success: function (result) {
            HR.Form.setValues('edit', result);
            //$('#cmbCategoryId').disable();
        }
    });
}

CodeManageOpt.delete = function (id) {
    $.messager.confirm('删除基础数据编码', '您确认删除吗?', function (event) {
        if (event) {
            debugger
            var o = {
                Id : id
            };
            $.ajax({
                type: 'POST',
                url: "../BaseCode/DeleteCode",
                contentType: "application/json",
                dataType: "json",
                data: JSON.stringify(o),
                dataType: "json",
                success: function (result) {
                    if (result.success) {
                        CodeManageOpt.queryCodes();
                        $.messager.alert('提示', '保存成功！');
                    }
                    else {
                        $.messager.alert('提示', '保存失败！');
                    }
                }
            });
        }
        else {
            return;
        }
    });
}

//动态生成操作列
CodeManageOpt.formatActions = function (val, row) {
    var id = row.Id;
    var deletes = '<span title="删除" style="margin-left:20px; "><a href="javascript:void(0)" onclick="CodeManageOpt.delete(' + id + ')"><i class="fa fa-trash fa-lg" aria-hidden="true"></i></a></span>';
    var edit = '<span title="编辑" "><a href="javascript:void(0)" onclick="CodeManageOpt.editCode(' + id + ')"><i class="fa fa-pencil fa-lg" aria-hidden="true"></i></a></span>';
    //var key = '<span title="重置密码" style="margin-left:20px;"><a href="javascript:void(0)" onclick="UserManageOpt.resetPwd(' + id + ')"><i class="fa fa-key fa-lg" aria-hidden="true"></i></a></span>';
    var html = '<div style="margin:0 auto; display: inline-block !important; display: inline;">' + edit + deletes + '</div>';
    return html;
}