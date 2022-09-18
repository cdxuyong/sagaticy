/**
 * jQuery EasyUI 1.4.5
 * upload files
 * history:
 * v 1.0.0.0    2016/5  publish
 * v 1.0.17.01  2017/11 修复文件后缀大小写问题
 * v 1.0.17.02  2018/1/26 优化窗口弹出效果
 */
(function ($) {
    function setSize(target, param) {
        var opts = $.data(target, 'upload').options;
        var t = $(target);
    }

    function init(target) {
        var opts = $.data(target, 'upload').options;
        $(target).addClass('upload').html(
            '<form id="upload_form" method="post" enctype="multipart/form-data" >' +
               '<div id="upload_files" class="upload-files"></div>' +
            '</form>' +
            '<div id="upload_buttons">' +
                '<label id="upload_msg" class="upload-msg" style="display:none">上传处理中...</label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' +
                '<a id="upload_add"  onclick="" style="display:none;">增加</a>' +
                '<a id="upload_ok"  onclick="">确定</a>' +
                '<a id="upload_cancel"  onclick="">取消</a>' +
            '</div>'
		);

        // init files
        addFilebox();
        // init paramaters
        for (var key in opts.params) {
            var hidden = '<input name="' + key + '" value="' + opts.params[key] + '" type="hidden" />';
            $('#upload_form').append(hidden);
        }
        // init buttons
        if (opts.multiple) {
            $('#upload_add').css('display', '');
        }
        $('#upload_add').linkbutton({
            iconCls: 'icon-add'
        });
        $('#upload_add').bind('click', function () {
            addFilebox();
        });
        $('#upload_ok').linkbutton({
            iconCls: 'icon-ok'
        });
        $('#upload_ok').bind('click', function () {
            uploadFiles(opts);
        });
        $('#upload_cancel').linkbutton({
            iconCls: 'icon-cancel'
        });
        $('#upload_cancel').bind('click', function () {
            $(target).dialog('close');
        });


        // init dialog
        $(target).dialog({
            title: opts.title,
            width: opts.width,
            height: opts.height,
            closed: true,
            modal: true,
            buttons: '#upload_buttons'
        });
        //$(target).dialog('open');
    }

    //重新加载控件的样式（主要重定位控件的位置）
    function reLoad(target) {
        var opts = $.data(target, 'upload').options;
        $(target).dialog({
            title: opts.title,
            width: opts.width,
            height: opts.height,
            position: 'absolute',
            left: '40%',
            top: $(document).scrollTop() + ($(window).height() - 200) / 2,
            closed: true,
            modal: true,
            buttons: '#upload_buttons'
        });
    }

    // 文件上传控件
    function uploadFiles(opts) {
        var url = opts.url;
        // file check
        if (opts.ext != '') {
            var error = '';
            var exts = opts.ext.split(',');

            $('#upload_files').find('input[type=file]').each(function (i, file) {
                var fileName = file.value;
                var extName = fileName.substring(fileName.lastIndexOf('.') + 1);
                var good = false;

                for (var j in exts) {

                    if (exts[j].toUpperCase() == extName.toUpperCase()) {
                        good = true;
                        break;
                    }
                }
                if (!good) {
                    error = '文件类型' + extName + '不允许，请上传' + opts.ext + '格式！';
                }
            });

            if (error != '') {
                alert(error);
                return false;
            }
        }

        // upload files
        $('#upload_msg').css('display', '');
        $('#upload_form').form('submit', {
            url: url,
            onSubmit: function () {
                return true;
            },
            success: function (result, status) {
                $('#upload_msg').css('display', 'none');
                try {
                    result = $.parseJSON(result);
                }
                catch (e) {
                }
                if (result instanceof Object) {
                    if (typeof result.success == 'boolean') {
                        // 默认情况返回JSON格式
                        if (result.success == true) {
                            $('#upload_files').html('');
                            addFilebox();
                            opts.onAfterUpload.call(this, result, result);
                        }
                        else {
                            if (result.data != '')
                                opts.onAfterUpload.call(this, result, result);
                            else alert('上传初始失败')
                        }
                    }
                    else {
                        // 非标准JSON格式
                        $('#upload_files').html('');
                        addFilebox();
                        opts.onAfterUpload.call(this, result, result);
                    }
                }
                else {
                    alert('上传文件出现异常，可能是网络或服务器设置问题！');
                }


            }
        });
    }

    // 多文件支持：增加文件上传控件
    function addFilebox() {
        var files = $('#upload_files');
        var count = files.data('count');
        if (typeof count != 'number')
            count = 1;
        else
            count = count + 1;
        files.data('count', count);
        var id = 'upload_file_' + count;
        var file = '<div class="upload-file"> <input class="easyui-filebox" data-options="prompt:\'Choose a file..\'" id="' + id + '" name="' + id + '"   style="width:90%;"> </div>';
        files.append(file);
        $('#' + id).filebox({
            buttonText: '选择文件',
            onChange: function (a, b, c) {
                // addFilebox();
            }
        });
    }

    // upload's div绑定事件
    function bindEvents(target) {
        var opts = $.data(target, 'upload').options;
        function toTarget(t) {
        }
    }

    /**
	 * show the upload day.
	 */
    function show(target) {
        reLoad(target);
        $(target).dialog('open');
    }
    function hide(target) {
        $(target).dialog('close');
    }
    function addElement(target, param) {
        $('#upload_form').append(param);
    }

    $.fn.upload = function (options, param) {
        if (typeof options == 'string') {
            return $.fn.upload.methods[options](this, param);
        }

        options = options || {};
        return this.each(function (i, e) {
            var state = $.data(this, 'upload');
            if (state) {
                $.extend(state.options, options);
            } else {
                state = $.data(this, 'upload', {
                    options: $.extend({}, $.fn.upload.defaults, $.fn.upload.parseOptions(this), options)
                });
                init(this);
            }
            if (state.options.border == false) {
                $(this).addClass('upload-noborder');
            }
            setSize(this);
            bindEvents(this);
            //show(this);
        });
    };
    // 公用方法
    $.fn.upload.methods = {
        options: function (jq) {
            return $.data(jq[0], 'upload').options;
        },
        /*
         * 显示上传窗口
         */
        show: function (jq, param) {
            show(jq[0]);
        },
        /*
         * 关闭上传窗口
         */
        hide: function (jq, param) {
            hide(jq[0]);
        },
        addElement: function (jq, param) {
            // 上传窗口增加自定义内容
            addElement(jq, param);
        }
    };

    $.fn.upload.parseOptions = function (target) {
        var t = $(target);
        return $.extend({}, $.parser.parseOptions(target, [
			{ version: '1.0.0.0' }
        ]));
    };
    // 参数列表
    $.fn.upload.defaults = {
        width: 400,
        height: 200,
        fit: false,
        border: true,
        title: '文件上传对话框',
        ext: '', //文件后缀限制，多个中间用,隔开，类似：rar,xls
        multiple: false,
        url: '',
        params: {},

        onSelect: function (file) { },
        onChange: function (file) { },
        // 上传完成后事件
        // result json格式{success:true,data:{} }
        onAfterUpload: function (result) { alert('upload') }
    };
})(jQuery);
