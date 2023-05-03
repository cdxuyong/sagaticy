var opt = window.NameSpace || {};

function init() {
    var height = $('.container-layout').height() - 160;
    $('#dgContainer').height(height);
    $('#dg').datagrid('resize');
    $('#dg').datagrid('enableFilter');
    opt.query();
}


//查询列表
opt.query = function () {
    var url = '../Pay/ImportorPayDetailTable?importorId=' + importorId;
    $('#dg').datagrid('loading');
    $.ajax({
        url: url,
        type: "GET",
        dataType: "json",
        data: {},
        success: function (data) {
            var total1 = 0, total2 = 0, total3 = 0, total4 = 0, total5 = 0;
            data.forEach(function (x) {
                total1 += x.f101;
                total2 += x.f102;
                total3 += x.f103;
                total4 += x.f104;
                total5 += x.f105;
            });
            var ds = {
                total: data.length,
                rows: data,
                footer: [
                    { CARD_ID: '合计', f101: total1.toFixed(2), f102: total2.toFixed(2), f103: total3.toFixed(2), f104: total4.toFixed(2), f207: total5.toFixed(2) },

                ]
            };

            $('#dg').datagrid('loadData', ds);
            $('#dg').datagrid('loaded');
        },
        error: function () {
            $('#dg').datagrid('loaded');
            $.messager.alert('提示', '查询出错！');
        }
    });
}

// 导入缴存数据或工资
opt.export = function () {
    var importId = importorId;
    var url = "../Pay/ExportInsuranceDetail?importId=" + importId;
    var params = {};
    HR.DownFile(url, params);
}
