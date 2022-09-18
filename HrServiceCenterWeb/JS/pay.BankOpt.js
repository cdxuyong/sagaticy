var opt = window.NameSpace || {};

opt.export = function () {
    var payMonth = $('#datebox').datebox('getValue');
    var url = "../Payment/ExportBank";
    if (payMonth == '') return;
    var params = { payMonth: payMonth};

    HR.DownFile(url, params);
}

opt.export2 = function () {
    var payMonth = $('#datebox').datebox('getValue');
    var url = "../Payment/ExportPayDetail";
    if (payMonth == '') return;
    var params = { payMonth: payMonth };

    HR.DownFile(url, params);
}


$.fn.datebox.defaults.formatter = function (date) {
    var y = date.getFullYear();
    var m = date.getMonth() + 1;
    var d = date.getDate();
    return y + '-' + m + '-1';
}

