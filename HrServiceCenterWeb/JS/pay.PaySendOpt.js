var opt = window.NameSpace || {};

function init() {
    var height = $('.container-layout').height() - 310;
    $('#dgContainer').height(height);
    $('#dg').datagrid('resize');
    opt.loadCmp();
    opt.initDateBox();
    //opt.query();

    var payId = $("#hideview").val();
    if (payId != 0) {
        opt.loadDetail();
    }
    else {
        $('#savebtn').click(function () {
            opt.save(0);
        });
    }
}

//加载发放表详情
opt.loadDetail = function () {
    //填充日期
    //填充发放表名称
    //填充公司名
    $("#savebtn").html('更新发放数据');
    $('#savebtn').click(function () {
        opt.update();
    });
    var payId = $("#hideview").val();
    $.ajax({
        url: '../Pay/GetPayDetailByPayId',
        type: 'post',
        data: { 'id': payId },
        success: function (result) {
            $("#datebox").datebox("setValue", result.PayMonth);
            $("#datebox").combobox('readonly', true);
            $('#cmpname').combobox('select', result.CompanyId);
            $("#cmpname").combobox('readonly', true);
            $("#tempname").attr("value", result.PayTitle);
            $("#tempname").attr("readonly", "readonly");
            $("#creatbtn").attr('disabled', true);
            if (result.Status == 2) {//已归档发放表不允许再提交
                $("#savebtn").attr('disabled', true);
                $("#filebtn").attr('disabled', true);
            }
            opt.loadTable();
        }
    })
}

//加载公司下拉
opt.loadCmp = function () {
    HR.Form.bindCombox('cmpname', '../Company/GetCompanyList?query=', null, true);
    $('#cmpname').combobox({
        onChange: function (newValue, oldValue) {
            if (newValue != undefined && newValue != '') {
                var text = $('#cmpname').combobox('getText');
                var month = $('#datebox').datebox('getValue');
                var newmonth = month.replace('-', '年') + '月';
                $("#tempname").attr("value", text + newmonth + '发放表');
            }
        }
    });
}

//加载人员
opt.loadTable = function () {
    var cmpId = $('#cmpname').combobox('getValue');
    if (cmpId == null || cmpId == undefined || cmpId == "") {
        $.messager.alert('提示', '请选择公司');
        return;
    }
    var payId = $("#hideview").val();
    $.ajax({
        url: "../Pay/RefreshTable",
        type: "Post",
        data: { "id": cmpId },
        success: function (data) {
            $('#dg').datagrid({
                title: ' ',
                url: '../Pay/GetPayDetail?cmpid=' + cmpId + '&payid=' + payId,
                columns: getCfg(data),
                onClickCell: onClickCell,
                onLoadSuccess: function (data) {
                    opt.loadCount();
                    opt.loadCountLab();
                }
            });
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            //alert("加载失败：" + errorThrown);
            $.messager.alert('提示', '加载失败');
        }
    });
    $('#dg').datagrid('resize');
}

$.extend($.fn.datagrid.methods, {
    editCell: function (jq, param) {
        return jq.each(function () {
            var opts = $(this).datagrid('options');
            var fields = $(this).datagrid('getColumnFields', true).concat($(this).datagrid('getColumnFields'));
            for (var i = 0; i < fields.length; i++) {
                var col = $(this).datagrid('getColumnOption', fields[i]);
                col.editor1 = col.editor;
                if (fields[i] != param.field) {
                    col.editor = null;
                }
            }
            $(this).datagrid('beginEdit', param.index);
            for (var i = 0; i < fields.length; i++) {
                var col = $(this).datagrid('getColumnOption', fields[i]);
                col.editor = col.editor1;
            }
        });
    }
});

var editIndex = undefined;
//结束编辑事件
function endEditing() {
    if (editIndex == undefined) { return true }
    if ($('#dg').datagrid('validateRow', editIndex)) {
        $('#dg').datagrid('endEdit', editIndex);
        tdCount();
        opt.loadCountLab();
        $('#dg').datagrid('refreshRow', editIndex);
        editIndex = undefined;
        return true;
    } else {
        return false;
    }
}

