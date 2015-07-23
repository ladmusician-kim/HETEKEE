var app = angular.module('myApp', ['myApp.filters', 'ngTable']).
       controller('ManagementCtrl', function ($scope, $timeout, ngTableParams) {
           $scope.pressless = {
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
                           url: '/Management/GetPresslesses',
                           type: 'POST',
                           contentType: 'application/json',
                           data: JSON.stringify(params.url()),
                           dataType: 'json',
                           success: function (data) {
                               $scope.$apply(function () {
                                   $scope.pressless.totalCount = data.RowCount;
                                   $scope.pressless.page = $scope.tableParams.page();
                                   $scope.pressless.perPage = $scope.tableParams.count();
                                   $scope.pressless.items = data.Items;

                                   _.each($scope.pressless.items, function (item) {
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

           $scope.deletePressless = function (item) {
               var rtv = confirm("DB에서 완전히 삭제하시겠습니까?");
               if (rtv) {
                   getJson('/Management/DeletePressless?id=' + item.Id, {},
                       function (data) {
                           if (data) {
                               $scope.pressless.items = _.without($scope.pressless.items, item);
                           }
                       }, function (arg) {
                           alert("데이터를 삭제하는데 문제가 발생했습니다. 콜미콜미 010-6233-8509");
                       }, $scope);
               }
           }
       });