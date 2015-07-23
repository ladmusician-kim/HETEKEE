var app = angular.module('myApp', ['myApp.filters', 'ngTable']).
       controller('ManagementCtrl', function ($scope, $timeout, ngTableParams) {
           $scope.notice = {
               items: [],
               page: 1,
               perPage: 10,
               totalCount: 0
           }
           $scope.resetCacheData = {};
           $scope.tableParams = new ngTableParams({
               page: 1,            // show first page
               count: 10,          // count per page
               sorting: {
                   Id: 'desc',     // initial sorting
               },
               filter: {
               },
           }, {
               total: 0,
               getData: function ($defer, params) {
                   tableResetPageWhenIfNeeded($scope.resetCacheData, $scope.tableParams, function () {
                       $.ajax({
                           url: '/Management/GetNotices',
                           type: 'POST',
                           contentType: 'application/json',
                           data: JSON.stringify(params.url()),
                           dataType: 'json',
                           success: function (data) {
                               $scope.$apply(function () {
                                   $scope.notice.totalCount = data.RowCount;
                                   $scope.notice.page = $scope.tableParams.page();
                                   $scope.notice.perPage = $scope.tableParams.count();
                                   $scope.notice.items = data.Items;

                                   _.each($scope.notice.items, function (item) {
                                       item.$selected = false;
                                   });

                                   // update table params
                                   params.total(data.RowCount);
                                   $scope.totalLength = data.RowCount;
                                   // set new datax
                                   $defer.resolve(data.Items);
                               })
                           }
                       })
                   });
               }
           });


           $scope.deleteNotice = function (item) {
               var rtv = confirm("DB에서 완전히 삭제하시겠습니까?");
               if (rtv) {
                   getJson('/Management/DeleteNotice?id=' + item.Id, {},
                       function (data) {
                           if (data) {
                               $scope.notice.items = _.without($scope.notice.items, item);
                           }
                       }, function (arg) {
                           alert("데이터를 삭제하는데 문제가 발생했습니다. 콜미콜미 010-6233-8509");
                       }, $scope);
               }
           }
       });