//动态统计单元格（小计、汇总）
function tdCount() {
    var row = $('#dg').datagrid('getSelected');//获取当前选中行数据
    var rowIndex = $('#dg').datagrid('getRowIndex', $('#dg').datagrid('getSelected'));  //选中行索引
    var a = row.BasicWage == undefined ? 0 : parseFloat(row.BasicWage);
    var b = row.AidWageOne == undefined ? 0 : parseFloat(row.AidWageOne);
    var c = row.AidWageTwo == undefined ? 0 : parseFloat(row.AidWageTwo);
    var d = row.AidWageThree == undefined ? 0 : parseFloat(row.AidWageThree);
    var e = row.Pension == undefined ? 0 : parseFloat(row.Pension);
    var f = row.Unemployment == undefined ? 0 : parseFloat(row.Unemployment);
    var g = row.Health == undefined ? 0 : parseFloat(row.Health);
    var h = row.BuckleCount == undefined ? 0 : parseFloat(row.BuckleCount);
    var m = row.TrueWage == undefined ? 0 : parseFloat(row.TrueWage);
    var j = row.Insurance == undefined ? 0 : parseFloat(row.Insurance);
    var k = row.Fund == undefined ? 0 : parseFloat(row.Fund);
    var l = row.ServiceWage == undefined ? 0 : parseFloat(row.ServiceWage);
    //应发工资
    if (row.WageCount != undefined) {
        row.WageCount = (opt.formatFloat(a + b + c + d, 10)).toString();
    }
    //费用总计
    if (row.Count != undefined) {
        row.Count = (opt.formatFloat(a + b + c + d - e - f - g + l, 10)).toString();
    }
    reloadRow(rowIndex, row);
}

//提高费精度
opt.formatFloat = function (f, digit) {
    var m = Math.pow(10, digit);
    return parseInt(f * m, 10) / m;
}

//重新加载行
function reloadRow(rowIndex, row) {
    $('#dg').datagrid('updateRow', {
        index: rowIndex,
        row: row
    });
}
function onClickCell(index, field) {
    if (endEditing()) {
        $('#dg').datagrid('selectRow', index)
            .datagrid('editCell', { index: index, field: field });
        editIndex = index;
    }
}

//动态加载表头
function getCfg(obj) {
    var arrys = new Array();
    var arry = new Array();
    for (let i = 0; i < obj.length; i++) {
        var varobj;
        if (obj[i].Subnode.length > 0) {
            if (obj[i].IsEdit == true) {
                varobj = { field: obj[i].FieldCode, title: obj[i].FieldTitle, width: 80 * obj[i].Subnode.length, rowspan: 1, colspan: obj[i].Subnode.length, align: 'center', editor: { type: 'text', options: { required: true } } };
            }
            else {
                varobj = { field: obj[i].FieldCode, title: obj[i].FieldTitle, width: 80 * obj[i].Subnode.length, rowspan: 1, colspan: obj[i].Subnode.length, align: 'center' };
            }
        }
        else if (obj[i].IsShow == false) {
            varobj = { field: obj[i].FieldCode, title: obj[i].FieldTitle, width: 80, rowspan: 2, align: 'center', hidden: 'true' };
        }
        else {
            varobj = { field: obj[i].FieldCode, title: obj[i].FieldTitle, width: 80, rowspan: 2, align: 'center' };
        }
        arry.push(varobj);
    }
    arrys.push(arry);
    arry = new Array();
    for (let i = 0; i < obj.length; i++) {
        if (obj[i].Subnode.length > 0) {
            for (let n = 0; n < obj[i].Subnode.length; n++) {
                var varobj;
                if (obj[i].Subnode[n].IsEdit == true) {
                    varobj = { field: obj[i].Subnode[n].FieldCode, title: obj[i].Subnode[n].FieldTitle, width: 80, rowspan: 1, align: 'center', editor: { type: 'text', options: { required: true } } };
                }
                else {
                    varobj = { field: obj[i].Subnode[n].FieldCode, title: obj[i].Subnode[n].FieldTitle, width: 80, rowspan: 1, align: 'center' };
                }
                arry.push(varobj);
            }
        }
    }
    arrys.push(arry);
    return arrys;
}


