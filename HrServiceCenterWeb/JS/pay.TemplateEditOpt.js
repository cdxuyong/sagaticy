var opt = window.NameSpace || {};

function init() {
    var height = $('.container-layout').height() - 160;
    $('#dgContainer').height(height);
    var tempId = $("#hideview").val();
    if (tempId != 0) {
        $("#form-cmp").hide();
    }
    else {
        opt.loadCmp();
    }
    opt.query(tempId);
    opt.loadTree(tempId);
}

//加载公司下拉
opt.loadCmp = function () {
    HR.Form.bindCombox('cmpname', '../Company/GetCompanyList?query=', null, true);
}


//查询模板列表
opt.query = function (dataId) {
    if (dataId == 0) return;
    var url = '../Pay/QueryTemplate';
    var params = { id: dataId };
    $.ajax({
        url: url,
        type: "POST",
        dataType: "json",
        data: params,
        success: function (data) {
            HR.Form.setValues('formTemplate', data);
        },
        error: function () {
            $.messager.alert('提示', '查询出错！');
        }
    });
}

//加载树
opt.loadTree = function (tempId) {
    $('#tree').tree({
        url: '../Pay/GetTemplateTree',
        checkbox: true,
        onContextMenu: function (e, node) {
            e.preventDefault();
            $('#tree').tree('select', node.target);
        },
        onLoadSuccess: function (data) {
            if (tempId == 0) return;
            $.ajax({
                type: 'POST',
                url: "../Pay/GetTemplateByTable?id=" + tempId,
                async: false,
                dataType: "json",
                success: function (result) {
                    if (result != "") {
                        var menuarr = result.split(",");
                        menuarr.forEach(function (item) {
                            var node = $('#tree').tree('find', item);
                            $('#tree').tree('check', node.target);
                        });
                    }
                }
            });
        },
        onLoadError: function (data) {
            $.messager.alert('错误提示', '加载数据失败，请重试！');
        }
    });
}

//保存操作SaveTemplateForTable  
opt.save = function () {
    var tempRight = opt.GetTreeData;
    var tempId = $("#hideview").val();
    if (tempId == 0) {
        var cmpId = $('#cmpname').combobox('getValue');
        $.ajax({
            url: '../Pay/SaveTemplateMsg',
            type: 'POST',
            datatype: 'json',
            data: {
                id: cmpId,
                temps: tempRight
            },
            success: function (data) {
                $.messager.alert('提示', data);
            },
            error: function () {
                $.messager.alert('错误提示', '分配失败，请联系管理员', 'error');
            }
        })
    }
    else {
        $.ajax({
            url: '../Pay/SaveTemplateForTable',
            type: 'POST',
            datatype: 'json',
            data: {
                id: tempId,
                temps: tempRight
            },
            success: function (data) {
                $.messager.alert('提示', data);
            },
            error: function () {
                $.messager.alert('错误提示', '分配失败，请联系管理员', 'error');
            }
        })
    }
}

//获取选中树形菜单ID
opt.GetTreeData = function () {
    var nodes = $('#tree').tree('getChecked', ['checked', 'indeterminate']);
    if (nodes.length == 0) {
        return "";
    }
    else {
        var re = "";
        nodes.forEach(function (item) {
            if (item.children == null) {
                re += "," + item.id;
            }
        });
    }
    return re.substr(1);
}
