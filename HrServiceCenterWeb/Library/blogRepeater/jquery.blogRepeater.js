/**
 * 
 * easyui.blogRepeater files
 * 支持将博客、微博或论坛等SNS互动消息以交互式的阅读方式展现，需要EASYUI JS支持
 * history:
 * v 1.0.0.0    2018/3  publish by xuyong
 */
(function ($) {
    function setSize(target, param) {
        var opts = $.data(target, 'blogRepeater').options;
        var t = $(target);
    }

    function init(target) {
        var opts = $.data(target, 'blogRepeater').options;
        $(target).addClass('blogRepeater').html(
		);

        // init paramaters

        // init buttons

        // init dialog
    }

    // 加载数据
    function loadData(opts) {
        var url = opts.url;
        // file check
        if (opts.ext != '') {
            var error = '';
            var exts = opts.ext.split(',');

            $('#blogRepeater').find('input[type=file]').each(function (i, file) {

            });

            if (error != '') {
                alert(error);
                return false;
            }
        }
    }

    // reapter's div绑定事件
    function bindEvents(target) {
        var opts = $.data(target, 'blogRepeater').options;
        function toTarget(t) {
        }
    }

    /**
	 * ajax 异步分页加载.
	 */
    function page(target) {
        var opts = $.data(target, 'blogRepeater').options;
        var params = { page: opts.pageIndex, rows: opts.pageSize, sessionId: Math.random().toString() }
        $.ajax({
            url: opts.url,
            type: "POST",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(params),
            success: function (data) {
                opts.pageIndex++;
                bindData(data,opts);
            },
            error: function () {
                UP.Loader.hide();
            }
        });

    }
    function bindData(data,opts) {
        var rows = data.rows;
        var rowTemplate =
        '<div class="row blogRepeater-row" dataId="@dataId">' +
            '<div class="blogRepeater-row-header">' +
                '<div style="width:64px;float:left;"><div class="round"> <img src="@header" /></div> </div>' +
                '<div style="float:left;">' +
                    '<div class="user">@user</div>' +
                    '<div>@date</div>' +
                '</div>' +
            '</div>' +
            '<div class="blogRepeater-row-title">' +
                '<div>@content </div>' +
            '</div>' +
            '<div class="blogRepeater-row-tool">' +
                '<div>' +
                '<span>回复@count次 &nbsp;&nbsp; 赞@votes个 &nbsp;&nbsp;吐槽@againsts个 </span>' +
                '<a href="#"  onclick="blogRepeater$votes$click(this)" class="vote" title="点赞" >&nbsp;</a> ' +
                '<a href="#" onclick="blogRepeater$against$click(this)" class="against" title="吐槽" ></a> ' +
                '<a href="#" onclick="blogRepeater$replay$show(this)" class="replay" title="评论" ></a> ' +
                '<a href="#" onclick="blogRepeater$delete$click(this)" class="delete" title="删除" ></a> ' +
                '</div>' +
            '</div>' +
            '<div class="blogRepeater-row-replay">' +
				'@replays' +
            '</div>' +
            '<div class="blogRepeater-row-post" style="display:none;"> ' +
                '<div><input type="text" placeholder="评论" /></div>' +
                '<div style="text-align:right;padding-top:5px;"><input type="button" value="评论" onclick="blogRepeater$replay$click(this)" /></div>' +
            '</div>' +
        '</div>';

        var replayTemplate =
        '<div class="blogRepeater-row-replay-content">' +
            '<div style="width:64px;float:left;">' +
                '<div class="round"><img src="@header" /></div>' +
            '</div>' +
            '<div style="float:left;">' +
                '<div class="user">@user:@content</div>' +
                '<div>@date</div>' +
            '</div>' +
        '</div>';
        for (var i = 0; i < rows.length; i++) {
            var html = rowTemplate;
            var blog = rows[i];
            html = html.replace('@header', opts.header1Url);
            html = html.replace('@dataId', blog.dataId);
            html = html.replace('@count', blog.count);
            html = html.replace('@votes', blog.votes);
            html = html.replace('@againsts', blog.againsts);
            html = html.replace('@user', blog.user);
            html = html.replace('@date', blog.date);
            html = html.replace('@content', blog.content);

            var rowsHtml = '';
            for (var j = 0; j < blog.rows.length; j++) {
                var blogReplay = blog.rows[j];
                var rowHtml = replayTemplate;
                rowHtml = rowHtml.replace('@header', opts.header2Url);
                rowHtml = rowHtml.replace('@user', blogReplay.user);
                rowHtml = rowHtml.replace('@date', blogReplay.date);
                rowHtml = rowHtml.replace('@content', blogReplay.content);
                rowsHtml += rowHtml;
            }
            html = html.replace('@replays', rowsHtml);
            var row = $(html);
            if (!blog.enable) {
                row.find('.delete').hide();
            }
            $('.blogRepeater').append(row);
        }
        // 获取滚动条位置
        var position = $('.blogRepeater-more').offset();
        // 加载更多
        $('.blogRepeater-more').remove();
        var moreHtml =
        '<div class="row blogRepeater-more">' +
            '<a href="#" onclick="$(\'.blogRepeater\').blogRepeater(\'page\')" title="点击加载加载更多精彩">加载更多</a>' +
        '</div>';
        var moreElement = $(moreHtml);
        $('.blogRepeater').append(moreElement);
        // 滚动条定位
        if (position != null) {
            window.scrollTo(0, position.top-200);
        }
    }
    /** 
     * 获取滚动条距离顶端的距离 
     * @return {}支持IE6 
     */
    function getScrollTop() {
        var scrollPos;
        if (window.pageYOffset) {
            scrollPos = window.pageYOffset;
        }
        else if (document.compatMode && document.compatMode != 'BackCompat')
        { scrollPos = document.documentElement.scrollTop; }
        else if (document.body) { scrollPos = document.body.scrollTop; }
        return scrollPos;
    }
    function publish(target) {
        var opts = $.data(target, 'blogRepeater').options;
        if (opts.onPublish != null) {
            opts.onPublish('abj');
        }
    }
    function replay(target,param) {
        var opts = $.data(target, 'blogRepeater').options;
        if (opts.onReplay != null) {
            opts.onReplay(param);
        }
    }
    function votes(target, param) {
        var opts = $.data(target, 'blogRepeater').options;
        if (opts.onVotes != null) {
            opts.onVotes(param);
        }
    }
    function against(target, param) {
        var opts = $.data(target, 'blogRepeater').options;
        if (opts.onAgainst != null) {
            opts.onAgainst(param);
        }
    }
    function deleteData(target, param) {
        var opts = $.data(target, 'blogRepeater').options;
        if (opts.onAgainst != null) {
            opts.onDelete(param);
        }
    }
    $.fn.blogRepeater = function (options, param) {
        if (typeof options == 'string') {
            return $.fn.blogRepeater.methods[options](this, param);
        }

        options = options || {};
        return this.each(function (i, e) {
            var state = $.data(this, 'upload');
            if (state) {
                $.extend(state.options, options);
            } else {
                state = $.data(this, 'blogRepeater', {
                    options: $.extend({}, $.fn.blogRepeater.defaults, $.fn.blogRepeater.parseOptions(this), options)
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
    $.fn.blogRepeater.methods = {
        options: function (jq) {
            return $.data(jq[0], 'upload').options;
        },
        /*
         * AJAX 加载页
         */
        page: function (jq, param) {
            page(jq[0]);
        },
        /*
         * 发表说说
         */
        publish: function (jq, param) {
            publish(jq[0]);
        },
        /*
         * 回复消息
         */
        replay: function (jq, param) {
            replay(jq[0],param);
        },
        /*
         * 点赞
         */
        votes: function (jq, param) {
            votes(jq[0], param);
        },
        /*
         * 吐槽
         */
        against: function (jq, param) {
            against(jq[0], param);
        },
        /*
         * 删除
         */
        delete: function (jq, param) {
            deleteData(jq[0], param);
        }
    };

    $.fn.blogRepeater.parseOptions = function (target) {
        var t = $(target);
        var o = $.extend({}, $.parser.parseOptions(target, [
			{ version: '1.0.0.0' }
        ]));
        return o;
    };
    // 参数列表
    $.fn.blogRepeater.defaults = {
        width: 'auto',
        height: 'auto',
        header1Url: 'images/user_default.png',
        header2Url: 'images/user_other.png',
        pageSize: 10,
        pageIndex:1,
        url: '',
        params: {},
        onReplay: function (text) {  alert('onReplay') },
        onPublish: function (text) { alert('onPublish')},
        onAfterLoad: function (result) { alert('onAfterLoad') }
    };
})(jQuery);

// 显示评论
function blogRepeater$replay$show(target) {
    debugger
    var row = $(target).parents('.blogRepeater-row');
    row.find('.blogRepeater-row-post').show();
}
// 回复
function blogRepeater$replay$click(target) {
    debugger
    var row = $(target).parents('.blogRepeater-row');
    var dataId = row.attr('dataId');
    var text = row.find('input[type=text]').val();
    if (text == '') return;
    $('.blogRepeater').blogRepeater('replay', {dataId:dataId,text:text});
}
// 点赞
function blogRepeater$votes$click(target) {
    var row = $(target).parents('.blogRepeater-row');
    var dataId = row.attr('dataId');
    $('.blogRepeater').blogRepeater('votes', { dataId: dataId});
}
// 吐槽
function blogRepeater$against$click(target) {
    var row = $(target).parents('.blogRepeater-row');
    var dataId = row.attr('dataId');
    $('.blogRepeater').blogRepeater('against', { dataId: dataId });
}
// 删除
function blogRepeater$delete$click(target) {
    var row = $(target).parents('.blogRepeater-row');
    var dataId = row.attr('dataId');
    $('.blogRepeater').blogRepeater('delete', { dataId: dataId });
}