opt.loadCount = function () {
    var eaRows = $("#dg").datagrid('getRows');
    for (var num = 0; num < eaRows.length; num++) {
        var a = eaRows[num].BasicWage == undefined ? 0 : parseFloat(eaRows[num].BasicWage);
        var b = eaRows[num].AidWageOne == undefined ? 0 : parseFloat(eaRows[num].AidWageOne);
        var c = eaRows[num].AidWageTwo == undefined ? 0 : parseFloat(eaRows[num].AidWageTwo);
        var d = eaRows[num].AidWageThree == undefined ? 0 : parseFloat(eaRows[num].AidWageThree);
        var e = eaRows[num].Pension == undefined ? 0 : parseFloat(eaRows[num].Pension);
        var f = eaRows[num].Unemployment == undefined ? 0 : parseFloat(eaRows[num].Unemployment);
        var g = eaRows[num].Health == undefined ? 0 : parseFloat(eaRows[num].Health);
        var h = eaRows[num].BuckleCount == undefined ? 0 : parseFloat(eaRows[num].BuckleCount);
        var i = eaRows[num].TrueWage == undefined ? 0 : parseFloat(eaRows[num].TrueWage);
        var j = eaRows[num].Insurance == undefined ? 0 : parseFloat(eaRows[num].Insurance);
        var k = eaRows[num].Fund == undefined ? 0 : parseFloat(eaRows[num].Fund);
        var l = eaRows[num].ServiceWage == undefined ? 0 : parseFloat(eaRows[num].ServiceWage);
        eaRows[num].BuckleCount = (opt.formatFloat(e + f + g, 10)).toString();
        eaRows[num].Count = (opt.formatFloat(a + b + c + d - e - f - g + l, 10)).toString();
        reloadRow(num, eaRows[num]);
        $('#dg').datagrid('refreshRow', num);
    }
}

//加载总金额
opt.loadCountLab = function () {
    var eaRows = $("#dg").datagrid('getRows');
    var countpay = 0;
    for (var num = 0; num < eaRows.length; num++) {
        var a = eaRows[num].BasicWage == undefined ? 0 : parseFloat(eaRows[num].BasicWage);
        var b = eaRows[num].AidWageOne == undefined ? 0 : parseFloat(eaRows[num].AidWageOne);
        var c = eaRows[num].AidWageTwo == undefined ? 0 : parseFloat(eaRows[num].AidWageTwo);
        var d = eaRows[num].AidWageThree == undefined ? 0 : parseFloat(eaRows[num].AidWageThree);
        var e = eaRows[num].Pension == undefined ? 0 : parseFloat(eaRows[num].Pension);
        var f = eaRows[num].Unemployment == undefined ? 0 : parseFloat(eaRows[num].Unemployment);
        var g = eaRows[num].Health == undefined ? 0 : parseFloat(eaRows[num].Health);
        var h = eaRows[num].BuckleCount == undefined ? 0 : parseFloat(eaRows[num].BuckleCount);
        var i = eaRows[num].TrueWage == undefined ? 0 : parseFloat(eaRows[num].TrueWage);
        var j = eaRows[num].Insurance == undefined ? 0 : parseFloat(eaRows[num].Insurance);
        var k = eaRows[num].Fund == undefined ? 0 : parseFloat(eaRows[num].Fund);
        var l = eaRows[num].ServiceWage == undefined ? 0 : parseFloat(eaRows[num].ServiceWage);
        countpay += opt.formatFloat(a + b + c + d - e - f - g + l, 10);
    }
    $('#dg').datagrid("getPanel").panel("setTitle", "<span style='margin-left:76.4%'></span>总金额：<label id='paylab'>" + countpay + "</label>");
}

//创建发放表/归档发放表(status：0 未审核  2 归档)
opt.save = function (status) {
    var name = $('#tempname').val();
    if (name == '' || name == undefined) {
        $.messager.alert('提示', '请输入发放表名称');
        return;
    }
    var row = $('#dg').datagrid('getRows');
    if (row == undefined || row.length == 0) {
        $.messager.alert('提示', '发放表为空');
        return;
    }
    var paycount = $('#paylab').html();
    if (paycount <= 0) {
        $.messager.alert('提示', '发放金额应大于0');
        return;
    }
    var month = $('#datebox').datebox('getValue');
    $('#dg').datagrid('loading');
    $('#dg').datagrid('endEdit', editIndex);
    editIndex = undefined;
    var tasks = $("#dg").datagrid("getRows");
    var cmpId = $('#cmpname').combobox('getValue');
    var parms = {
        list: tasks,
        cmpid: cmpId,
        tname: name,
        time: month,
        count: paycount,
        stus: status
    };
    var Url;
    if (status == 2) { Url = "../Pay/OnFilePay"; }
    else { Url = "../Pay/SavePayDetail"; }
    $.ajax({
        url: Url,
        type: "post",
        contentType: "application/json",
        dataType: "json",
        data: JSON.stringify(parms),
        success: function (data) {
            $.messager.alert('提示', data);
            if (status == 2) {
                $("#savebtn").attr('disabled', true);
                $("#filebtn").attr('disabled', true);
            }
            $('#dg').datagrid('loaded');
        },
        error: function () {
            $.messager.alert('提示', '保存失败');
            $('#dg').datagrid('loaded');
        }
    });
}

