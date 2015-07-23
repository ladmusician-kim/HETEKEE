function getJson(url, data, success, error, $scope) {
    $.ajax({
        url: url,
        type: "POST",
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(data),
        xhrFields: {
            withCredentials: true
        },
        success: function (data) {
            $scope.$apply(function () {
                success(data)
            });
        },
        error: function (arg) {
            if (error) {
                $scope.$apply(function () {
                    error(arg);
                });
            } else {
                alert("error");
            }
        },
    });
}