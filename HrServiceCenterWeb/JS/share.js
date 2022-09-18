/*
 * share.js
 * date：2016/03/29
*/
window.HR = {
    __namespace: true,
    __typeName: "HR",
    __ispostback:false,
    __return: null,
    lastQueryPageURL:"",
    getVersion: function () { return "1.0.0.0"; },
    getVirtualPath:function(){
        var virPath = self.location.pathname;
        var end = virPath.indexOf('/',1);
        virPath = virPath.substr(0,end);
        return virPath;
    },
    getTop : function(){
    	var parent = self;
	    var frmPath = "";
	    var flag = true;
	    while(flag)
	    {
		    if(parent.parent==parent)
			    flag = false;
		    else
		    {
			    parent = parent.parent;
	        }
	    }
	    return parent;
    },
    getLastQueryPage: function () {
        var top = this.getTop();
        return top.HR.lastQueryPageURL;
    },
    dispose:function(){
    },
    http:''
};
/* 
 * ultrapower datetime
 */
HR.Date = function HR$Date(datetime) {
    if (datetime == null) {
        datetime = new Date();
    }
    else {
        if (typeof (datetime) == 'string') {
            var dateString = datetime.replace("/Date(", "").replace(")/", "");
            datetime = new Date(parseInt(dateString));
        }
    }
    this.addMonths = function(months){
    }
    this.addDay = function(days){
    }
    this.format = function (fmt) { 
        var o = {
            "M+": datetime.getMonth() + 1, //月份 
            "d+": datetime.getDate(), //日 
            "h+": datetime.getHours(), //小时 
            "m+": datetime.getMinutes(), //分 
            "s+": datetime.getSeconds(), //秒 
            "q+": Math.floor((datetime.getMonth() + 3) / 3), //季度 
            "S": datetime.getMilliseconds() //毫秒 
        };
        if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (datetime.getFullYear() + "").substr(4 - RegExp.$1.length));
        for (var k in o)
            if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        return fmt;
    }
}
/* 
 ultrapower hashmap
 */
HR.HashMap = function HR$HashMap(){
    var size = 0;
    var dics = new Object();
    this.push = function(key, value) {
        if (!this.containsKey(key)) {
            size++;
            dics[key] = value;
        }
    }
    this.get = function(key) {
        return this.containsKey(key) ? dics[key] : null;
    }
    this.remove = function(key) {
        if (this.containsKey(key) && (delete dics[key])) {
            size--;
        }
    }
    this.containsKey = function(key) {
        return (key in dics);
    }
    this.containsValue = function(value) {
        for (var prop in dics) {
            if (dics[prop] == value) {
                return true;
            }
        }
        return false;
    }
    this.values = function() {
        var values = new Array();
        for (var prop in dics) {
            values.push(dics[prop]);
        }
        return values;
    }
    this.keys = function() {
        var keys = new Array();
        for (var prop in dics) {
            keys.push(prop);
        }
        return keys;
    }
    this.size = function() {
        return size;
    }
    this.clear = function() {
        size = 0;
        dics = new Object();
    }
}
HR.Dialog = function HR$Dialog(){
    ///<summary>对话框</summary>
}
HR.Dialog.show = function HR$Dialog$show(url,afterEvent,style){
    // add code
};
HR.Dialog.close = function (){
    // add code
}
HR.Dialog.finish = function (ds){
    // add code

}
HR.Dialog.callMethod = function (ds){
    // add code
}
HR.Form = function HR$Form() {
}
HR.Form.getValues = function (elementid) {
    var obj = {};
    var form = $('#'+elementid);
    var fields = form.find('[form-field-name]');
    for(var i=0;i<fields.length;i++){
        var field = fields[i];
        var ele = $(field);
        //if( ele.attr('form-field-name') == null ) continue;
        var ctlType = ele.attr('form-control-type');
        var fieldName = ele.attr('form-field-name');
        var value = null;
        switch(ctlType){
            case 'combobox': //multiple
                var multiple = ele.combobox('options').multiple;
                if(!multiple)
                    value = ele.combobox('getValue');
                else{
                    value = '';
                    var datas = ele.combobox('getValues');
                    for(var n=0;n<datas.length;n++)
                        value += (n==0) ? datas[0] : ','+datas[n];
                }
                break;
            case 'textbox':
                value = ele.textbox('getValue');
                break;
            case 'numberbox':
                value = ele.textbox('getValue');
                break;
            case 'datebox':
                value = ele.datebox('getValue');
                break;
            default:
                value = ele.val();
                break;
        }
        eval('obj.'+fieldName+'=value');
        //$.extend({}, obj, { cmd: 'down'});
    }
    return obj;
}
HR.Form.setValues = function (elementid, o) {
    
    var form = $('#'+elementid);
    var fields = form.find('[form-field-name]');
    for(var i=0;i<fields.length;i++){
        var field = fields[i];
        var ele = $(field);
        var ctlType = ele.attr('form-control-type');
        var fieldName = ele.attr('form-field-name');
        var value = eval('o.' + fieldName);
        if (value == null) continue;
        switch (ctlType) {
            case 'combobox':
                ele.combobox('setValue',value);
                break;
            case 'textbox':
                ele.textbox('setValue',value);
                break;
            case 'numberbox':
                ele.numberbox('setValue', value);
                break;
            case 'datebox':
                ele.datebox('setValue',value);
                break;
            case 'label':
                ele.text(value);
                break;
            default:
                ele.val(value);
                break;
        }
    }
}

