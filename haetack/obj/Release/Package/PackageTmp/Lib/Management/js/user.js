var app = angular.module('myApp', ['myApp.filters', 'ngTable']).
       controller('ManagementCtrl', function ($scope, $timeout, ngTableParams) {
           $scope.user = {
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
                           url: '/Management/GetUsers',
                           type: 'POST',
                           contentType: 'application/json',
                           data: JSON.stringify(params.url()),
                           dataType: 'json',
                           success: function (data) {
                               $scope.$apply(function () {
                                   $scope.user.totalCount = data.RowCount;
                                   $scope.user.page = $scope.tableParams.page();
                                   $scope.user.perPage = $scope.tableParams.count();
                                   $scope.user.items = data.Items;

                                   _.each($scope.user.items, function (item) {
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
       });