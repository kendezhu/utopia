jQuery.Utopia = {
    //文本框字数插件(文本框ID，字数ID，最大字数)
    textNum: function (textId, numId, maxLength) {
        $(textId).keyup(function () {
            $txt = $(this);
            var len = $txt.val().length;
            var leftLen = maxLength - len;
            $(numId).text(leftLen);
        });
    },

    //键盘上下选择，Enter选择(文本框ID父容器ID,文本框ID,UL的ID)
    keyUpDownEnter: function (textFatherId, textId, ulId) {
        //键盘事件
        var index = -1; //当前选中li的index
        $(textFatherId).delegate(textId, "keydown", function (e) {
            var last = $(ulId).children("li[class^=atUserLi]").length - 1; //最后一个li的index
            //上
            if (e.keyCode == 38) {
                index--;
                if (index < 0) {
                    index = last;
                }
                $(ulId).children("li[class$=active]").removeClass("active");
                $(ulId).children("li[class=atUserLi]").eq(index).addClass("active");
            }
            //下
            if (e.keyCode == 40) {
                index++;
                if (index > last) {
                    index = 0;
                }
                $(ulId).children("li[class$=active]").removeClass("active");
                $(ulId).children("li[class=atUserLi]").eq(index).addClass("active");
            }
            //回车
            if (e.keyCode == 13) {
                var $user = $(ulId).children("li[class$=active]");
                var txt = $("#wordBody").val();
                var userId = $user.attr("atUserId");
                var userName = $user.text().split(',')[0];
                $("#wordBody").val(txt + "@" + userName + "(" + userId + ")");
                //处理剩余字数
                var len = $("#wordBody").val().length;
                var leftLen = 140 - len;
                $("#wordNum").text(leftLen);
            }
        })

        //鼠标事件
        $(textFatherId).delegate("li[class^=atUserLi]", "mouseover", function (e) {
            $(ulId).children("li[class^=atUserLi]").removeClass("active");
            $(this).addClass("active");
            index = $(ulId).children("li[class^=atUserLi]").index($(ulId).children("li[class$=active]"));
        })
        $(textFatherId).delegate("li[class^=atUserLi]", "click", function (e) {
            var $user = $(ulId).children("li[class$=active]");
            var txt = $("#wordBody").val();
            var userId = $user.attr("atUserId");
            var userName = $user.text().split(',')[0];
            $("#wordBody").val(txt + "@" + userName + "(" + userId + ")");
            //处理剩余字数
            var len = $("#wordBody").val().length;
            var leftLen = 140 - len;
            $("#wordNum").text(leftLen);
        })

    }

}