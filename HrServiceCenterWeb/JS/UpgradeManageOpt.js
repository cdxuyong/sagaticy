var opt = window.NameSpace || {};

function init() {
}


opt.import = function () {
    $('#winUpload').upload({
        title: '请选择指定脚本文件',
        multiple: false,
        params: {},
        ext: 'ups',
        url: '../System/UpgradeFile',
        onAfterUpload: function (result) {
            if (result.success === true) {
                debugger
                $('#winUpload').upload('hide');
                $('#myContent')[0].innerText = result.data;
            }
            else {
                //alert(result.data);
                $.messager.alert('警告', result.data);
            }
        }
    });
    $('#winUpload').upload('show');
};