opt.add = function () {
    // TODO 
    var url = '../Pay/PayEditor?id=0';
    self.location = url;
}

//编辑对象
opt.edit = function (id) {
    // TODO 
    var row = $('#dg').datagrid('getSelected');
    if (row == null) {
        $.messager.alert('提示', '未选中任何数据!');
        return;
    }
    var id = row.CompanyId;
    var url = '../Company/CompanyPage?id=' + id;
    self.location = url;
}

//初始化时间控件
opt.initDateBox = function () {
    $('#datebox').datebox({
        //显示日趋选择对象后再触发弹出月份层的事件，初始化时没有生成月份层
        onShowPanel: function () {
            //触发click事件弹出月份层
            span.trigger('click');
            if (!tds)
                //延时触发获取月份对象，因为上面的事件触发和对象生成有时间间隔
                setTimeout(function () {
                    tds = p.find('div.calendar-menu-month-inner td');
                    tds.click(function (e) {
                        //禁止冒泡执行easyui给月份绑定的事件
                        e.stopPropagation();
                        //得到年份
                        var year = /\d{4}/.exec(span.html())[0],
                            //月份
                            //之前是这样的month = parseInt($(this).attr('abbr'), 10) + 1; 
                            month = parseInt($(this).attr('abbr'), 10);

                        //隐藏日期对象                     
                        $('#datebox').datebox('hidePanel')
                            //设置日期的值
                            .datebox('setValue', year + '-' + month);
                    });
                }, 0);
        },
        //配置parser，返回选择的日期
        parser: function (s) {
            if (!s) return new Date();
            var arr = s.split('-');
            return new Date(parseInt(arr[0], 10), parseInt(arr[1], 10) - 1, 1);
        },
        //配置formatter，只返回年月 之前是这样的d.getFullYear() + '-' +(d.getMonth()); 
        formatter: function (d) {
            var currentMonth = (d.getMonth() + 1);
            var currentMonthStr = currentMonth < 10 ? ('0' + currentMonth) : (currentMonth + '');
            return d.getFullYear() + '-' + currentMonthStr;
        }
    });
    //日期选择对象
    var p = $('#datebox').datebox('panel'),
        //日期选择对象中月份
        tds = false,
        //显示月份层的触发控件
        span = p.find('span.calendar-text');
    var curr_time = new Date();

    //设置前当月
    $("#datebox").datebox("setValue", myformatter(curr_time));
}

//格式化日期
function myformatter(date) {
    //获取年份
    var y = date.getFullYear();
    //获取月份
    var m = date.getMonth() + 1;
    return y + '-' + m;
}

//归档操作
opt.onfile = function () {
    $.messager.confirm('提示窗', '该操作将扣除相应余额，不可恢复，是否提交归档？', function (event) {
        if (event) {
            opt.save(2);
        }
    })
}

//更新发放表信息
opt.update = function () {
    var paycount = $('#paylab').html();
    if (paycount <= 0) {
        $.messager.alert('提示', '发放金额应大于0');
        return;
    }
    $('#dg').datagrid('loading');
    $('#dg').datagrid('endEdit', editIndex);
    editIndex = undefined;
    var tasks = $("#dg").datagrid("getRows");
    var payId = $("#hideview").val();
    var parms = {
        list: tasks,
        id: payId
    };
    $.ajax({
        url: '../Pay/UpdatePayDetail',
        type: "post",
        contentType: "application/json",
        dataType: "json",
        data: JSON.stringify(parms),
        success: function (data) {
            $.messager.alert('提示', data);
            $('#dg').datagrid('loaded');
        },
        error: function () {
            $.messager.alert('提示', '保存失败');
            $('#dg').datagrid('loaded');
        }
    });
}
