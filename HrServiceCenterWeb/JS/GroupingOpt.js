var GroupingOpt = window.NameSpace || {};

GroupingOpt.loadRoleUsers = function () {
    $("#dg").datagrid({
        url: '../System/GetUserList',
        rownumbers: true,
        singleSelect: false,
        autoRowHeight: true,
        pagination: true,
        nowrap: false,
        loadFilter: pagerFilter,
        pageSize: 10,
        pageList: [10, 20, 30, 50, 50],
        striped: true,
        method: 'post',
        onBeforeLoad: function (params) {
        },
        onLoadSuccess: function (data) {
            $.ajax({
                type: 'POST',
                url: "../System/LoadRoleUsers",
                async: false,
                data: { RoleId: $("#roleId").val() },
                dataType: "json",
                success: function (result) {
                    if (data && result != "") {
                        $.each(data.rows, function (index, item) {
                            if (result.indexOf(item.UserId) > -1) {
                                $('#dg').datagrid('checkRow', index);
                            }
                        });
                    }
                }
            });
        },
        onLoadError: function (data) {
            $.messager.alert('错误提示', '加载数据失败，请重试！', 'error');
        }
    })
}

//获取选中行的USERID
GroupingOpt.getUserId = function() {
    var rows = $('#dg').datagrid('getSelections');
    if (rows.length == 0) {
        return "";
    }
    else {
        var re = "";
        rows.forEach(function (item) {
            re += "," + item.UserId;
        });
    }
    return re.substr(1);
}

//保存操作（分组的用户）
GroupingOpt.Save = function() {
    var roleid = $("#roleId").val();
    var Grouping = GroupingOpt.getUserId();
    $.ajax({
        url: "../System/SaveRoleUsers",
        type: "POST",
        //data:obj,
        data: {
            RoleId: roleid,
            Grouping: Grouping//分组
        },
        success: function (msg) {
            $.messager.alert('成功', msg);
        },
        error: function () {
            $.messager.alert('失败', '提交失败');
        }
    });
}