/*
 * bindCombox：绑定数据源到下拉控件
 * @elementid 元素ID
 * @url 请求JSON地址
 * @callback 回调方法
 * @ansync 是否异步调用,如果ansync为异步则可以不用定义callback
 */
HR.Form.bindCombox = function (elementid, url, callback, ansync) {
    if (ansync == null) ansync = true;
    $.ajax({
        url: url,
        data: null,
        async: ansync,
        type: "POST",
        dataType: "json",
        success: function (items) {
            var combox = $('#' + elementid);
            combox.combobox('setValue', null);
            combox.combobox('loadData', items);
            if( callback != null && callback != '' )
                callback(combox,items);
        },
        error: function (){
            alert('下拉控件数据加载失败，请重新刷新页面！');
        }
    });
}


HR.Loader = function HR$Loader(){
}
HR.Loader.time = 0;
HR.Loader.show = function HR$Loader$Show(text){
    var ele = $('#up_loading_top');
    if (ele.length == 0) {
        var width = 160;
        if (text != null) {
            width = text.length * 16;
        }
        // div top
        var style = 'position:absolute;padding: 10px 10px 10px 20px;border: 3px solid #CCCCCC; z-index: 100000; background-color: #FFFFFF;';
        var img = '../images/loading.gif';
        var html = '<div id="up_loading_top" style="@style"><div style="float:left;width:30px;"><img src="@img" /></div> <div style="float:left;width:'+width+'px;">@text</div> </div> ';
        if(text==null||text=='')  text='加载中，请稍候...';
        html = html.replace('@style',style);
        html = html.replace('@img',img);
        html = html.replace('@text',text);
        var ele = $(html);
        var windowWidth = document.documentElement.clientWidth;   
        var windowHeight = document.documentElement.clientHeight;   
        var popupHeight = ele.height();   
        var popupWidth = ele.width(); 
        ele.css({   
            "position": "absolute",   
            "top": (windowHeight-popupHeight)/2+$(document).scrollTop(),   // (700-popupHeight)/2, //
            "left": (windowWidth-popupWidth)/2
         });
        $('body').append(ele);
        // div background
        var bgHtml = '<div id="up_loading_bg" style="top:0px;left:0px;position:absolute;z-index: 99999;width:100%;background-color:#CCCCCC;opacity: 0.2;"></div>';
        var bgEle = $(bgHtml);
        $('body').append(bgEle);
        bgEle.css("height", windowHeight + $(document).scrollTop());
        bgEle.show();
    }
    else{
    }
    HR.Loader.time++;
    ele.show();
}
HR.Loader.hide = function HR$Loader$Hide(){
    if(HR.Loader.time>0)
        HR.Loader.time--;
    if( HR.Loader.time == 0 ){
        $('#up_loading_bg').hide();
        $('#up_loading_top').hide();
    }  
}
HR.DownFile = function HR$DownFile(url,params){
    var downloadHelper = $('<iframe style="display:none;" id="downloadHelper"></iframe>').appendTo('body')[0];
    var doc = downloadHelper.contentWindow.document;
    if (doc) {
        doc.open();
        doc.write('')//微软为doc.clear()有时会出bug
        doc.writeln("<html><body><form id='downloadForm' name='downloadForm' method='post' action='" + url + "'>");
        if( params != null )
        for (var key in params){
            doc.writeln("<input type='hidden' name='" + key + "' value='" +  params[key] + "'>");
        }
        doc.writeln('<\/form>')
        doc.writeln('<\/body><\/html>');
        doc.close();
        var form = doc.forms[0];
        if (form) {
            HR.Loader.show();
            form.submit();
            var icount = setInterval(function(){
                try{
                    if(doc.readyState != 'loading'){  
                        HR.Loader.hide();
                        clearTimeout(icount);
                    }
                }
                catch(err){
                    HR.Loader.hide();
                    clearTimeout(icount);
                    alert("导出过程中发生未知错误，导出失败！");
                }
            }, 5000);
            
        }
    }
}
/* echart method */
HR.EChart = function HR$EChart() {
}
// load echart
HR.EChart.load = function HR$EChart$load(elementId,option) {
    var chart = echarts.init(document.getElementById(elementId));
    chart.setOption(option);
}
/* ajax queue method  */
HR.Ajax = function HR$Ajax() {
}
HR.Ajax.queue = new Array();
HR.Ajax.index = -1;
HR.Ajax.clear = function HR$Ajax$clear(){
    HR.Ajax.queue = new Array();
    HR.Ajax.index = 0
}
HR.Ajax.add = function HR$Ajax$add(url,params,callback) {
    var thread = {
        url:url,
        params:params,
        callback:callback
    };
    HR.Ajax.queue[HR.Ajax.Queue.length] = thread;
}
// default/onebyone/timeout
HR.Ajax.load = function HR$Ajax$load(type) {
    var queue = HR.Ajax.queue;
    if( type=='onebyone' ){
        HR.Ajax.load_onebyone(queue[0]);
        return;
    }
    if( type=='timeout' ){
        for(var i=0;i<queue.length;i++){
            setTimeout(load_timeout,i*5000);
        }
        return;
    }
    if( typeof(type) == 'undefined' || type == null ){
        for(var i=0;i<queue.length;i++){
            var thread = queue[i];
            HR.Ajax.load_default(thread);
        }
        return;
    }
}
HR.Ajax.load_default = function HR$Ajax$load_default(thread){
    $.ajax({
        url: thread.url,
        type: "POST",
        dataType: "json",
        data: thread.params,
        success: function (data){
            eval(thread.callback + '(data)');
            HR.Ajax.index++;
            if( HR.Ajax.index == HR.Ajax.queue.length ){
                // hiden loading
            }
        },
        error: function (){
                // hiden loading
        }
    });
}
HR.Ajax.load_timeout = function HR$Ajax$load_timeout(){

}
HR.Ajax.load_onebyone = function HR$Ajax$load_onebyone(thread){
    $.ajax({
        url: thread.url,
        type: "POST",
        dataType: "json",
        data: thread.params,
        success: function (data){
            eval(thread.callback + '(data)');
            HR.Ajax.index++;
            if( HR.Ajax.index == HR.Ajax.queue.length ){
                // hiden loading
                HR.Ajax.queue = new Array();
            }
            else{
                HR.Ajax.load_onebyone(HR.Ajax.queue[HR.Ajax.index]);
            }
        },
        error: function (){
                // hiden loading
        }
    });
}
/* easyui  pages on client */
function pagerFilter(data){
	if (typeof data.length == 'number' && typeof data.splice == 'function'){	// is array
		data = {
			total: data.length,
			rows: data
		}
	}
	var dg = $(this);
	var opts = dg.datagrid('options');
	var pager = dg.datagrid('getPager');
	pager.pagination({
		onSelectPage:function(pageNum, pageSize){
			opts.pageNumber = pageNum;
			opts.pageSize = pageSize;
			pager.pagination('refresh',{
				pageNumber:pageNum,
				pageSize:pageSize
			});
			dg.datagrid('loadData',data);
		}
	});
	if (!data.originalRows){
		data.originalRows = (data.rows);
	}
	var start = (opts.pageNumber-1)*parseInt(opts.pageSize);
	var end = start + parseInt(opts.pageSize);
	data.rows = (data.originalRows.slice(start, end));
	return data;
}

/**
* EasyUI DataGrid根据字段动态合并单元格
* @param fldList 要合并table的id
* @param fldList 要合并的列,用逗号分隔(例如："name,department,office");
*/
function MergeCells(tableID, fldList) {
    var Arr = fldList.split(",");
    var dg = $('#' + tableID);
    var fldName;
    var RowCount = dg.datagrid("getRows").length;
    var span;
    var PerValue = "";
    var CurValue = "";
    var length = Arr.length - 1;
    for (i = length; i >= 0; i--) {
        fldName = Arr[i];
        PerValue = "";
        span = 1;
        for (row = 0; row <= RowCount; row++) {
            if (row == RowCount) {
                CurValue = "";
            }
            else {
                CurValue = dg.datagrid("getRows")[row][fldName];
            }
            if (PerValue == CurValue) {
                span += 1;
            }
            else {
                var index = row - span;
                dg.datagrid('mergeCells', {
                    index: index,
                    field: fldName,
                    rowspan: span,
                    colspan: null
                });
                span = 1;
                PerValue = CurValue;
            }
        }
    }
}