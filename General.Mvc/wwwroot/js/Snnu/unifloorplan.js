/*
 * UniFloorPlan v1.0.0
 * http://www.unifound.net
 * 日期: 2014/12/3
 * 作者: 何昆鹏
 */


(function ($, uni) {
    var defaults = {
        width: 600,
        height: 600,
        step: 10,
        isTitle: true,//显示名称
        allDay: false,
        interval: 60,
        img: "",
        selectTime: false
    }
    $.fn.uniFloorPlan = function (options) {
        //公共对象
        var fp = $(this);
        var opt = $.extend(true, {}, defaults, options);
        var iobjs;//坐标对象集合
        var iqz = $("<div class='fp-qzone' tabindex='0' onselectstart='return false'></div>");
        var header = $("<div class='fp-header'></div>");
        var editPanel = $("<div class='fp-edit fp-edit-panel' style='display:none;'></div>").appendTo(header);
        var userPanel = $("<div class='fp-user fp-user-panel'></div>").appendTo(header);
        var content = $("<div class='fp_content fp-show-guides' style='position:relative;'><div class='fp-guides'><table class='fp-guides-tbl'></table></div><img src='" + opt.img + "' style='width:100%;height:100%;'/></div>");
        var editCon = $("<div class='fp-edit fp-edit-con' style='display:none;'></div>").appendTo(content);
        var userCon = $("<div class='fp-user fp-user-con'></div>").appendTo(content);
        var guides = content.find(".fp-guides");

        iqz.html(header);
        iqz.append(content);
        //编辑公共对象
        var boxs = uni.getHash();//哈希对象存储编辑对象
        var isz;//尺寸
        var ibox;//当前对象
        var selObjs = $("<select class='fp-objs'></select>");//对象列表
        //状态公共对象
        var dots = [];//数组存储状态对象
        //初始化
        fp.html("<span class='uni_trans'>加载中...</span>");
        if (opt.evInit) {
            opt.evInit(function (rlt) {
                Init(rlt.data);
                InitEdit();
                InitUser();
                fp.html(iqz);
                if (opt.evFinish) opt.evFinish(fp);
            });
        }
        else
            msgbox("未定义初始化函数");
        //初始化过程
        function Init(data) {
            if (data.height) {
                opt.height = data.height;
                content.height(opt.height);
            }
            if (data.width) {
                opt.width = data.width;
                content.width(opt.width);
            }
            if (typeof (data.istitle) == "boolean") {
                opt.isTitle = data.istitle;
                if (!data.istitle)
                    content.addClass("fp-hide-obj-title")
            }
            iobjs = data.objs;
        }
        function InitEdit() {
            editPanel.html("");
            editCon.html("");
            //编辑面板
            var unit = $("<select class='fp-unit'>"
                + "<option value='14'>14</option>"
                + "<option value='16'>16</option>"
                + "<option value='20' selected>20</option>"
                + "<option value='24'>24</option>"
                + "<option value='28'>28</option>"
                + "</select>");
            editPanel.append(unit);
            //重置尺寸
            unit.change(function () {
                isz = parseInt($(this).val());
            });
            if (iobjs) {
                $.each(iobjs, function (i) {
                    selObjs.append("<option value='" + this.id + "'>" + (i + 1) + "." + this.name + "</option>");
                    if (this.top && this.left)
                        createBox(this.id, this.name, this.top, this.left, this.size);
                });
            }
            editPanel.append(selObjs);
            //列表切换
            selObjs.change(function () {
                var v = $(this).val();
                ibox = boxs.get(v);
                var bs = $(".fp-box", content).removeClass("fp-box-act");
                if (ibox) {
                    bs.removeClass("fp-box-sel");
                    ibox.addClass("fp-box-act fp-box-sel");
                }
            });
            //功能
            editPanel.append("<span>宽:</span><input type='text' class='fp-area fp-area-width' value='" + opt.width + "'/><span>高:</span><input type='text' class='fp-area fp-area-height' value='" + opt.height + "'/>");
            $("<input type='button' class='fp-change-area btn-sm btn-info btn' value='设置尺寸'/>").click(function () {
                var w = parseInt(editPanel.find(".fp-area-width").val());
                var h = parseInt(editPanel.find(".fp-area-height").val());
                if (!isNaN(w) && !isNaN(h)) {
                    content.css({ width: w, height: h });
                }
                drawGuides();
            }).appendTo(editPanel);
            //下一步
            editPanel.append("<label><input type='checkbox' class='fp-autonext' checked/>自动下一步&nbsp;</label>");
            //名称显示
            $("<label><input type='checkbox' class='fp-show-title' " + (opt.isTitle ? "checked" : "") + "/>显示名称&nbsp;</label>").click(function () {
                if ($(".fp-show-title", editPanel).is(":checked")) {
                    content.removeClass("fp-hide-obj-title");
                }
                else {
                    content.addClass("fp-hide-obj-title");
                }
            }).appendTo(editPanel);
            //显示网格
            $("<label><input type='checkbox' class='fp-show-guides' checked/>显示网格&nbsp;</label>").click(function () {
                if ($(".fp-show-guides", editPanel).is(":checked")) {
                    content.addClass("fp-show-guides");
                }
                else {
                    content.removeClass("fp-show-guides");
                }
            }).appendTo(editPanel);
            //操作
            var act = $("<div class='fp-edit-act'></div>").appendTo(editPanel);

            //批量对齐
            $("<a class='fp-clear'>批量选中</a>").click(function () {
                $("#fp_Select_Strat").val("");
                $("#fp_Select_End").val("");
                $(".ul_items").html("");
                $(".fp-box").removeClass("fp-box-sel")
                uni.dlg($("#fp_Select_Modal"), "选择批量选中", 460, 200);

            }).appendTo(act);
            $("#fp_Select_Btn").click(function () {

            })

            //删除
            $("<a class='fp-clear'>删除</a>").click(function () {
                var sels = boxs.values();
                $.each(sels, function () {
                    if (this.hasClass("fp-box-sel")) {
                        if (ibox && this.key == ibox.key) ibox = null;
                        boxs.remove(this.key);
                        this.remove();
                        selObjs.children().eq(this.index).removeClass("fp-obj-do");
                    }
                });
            }).appendTo(act);
            $("<a class='fp-clear'>全部删除</a>").click(function () {
                $(".fp-box", content).remove();
                boxs.clear();
                ibox = null;
                selObjs.children().removeClass("fp-obj-do");
                selObjs.find("option:first").attr("selected", true);
                selObjs.trigger("change");
            }).appendTo(act);
            act.append("| ");
            $("<a class='fp-hori-align'>水平对齐</a>").click(function () {
                var sels = content.find(".fp-box-sel");
                if (sels.length < 2) return;
                var _top = 0;
                sels.each(function () {
                    var t = parseInt($(this).css("top"));
                    if (t > _top) _top = t;
                });
                if (_top)
                    sels.css("top", _top);
            }).appendTo(act);
            $("<a class='fp-vert-align'>垂直对齐</a>").click(function () {
                var sels = content.find(".fp-box-sel");
                if (sels.length < 2) return;
                var _left = 0;
                sels.each(function () {
                    var l = parseInt($(this).css("left"));
                    if (l > _left) _left = l;
                });
                if (_left)
                    sels.css("left", _left);
            }).appendTo(act);
            act.append("| ");
            //保存
            $("<a>保存</a>").click(function () {
                SaveCoorb(function () {
                    uni.msgBox("保存成功");
                });
            }).appendTo(act);
            //转使用
            $("<a class='fp-cvt-mode'>转使用</a>").click(function () {
                uni.confirm("保存并返回使用端？", function () {
                    SaveCoorb(function () {
                        iqz.removeClass("fp-editing");
                        iqz.find(".fp-edit").hide();

                        userPanel.find(".fp-time-start").val(timeStep(new Date()));
                        userPanel.find(".fp-time-end").val(timeStep((new Date()).addMinutes(opt.interval || 0)));
                        userPanel.find(".fp-user-search").trigger("click");
                        iqz.find(".fp-user").show();
                    });
                });
            }).appendTo(act);
            //使用说明
            $("<a class='fp-help'>操作说明</a>").click(function () {
                var str = "操作说明：<br/>平面图放置在ClientWeb/upload/DevImg/floorplan/下,资源ID.jpg（资源通常为实验室/设备类别，取决于系统定义）<br/><br/>功能区域从左到右：<br/>1、圆点尺寸设置<br/>2、对象列表，选择/在平面图上点击布置<br/>3、平面图区域尺寸设置<br/>4、布置圆点自动下一个，回车自动选中下一个<br/>5、显示/隐藏对象名称<br/>6、显示/隐藏辅助网格<br/>7、删除选中的圆点<br/>8、删除全部圆点<br/>9、多选圆点水平对齐<br/>10、多选圆点垂直对齐<br/>11、保存设置<br/><br/>";
                str += "操作：<br/>1、点击选中，回车释放/下一个<br/>2、control+单击多选<br/>3、支持拖拽<br/>4、选中后，上下左右微调";
                uni.msgBox(str, "操作说明");
            }).appendTo(act);
            //键盘微调
            iqz.keydown(function (ev) {
                var sels = $(".fp-box-sel", content);
                if (sels.length > 0) {
                    var _x = 0;
                    var _y = 0;
                    var ori = true;
                    var code = ev.keyCode;
                    if (code == 37) {//left
                        _x--;
                    }
                    else if (code == 38) {//up
                        _y--;
                    }
                    else if (code == 39) {//right
                        _x++;
                    }
                    else if (code == 40) {//down
                        _y++;
                    }
                    else if (code == 13) {//回车
                        sels.removeClass("fp-box-sel");
                        if (sels.length == 1 && $(".fp-autonext", editPanel).is(":checked")) {
                            var i = boxs.get(selObjs.find("option:eq(" + (parseInt(sels.attr("index")) + 1) + ")").attr("value"));
                            if (i) i.addClass("fp-box-sel");
                        }
                        return false;
                    }
                    else {
                        ori = false;
                    }
                    if (ori) {
                        sels.each(function () {
                            var p = $(this);
                            p.css({ "top": parseInt(p.css("top")) + _y, "left": parseInt(p.css("left")) + _x });
                        });
                        return false;
                    }
                }
            });
            //
            unit.trigger("change");
            selObjs.trigger("change");
            //******************************主体***************************
            drawGuides();
            content.click(function (ev) {
                if (!ibox) {
                    var offset = $(this).offset();
                    var x = ev.pageX - offset.left;//ev.offsetX;
                    var y = ev.pageY - offset.top;//ev.offsetY;
                    var top = y - (isz / 2) + "px";
                    var left = x - (isz / 2) + "px";
                    createBox(selObjs.val(), $("option:selected", selObjs).text().substr(2), top, left, isz, $(".fp-autonext", editPanel).is(":checked"));
                }
                else {
                    content.find(".fp-box-sel").removeClass("fp-box-sel");
                }
            });
            //保存信息
            function SaveCoorb(suc) {
                if (opt.evSaveCoorb)
                    opt.evSaveCoorb({ width: content.width(), height: content.height(), istitle: $(".fp-show-title", editPanel).is(":checked") }, boxs, function () {
                        opt.evInit(function (rlt) {
                            Init(rlt.data);
                            if (suc) suc(rlt);
                        });
                    });
            }
        }
        function InitUser() {
            //清空编辑对象
            userPanel.html("");
            userCon.html("");
            var rsch = $("<select class='fp-rsch'><option value='0'>----" + uni.translate("查找") + "----</option></select>");
            //用户面板
            var str = "<span class='fp-date-panel'><span class='fp-date-text'>" + uni.translate("选择时间") + ":</span><input type='text' class='fp-dt-input fp-date' style='width:120px;'/>&nbsp;&nbsp;<span style='" + (opt.allDay ? "display:none;" : "") + "'><input type='text' class='fp-dt-input fp-time-start' style='width:50px;'/>" +
                "-<input type='text' class='fp-dt-input fp-time-end' style='width:50px;'/></span></span>";

            if (opt.selectTime) {
                var str = "<span class='fp-date-panel' style='display:none'><span class='fp-date-text'>" + uni.translate("选择时间") + ":</span><input type='text' class='fp-dt-input fp-date' style='width:120px;'/>&nbsp;&nbsp;<span style='" + (opt.allDay ? "display:none;" : "") + "'><input type='text' class='fp-dt-input fp-time-start' style='width:50px;'/>" +
                    "-<input type='text' class='fp-dt-input fp-time-end' style='width:50px;'/></span></span>";
            }
            userPanel.append(opt.mode === "status" ? "" : str);
            var date = $(".fp-date", userPanel);
            var t_start = $(".fp-time-start", userPanel);
            var t_end = $(".fp-time-end", userPanel);
            var search = $("<input type='button' class='fp-user-search btn btn-warning btn-sm' value='刷新' style='" + ((opt.mode === "status" || opt.allDay) ? "" : "display:none;") + "'/>").click(function () {
                if (opt.evUpTime) {
                    if (!opt.allDay) {
                        var now = new Date();
                        var st = uni.parseDate(date.val() + " " + t_start.val());
                        if (uni.compareDate(now, st, 'm') > 0) {//起始时间比当前早
                            t_start.val(timeStep(now));
                            t_start.trigger("change");
                        }
                    }
                    var load = userPanel.find(".fp-load-status .load").show();
                    opt.evUpTime(date.val(), opt.allDay ? "" : t_start.val(), opt.allDay ? "" : t_end.val(), function (list, type, para) {
                        userCon.html("");
                        rsch.html("<option value='0'>---" + uni.translate("查找") + "---</option>");
                        dots = cvtUniLab3(list, iobjs, para);
                        $.each(dots, function (i) {
                            createDot(this, date.val(), opt.allDay ? this.openStart : t_start.val(), opt.allDay ? this.openEnd : t_end.val());
                            rsch.append("<option value='" + this.devId + "'>" + this.devName + "</option>");
                        });
                        userCon.find(".fp-dot").tooltip();
                        load.hide();
                    });
                }
                else if (opt.evUpStatus) {
                    opt.evUpStatus(function (list, type, para) {
                        userCon.html("");
                        rsch.html("<option value='0'>----" + uni.translate("查找") + "----</option>");
                        dots = cvtPostion(list, iobjs, para);
                        $.each(dots, function (i) {
                            createStatus(this);
                            rsch.append("<option value='" + this.devId + "'>" + this.devName + "</option>");
                        });
                        userCon.find(".fp-dot").tooltip();
                    });
                }
                else
                    uni.msgBox("evUpTime函数未定义");
            }).appendTo(userPanel);
            userPanel.append("<span class='fp-load-status'><span class='load' style='display:none;'>Loading</span></span>");
            date.change(function () {
                search.trigger("click");
            });
            t_start.change(function () {
                if (t_end.val()) {
                    var start = parseInt($(this).val().replace(':', ''));
                    var end = parseInt(t_end.val().replace(':', ''));
                    if (end < start) {
                        t_end.val(t_start.val());
                    }
                }
            });
            t_end.change(function () {
                t_start.trigger("change");
            });
            //提示
            var half = "<span class='fp-dot fp-dot-doing fp-dot-tip' style='position:relative;'><div class='fp-dot-layer fp-layer-0'/><div class='fp-dot-layer fp-layer-1'/></span>" + uni.translate("半空闲") + "&nbsp;";
            userPanel.append("<span><span class='fp-dot fp-dot-ok fp-dot-tip'></span>" + uni.translate("空闲") + "&nbsp;" + (opt.mode === "status" ? "" : half) + "<span class='fp-dot fp-dot-busy fp-dot-tip'></span>" + uni.translate("忙碌") + "&nbsp;<span class='fp-dot fp-dot-close fp-dot-tip'></span>" + uni.translate("不开放") + "&nbsp;</span>");
            //查找操作
            rsch.change(function () {
                var id = $(this).val();
                var list = userCon.find(".fp-dot");
                for (var i = 0; i < list.length; i++) {
                    var dot = $(list[i]);
                    if (dot.attr("key") == id)
                        dot.addClass("fp-dot-ing");
                    else
                        dot.removeClass("fp-dot-ing");
                }
            }).appendTo(userPanel);
            //转编辑
            if (opt.isEdit) {
                $("<a class='fp-cvt-mode'>编辑</a>").click(function () {
                    iqz.addClass("fp-editing");
                    iqz.find(".fp-user").hide();
                    iqz.find(".fp-edit").show();
                }).appendTo(userPanel);
            }
            //初始值
            var now = new Date();
            var today = now.format("yyyy-MM-dd");
            date.val(today);
            t_start.val(timeStep(now));
            t_end.val(timeStep(now.addMinutes(opt.interval || 0)));
            search.trigger("click");
        }
        //绘制网格线
        function drawGuides() {
            var h = parseInt(content.height() / 16);
            var w = parseInt(content.width() / 16);
            var tbl = $("<table class='fp-guides-tbl'></table>");
            for (var i = 0; i < h; i++) {
                var tr = $("<tr></tr>");
                for (var j = 0; j < w; j++) {
                    tr.append("<td></td>");
                }
                tbl.append(tr);
            }
            guides.html(tbl);
        }
        function createDot(dot, dt, start, end) {
            var sz = parseInt(dot.size);
            var title = dot.name;
            var size = sz / 2;
            if (size < 12) size = 12;
            var width = title.length * size + 2;
            var sta = "close";
            var rate = "";
            if (dot.state != "close") {
                if (dot.freeSta == 0) sta = "ok";
                else if (dot.freeSta > -2 && dot.freeTime == 0) sta = "busy";
                else if (dot.freeTime > 0) {
                    sta = "doing";
                    if (opt.allDay) {
                        var now = new Date();
                        var st = uni.parseDate(dt + " " + start);
                        if (uni.compareDate(now, st, 'm') > 0) {//起始时间比当前早
                            start = timeStep(now);
                        }
                    }
                    rate += "<div class='fp-dot-layer fp-layer-0'/>";
                    var dv = parseInt(dot.freeTime * 100 / uni.compareDate(dt + " " + end, dt + " " + start, 'm'));
                    if (dv > 24) rate += "<div class='fp-dot-layer fp-layer-1'/>";
                    if (dv > 49) rate += "<div class='fp-dot-layer fp-layer-2'/>";
                    title += "(空:" + m2hm(dot.freeTime) + ")";
                }
            }
            var clk = $("<div class='fp-dot fp-dot-" + sta + "' key='" + dot.id + "' title='" + title + "' style='position:absolute;width:" + sz + "px;height:" + sz + "px;line-height:" + sz + "px;top:" + dot.top + ";left:" + dot.left + ";'>" + rate +
                "<div class='fp-obj-title' style='position: absolute;text-align:center;top:" + (sz + 3) + "px;left:" + (sz - width) / 2 + "px;width: " + width + "px;line-height: " + size + "px;font-size: " + size + "px;'><span>" + title + "</span></div></div>");
            if (opt.evSelDot && sta != "close") {
                clk.click(function () {
                    opt.evSelDot({ obj: dot, dt: dt, start: start, end: end });
                });
            }
            userCon.append(clk);
        }
        function createStatus(dot) {
            var sz = parseInt(dot.size);
            var title = dot.name;
            var size = sz / 2;
            if (size < 12) size = 12;
            var width = title.length * size + 2;
            var sta = dot.state;
            var clk = $("<div class='fp-dot fp-dot-" + sta + "' key='" + dot.id + "' title='" + title + "' style='position:absolute;width:" + sz + "px;height:" + sz + "px;line-height:" + sz + "px;top:" + dot.top + ";left:" + dot.left + ";'>" +
                "<div class='fp-obj-title' style='position: absolute;text-align:center;top:" + (sz + 3) + "px;left:" + (sz - width) / 2 + "px;width: " + width + "px;line-height: " + size + "px;font-size: " + size + "px;'><span>" + title + "</span></div></div>");
            if (opt.evSelDot && sta == "ok") {
                clk.click(function () {
                    opt.evSelDot({ obj: dot });
                });
            }
            userCon.append(clk);
        }

        function createBox(key, title, top, left, sz, next) {
            var selObj = selObjs.children("[value=" + key + "]");
            var index = selObj.index();
            if (index < 0) return;
            selObj.addClass("fp-obj-do");
            sz = parseInt(sz);
            var size = sz / 2;
            if (size < 12) size = 12;
            var width = title.length * size + 2;
            var bx = $("<div class='fp-box fp-box-act fp-box-sel' key='" + key + "' index='" + index + "' title='" + title + "' style='position:absolute;width:" + sz + "px;height:" + sz + "px;line-height:" + sz + "px;top:" + top + ";left:" + left + ";'><div class='fp-box-drag' style='height:100%;'>" + (index + 1) +
                "</div><div class='fp-obj-title' style='position: absolute;text-align:center;top:" + (sz + 3) + "px;left:" + (sz - width) / 2 + "px;width: " + width + "px;line-height: " + size + "px;font-size: " + size + "px;'><span>" + title + "</span></div></div>");
            bx.fpDrag(content);
            $(".fp-box", content).removeClass("fp-box-sel");
            editCon.append(bx);
            bx.index = index;
            bx.key = key;
            bx.sz = sz;
            boxs.set(key, bx);
            ibox = bx;
            //自动下一个
            if (next) {
                var next = selObj.next();
                if (next && next.length > 0) {
                    selObjs.val(next.val());
                    selObjs.trigger("change");
                }
            }
        }
        $.fn.fpDrag = function (content) {
            var o = this;
            var move = false;
            var _x;
            var _y;
            var o_x;
            var o_y;
            o.click(function (ev) {
                if (!move) {
                    if (!ev.ctrlKey) {
                        $.each(boxs.values(), function () {
                            if (this != o) this.removeClass("fp-box-sel");
                        })
                    }
                    o.switchover("fp-box-sel");
                }
                ev.stopPropagation();
            });
            o.mousedown(function (ev) {
                move = false;
                _x = ev.pageX;
                _y = ev.pageY;
                var offset = o.offset();
                o_x = offset.left;
                o_y = offset.top;
                content.mousemove(function (ev) {
                    var x = ev.pageX - _x;
                    var y = ev.pageY - _y;
                    if (x != 0 && y != 0) {
                        move = true;
                    }
                    o.offset({ top: (o_y + y), left: (o_x + x) });
                });
            });
        }
        content.mouseup(function () {
            content.unbind("mousemove");
        });
        //-------------------------------other function
        function timeStep(dt) {
            var t = dt.getMinutes();
            var diff = t % opt.step;
            if (diff > 0) {
                dt.addMinutes(opt.step - diff);
            }
            return dt.format("HH:mm");
        }
        function m2hm(m) {
            var t = parseInt(m);
            var hm = parseInt(t / 60) + uni.translate('时');
            hm += parseInt(t % 60) + uni.translate('分');
            return hm;
        }
    }

    //-------------------------传值转换

    //pro:实验室 值转换
    function cvtUniLab3(list, cs, para) {
        var boxs = [];
        if (!para) para = {};
        $.each(list, function () {
            var box = this;
            if ((this.prop & 524288) > 0) return true;//不支持预约
            if (this.state == "close" && !para.showClose) {
                return true;
            }
            if (this.state == "forbid") box.state = "close";//禁用按不开放处理
            box.id = this.devId;
            box.name = this.devName;
            //开放时间
            var open = this.open;
            if (open && open.length > 1) {
                this.openStart = open[0];
                this.openEnd = open[1];
            }
            //坐标
            for (var i = 0; i < cs.length; i++) {
                var c = cs[i];
                if (c.id == box.id) {
                    if (c.size && (c.top || c.left)) {
                        box.top = c.top || 0;
                        box.left = c.left || 0;
                        box.size = c.size;
                        boxs.push(box);
                    }
                    break;
                }
            }
        });
        return boxs;
    }
    //pro:位置状态 值转换
    function cvtPostion(list, cs, para) {
        var boxs = [];
        if (!para) para = {};
        $.each(list, function () {
            var box = this;
            if (!this.status) box.state = "close";//禁用按不开放处理
            else if (this.status == 1) box.state = "ok";
            else if (this.status == 2) box.state = "doing";
            box.id = this.devId;
            box.name = this.devName;
            //坐标
            for (var i = 0; i < cs.length; i++) {
                var c = cs[i];
                if (c.id == box.id) {
                    if (c.size && (c.top || c.left)) {
                        box.top = c.top || 0;
                        box.left = c.left || 0;
                        box.size = c.size;
                        boxs.push(box);
                    }
                    break;
                }
            }
        });
        return boxs;
    }
})(jQuery, uni);