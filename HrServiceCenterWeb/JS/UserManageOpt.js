var UserManageOpt = window.NameSpace || {};

var oldName;
//初始化页面参数
function init() {
    UserManageOpt.queryUser();
    UserManageOpt.loadOrg();
    $('#edit').window({
        onBeforeClose: function () {
            $("#txtName").val("");
            $("#tre").val("");
            UserManageOpt.loadOrg();
            $('#txtName').validatebox('enableValidation');
            $('#tre').validatebox('enableValidation');
        },
        onOpen: function () {
            $('#txtName').validatebox('validate');
            $('#tre').validatebox('validate');
        }
    })

}

//加载用户归属组织下拉列表
UserManageOpt.loadOrg = function () {
    HR.Form.bindCombox('cmbOrg', '../System/GetOrgs', null, true);
}

//查询用户列表
UserManageOpt.queryUser = function () {
    var que = $("#uname").val();
    $("#tname").val(que);
    var obj = HR.Form.getValues('pnlSearch');
    $('#dg').datagrid({
        url: '../System/UsersQuery?userName=' + que,
        fitColumns: true,
        rownumbers: true,
        singleSelect: true,
        autoRowHeight: false,
        pagination: true,
        nowrap: false,
        pageSize: 10,
        striped: true,
        idField: 'UserId',
        queryParams: obj,
        type: 'json',
        loadFilter: pagerFilter,
        method: 'post',
        onLoadSuccess: function (data) {
            //self.parent.after_loadIframe();
        },
        onLoadError: function (data) {
            $.messager.alert('错误提示', '加载数据失败，请重试！');
        }
    });
}

//加载用户
UserManageOpt.loadUser = function (id) {
    $.ajax({
        type: 'POST',
        url: "../System/LoadUser",
        data: {
            UserId: id
        },
        dataType: "json",
        success: function (result) {
            HR.Form.setValues('edit', result);
            //$("#newName").val($("#txtName").val());
            oldName = $("#txtName").val();
        }
    });
}

//提交更新/新增用户
UserManageOpt.saveBtn = function () {
    var success = $("#edit").form('validate');//验证非空
    if (success) {
        var obj = HR.Form.getValues('edit');
        var Url;
        if ($("#userId").val() == "0") {
            Url = "../System/AddUser";
            obj = $.extend({}, obj, { Password: "123456" });
        }
        else {
            Url = "../System/UpDateUser?oldName=" + oldName;
        }
        $.ajax({
            type: 'POST',
            url: Url,
            data: obj,
            dataType: "json",
            success: function (result) {
                $('#edit').window('close');
                $.messager.alert('提示', result);
                UserManageOpt.queryUser();
            }
        });
    }
}

//删除操作
UserManageOpt.delete = function (id) {
    $.messager.confirm('删除用户', '您确认删除吗?', function (event) {
        if (event) {
            $.ajax({
                type: 'POST',
                url: "../System/DeleteUser",
                data: {
                    UserId: id
                },
                dataType: "json",
                success: function (result) {
                    $.messager.alert('提示', result);
                    UserManageOpt.query();
                }
            });
        }
        else {
            return;
        }
    });
}

//重置密码
UserManageOpt.resetPwd = function (id) {
    $.messager.confirm('重置密码', '是否重置该用户的密码为：123456', function (event) {
        if (event) {
            $.ajax({
                type: 'POST',
                url: "../System/InitPwd",
                data: {
                    UserId: id,
                    Password: "123456"
                },
                dataType: "json",
                success: function (result) {
                    $.messager.alert('提示', result);
                }
            });
        }
        else {
            return;
        }
    });
}

//新增用户
UserManageOpt.addUser = function () {
    $('#edit').panel({ title: "新增用户" });
    $("#userId").val("0");
    $('#edit').window('open');
}

//编辑用户
UserManageOpt.editUser = function (id) {
    $('#edit').panel({ title: "编辑用户" });
    $("#userId").val(id);
    $('#edit').window('open');
    UserManageOpt.loadUser(id);
}

//动态生成操作列
UserManageOpt.formatActions = function (val, row) {
    var id = row.UserId;
    var deletes = '<span title="删除" style="margin-left:20px; "><a href="javascript:void(0)" onclick="UserManageOpt.delete(' + id + ')"><i class="fa fa-trash fa-lg" aria-hidden="true"></i></a></span>';
    var edit = '<span title="编辑" "><a href="javascript:void(0)" onclick="UserManageOpt.editUser(' + id + ')"><i class="fa fa-pencil fa-lg" aria-hidden="true"></i></a></span>';
    var key = '<span title="重置密码" style="margin-left:20px;"><a href="javascript:void(0)" onclick="UserManageOpt.resetPwd(' + id + ')"><i class="fa fa-key fa-lg" aria-hidden="true"></i></a></span>';
    var html = '<div style="margin:0 auto; display: inline-block !important; display: inline;">' + edit + key + deletes + '</div>';
    return html;
}