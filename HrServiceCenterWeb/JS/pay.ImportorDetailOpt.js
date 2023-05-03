var opt = window.NameSpace || {};

function init() {
    var height = $('.container-layout').height() - 160;
    $('#dgContainer').height(height);
    $('#dg').datagrid('resize');
    $('#dg').datagrid('enableFilter');
    var importId = $("#hideview").val();
    opt.query(importId);
}


//查询列表
opt.query = function (importId) {
    var url = '../Pay/QueryInsuranceDetail';

    $('#dg').datagrid('loading');
    $.ajax({
        url: url,
        type: "POST",
        dataType: "json",
        data: {
            importId: importId
        },
        success: function (data) {
            var total1 = 0;
            var total2 = 0;
            var gs = 0, yliao = 0, ylao = 0, syu = 0, sye = 0, gjj = 0;
            var gs2 = 0, yliao2 = 0, ylao2 = 0, syu2 = 0, sye2 = 0, gjj2 = 0;
            data.forEach(function (x) {
                total1 += x.PersonPayValue;
                total2 += x.CompanyPayValue;
                if (x.ItemId == 201) {
                    ylao += x.PersonPayValue;
                    ylao2 += x.CompanyPayValue;
                }
                if (x.ItemId == 202) {
                    sye += x.PersonPayValue;
                    sye2 += x.CompanyPayValue;
                }
                if (x.ItemId == 203) {
                    yliao += x.PersonPayValue;
                    yliao2 += x.CompanyPayValue;
                }
                if (x.ItemId == 204) {
                    gs += x.PersonPayValue;
                    gs2 += x.CompanyPayValue;
                }
                if (x.ItemId == 205) {
                    sye += x.PersonPayValue;
                    sye2 += x.CompanyPayValue;
                }
                if (x.ItemId == 206) {
                    gjj += x.PersonPayValue;
                    gjj2 += x.CompanyPayValue;
                }
            });
            var ds = {
                total:data.length,
                rows: data,
                footer: [
                    { CardId: '合计', PersonPayValue: total1.toFixed(2), CompanyPayValue: total2.toFixed(2) },
                    { CardId: '养老保险', PersonPayValue: ylao.toFixed(2), CompanyPayValue: ylao2.toFixed(2) },
                    { CardId: '医疗保险', PersonPayValue: yliao.toFixed(2), CompanyPayValue: yliao2.toFixed(2) },
                    { CardId: '失业保险', PersonPayValue: sye.toFixed(2), CompanyPayValue: sye2.toFixed(2) },
                    { CardId: '工伤保险', PersonPayValue: gs.toFixed(2), CompanyPayValue: gs2.toFixed(2) },
                    { CardId: '生育保险', PersonPayValue: syu.toFixed(2), CompanyPayValue: syu2.toFixed(2) },
                    { CardId: '住房公积金', PersonPayValue: gjj.toFixed(2), CompanyPayValue: gjj2.toFixed(2) }
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

opt.export = function(){
    var importId = $("#hideview").val();
    var url = "../Pay/ExportInsuranceDetail?importId=" + importId;
    var params = {};
    HR.DownFile(url, params);
}