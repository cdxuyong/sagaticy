
var opt = window.NameSpace || {};

function init() {
    /*
    $.ajax({
        url: "../Counter/GetEmployeeCount",
        type: "GET",
        dataType: "json",
        success: function (data) {
            opt.loadCompanyCounterChart(data);
        },
        error: function () {
            $.messager.alert('提示', '查询出错！');
        }
    });
    */
    $.ajax({
        url: "../Counter/GetPositionCounts",
        type: "GET",
        dataType: "json",
        success: function (data) {
            opt.loadPositionCounter(data);
        },
        error: function () {
            $.messager.alert('提示', '查询出错！');
        }
    });

    $.ajax({
        url: "../Counter/GetDegreeCounts",
        type: "GET",
        dataType: "json",
        success: function (data) {
            opt.loadDegreePositionCounter(data);
        },
        error: function () {
            $.messager.alert('提示', '查询出错！');
        }
    });

    $.ajax({
        url: "../Counter/GetPayCounts",
        type: "GET",
        dataType: "json",
        success: function (data) {
            if (data.success)
                opt.loadPayChart(data.series);
            else
                return;
        },
        error: function () {
            $.messager.alert('提示', '加载发放信息出错！');
        }
    });

    $.ajax({
        url: "../Employee/GetContractIsEndEmployeeList",
        type: "GET",
        dataType: "json",
        success: function (source) {
            new Vue({
                el: '#app',
                data: {
                    persons: source
                }
            });
        },
        error: function () {
            $.messager.alert('提示', '加载人员信息出错！');
        }
    });

    $.ajax({
        url: "../Employee/GetRetireIsEndEmployeeList",
        type: "GET",
        dataType: "json",
        success: function (source) {
            new Vue({
                el: '#app2',
                data: {
                    persons: source
                }
            })
        },
        error: function () {
            $.messager.alert('提示', '加载人员信息出错！');
        }
    });
    //loadEmployeePositionCounter();
    //loadDegreePositionCounter();
    //loadPayChart();
}

function getMax(list) {
    var max = 0;
    for (var i = 0; i < list.length; i++) {
        if (max < list[i])
            max = list[i];
    }
    return max;
}

opt.loadCompanyCounterChart = function (dataSource) {

    var myChart = echarts.init(document.getElementById('employeeCounter'), 'infographic');

    var dataAxis = dataSource.DataAxis;
    var data = dataSource.Data;
    var yMax = getMax(data) + 1;
    var dataShadow = [];

    for (var i = 0; i < data.length; i++) {
        dataShadow.push(yMax);
    }

    option = {
        title: {
            text: '事业单位人员统计表',
            left: 'center'
        },
        xAxis: {
            data: dataAxis,
            axisLabel: {
                interval: 0,
                rotate: "90",
                inside: true,
                textStyle: {
                    color: '#fff'
                }
            },
            axisTick: {
                show: false
            },
            axisLine: {
                show: false
            },
            z: 10
        },
        yAxis: {
            axisLine: {
                show: false
            },
            axisTick: {
                show: false
            },
            axisLabel: {
                textStyle: {
                    color: '#999'
                }
            }
        },
        dataZoom: [
            {
                type: 'inside'
            }
        ],
        series: [
            { // For shadow
                type: 'bar',
                itemStyle: {
                    normal: { color: 'rgba(0,0,0,0.05)' }
                },
                barGap: '-100%',
                barCategoryGap: '40%',
                data: dataShadow,
                animation: false
            },
            {
                type: 'bar',
                itemStyle: {
                    normal: {
                        color: new echarts.graphic.LinearGradient(
                            0, 0, 0, 1,
                            [
                                { offset: 0, color: '#83bff6' },
                                { offset: 0.5, color: '#188df0' },
                                { offset: 1, color: '#188df0' }
                            ]
                        )
                    },
                    emphasis: {
                        color: new echarts.graphic.LinearGradient(
                            0, 0, 0, 1,
                            [
                                { offset: 0, color: '#2378f7' },
                                { offset: 0.7, color: '#2378f7' },
                                { offset: 1, color: '#83bff6' }
                            ]
                        )
                    }
                },
                data: data
            }
        ]
    };

    myChart.setOption(option);
    // Enable data zoom when user click bar.
    var zoomSize = 6;
    myChart.on('click', function (params) {
        console.log(dataAxis[Math.max(params.dataIndex - zoomSize / 2, 0)]);
        myChart.dispatchAction({
            type: 'dataZoom',
            startValue: dataAxis[Math.max(params.dataIndex - zoomSize / 2, 0)],
            endValue: dataAxis[Math.min(params.dataIndex + zoomSize / 2, data.length - 1)]
        });
    });

}

opt.loadPositionCounter = function (dataSource) {
    option = {
        title: {
            text: '岗位分布',
            x: 'center'
        },
        tooltip: {
            trigger: 'item',
            formatter: "{a} <br/>{b} : {c} ({d}%)"
        },

        series: [
            {
                name: '岗位分布',
                type: 'pie',
                radius: '50%',
                data: dataSource,
                itemStyle: {
                    emphasis: {
                        shadowBlur: 10,
                        shadowOffsetX: 0,
                        shadowColor: 'rgba(0, 0, 0, 0.5)'
                    }
                }
            }
        ]
    };

    var myChart = echarts.init(document.getElementById('employeePositionCounter'), 'infographic');
    myChart.setOption(option);

}

opt.loadDegreePositionCounter = function (dataSource) {
    option = {
        title: {
            text: '学历分布',
            x: 'center'
        },
        tooltip: {
            trigger: 'item',
            formatter: "{a} <br/>{b} : {c} ({d}%)"
        },

        series: [
            {
                name: '岗位分布',
                type: 'pie',
                radius: '50%',
                data: dataSource,
                itemStyle: {
                    emphasis: {
                        shadowBlur: 10,
                        shadowOffsetX: 0,
                        shadowColor: 'rgba(0, 0, 0, 0.5)'
                    }
                }
            }
        ]
    };

    var myChart = echarts.init(document.getElementById('employeeDegreeCounter'), 'infographic');
    myChart.setOption(option);

}

opt.loadPayChart = function (series) {
    
    datetime = new Date();
    nowYear = datetime.getFullYear();
    option = {
        title: {
            text: nowYear + '年度发放趋势',
            left: 'center'
        },
        tooltip: {
            trigger: 'axis'
        },
        legend: {
            data: [series[0].Title, series[1].Title, series[2].Title, series[3].Title, series[4].Title],
            y: 'bottom',
        },
        xAxis: {
            type: 'category',
            data: series[0].DataAxis
        },
        yAxis: {
            type: 'value'
        },
        series: [{
            data: series[0].Data,
            name: series[0].Title,
            type: 'line'
        },
        {
            data: series[1].Data,
            name: series[1].Title,
            type: 'line'
        }, {
            data: series[2].Data,
            name: series[2].Title,
            type: 'line'
            }
            , {
                data: series[3].Data,
                name: series[3].Title,
                type: 'line'
            }
            , {
                data: series[4].Data,
                name: series[4].Title,
                type: 'line'
            }
        ]
    };


    var myChart = echarts.init(document.getElementById('payChart'), 'infographic');
    myChart.setOption(option);

}
