$(function () {
    $("div.angularViewPlace").each(function () {
        var t = $(this);
        var v = t.data("view");
        $.ajax({
            url: getUrl("app/" + v + ".html?" + Unicorn_AppVersion),
            cache: true,
            dataType: "html",
            success: function (data) {
                var u = baseUrl;
                if (u.lastIndexOf('/') == u.length - 1)
                    u = u.substr(0, u.length - 1);
                data = data.replace(/{appPath}/g, u);
                t.html(data);
                if (v.lastIndexOf('/') > 0)
                    v = v.substr(v.lastIndexOf('/') + 1);
                angular.bootstrap(t, [v + 'App']);
            }
        })
        //t.load(getUrl("app/" + v + ".html"), function () {
        //    if (v.indexOf('/') > 0)
        //        v = v.substr(v.indexOf('/') + 1);
        //    angular.bootstrap($(this), [v + 'App']);
        //});
    });
});