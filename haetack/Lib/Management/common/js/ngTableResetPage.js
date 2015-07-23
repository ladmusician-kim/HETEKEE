function tableResetPageWhenIfNeeded(cacheData, tableParams, task) {
    if (!(JSON.stringify(cacheData.filter) === JSON.stringify(tableParams.filter()) &&
          JSON.stringify(cacheData.sorting) === JSON.stringify(tableParams.sorting()))) {
        cacheData.filter = $.extend(true, {}, tableParams.filter());
        cacheData.sorting = $.extend(true, {}, tableParams.sorting());
        tableParams.page(1);
    }
    task();
}