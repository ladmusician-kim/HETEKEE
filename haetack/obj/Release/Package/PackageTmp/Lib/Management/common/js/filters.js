'use strict';

/* Filters */

angular.module('myApp.filters', [])
	.filter("breakwords", [function () {
		return function (value, length) {
			if (length) {
				if (value.length <= length) {
					return value;
				} else {
					return value.substr(0, length) + "...";
				}
			}
			return value;
		};
	}])
    .filter('interpolate', ['version', function (version) {
        return function (text) {
            return String(text).replace(/\%VERSION\%/mg, version);
        }
    }])
    .filter('aimToDate', [function () {
        return function (aim) {
            if (!aim) return "";
            if (aim.Targeted != undefined) {
                return aim.Targeted;
            } else if (aim.TestInstance != undefined && aim.TestInstance.Happens != undefined) {
                return aim.TestInstance.Happens;
            }
            return "";
        }
    }])
    .filter('dday', [function () {
        return function (value) {
            try {
                var then = new Date(parseInt(value.substr(6)))
                var now = new Date();
                var gap = then.getTime() - now.getTime();
                gap = Math.floor((gap + ((1000 * 60 * 60 * 24) / 2)) / (1000 * 60 * 60 * 24));
                if (gap == 0) {
                    return '오늘';
                } else if (gap == 1) {
                    return "내일"
                } else if (gap == 2) {
                    return "모레";
                } else if (gap == -1) {
                    return "어제";
                } else if (gap < -1) {
                    return "D+" + (-gap);
                } else {
                    return "D-" + gap;
                }
            }
            catch (e) {
                return "";
            }
        }
    }])
    .filter('studypattern', [function () {
        return function (value) {
            try {
                if (value >= 0.7) {
                    return "좋음";
                } else if (value >= 0.3) {
                    return "보통";
                }
            }
            catch (e) {
            }
            return "나쁨";
        }
    }])
    .filter('sheettype', [function () {
        return function (value) {
            try {
                if (value.QuestionSheetType.Id == 1) {
                    return "모의고사";
                }
                if (value.QuestionSheetType.Id == 2) {
                    return "보충학습지";
                }
            }
            catch (e) {
            }
            return "";
        }
    }])
    .filter('dateize', [function () {
        return function (value) {
            try {
                return new Date(parseInt(value.substr(6)));
            }
            catch (e) {
                return new Date();
            }
        }
    }])
    .filter('day', [function () {
        return function (value) {
            try {
                var then = value;
                var now = new Date();
                var gap = then.getTime() - now.getTime();
                gap = Math.floor((gap + ((1000 * 60 * 60 * 24) / 2)) / (1000 * 60 * 60 * 24));
                if (gap == 0) {
                    return '오늘';
                } else if (gap == 1) {
                    return "내일"
                } else if (gap == 2) {
                    return "모레";
                } else if (gap == -1) {
                    return "어제";
                } else if (gap < -1 && now.getMonth() == then.getMonth()) {
                    return (-gap) + "일전";
                } else {
                    if (now.getYear() == then.getYear()) {
                        return (1 + then.getMonth()) + "월 " + then.getDate() + "일";
                    } else {
                        return then.getFullYear() + "년 " + (1 + then.getMonth()) + "월 " + then.getDate() + "일";
                    }
                }
            }
            catch (e) {
                return "";
            }
            return "";
        }
    }])
    .filter('ko_weekday', [function () {
        return function (value) {
            try {
                value = value.replace("Sunday", "일요일");
                value = value.replace("Monday", "월요일");
                value = value.replace("Tuesday", "화요일");
                value = value.replace("Wednesday", "수요일");
                value = value.replace("Thursday", "목요일");
                value = value.replace("Friday", "금요일");
                value = value.replace("Saturday", "토요일");
                value = value.replace("Sunday", "일요일");
                value = value.replace("Mon", "월");
                value = value.replace("Tue", "화");
                value = value.replace("Wed", "수");
                value = value.replace("Thu", "목");
                value = value.replace("Fri", "금");
                value = value.replace("Sat", "토");
                return value;
            }
            catch (e) {
                return value;
            }
        }
    }])
    .filter('ko_month', [function () {
        return function (value) {
            try {
                value = value.replace("January", "1월");
                value = value.replace("February", "2월");
                value = value.replace("March", "3월");
                value = value.replace("April", "4월");
                value = value.replace("May", "5월");
                value = value.replace("June", "6월");
                value = value.replace("July", "7월");
                value = value.replace("August", "8월");
                value = value.replace("September", "9월");
                value = value.replace("October", "10월");
                value = value.replace("November", "11월");
                value = value.replace("December", "12월");
                return value;
            }
            catch (e) {
                return value;
            }
        }
    }])
    .filter('questionMenuLabel', [function () {
        return function (value) {
            if (value == undefined || value == null) return '';
            if (value.type == 'section') {
                return '☆';
            } else {
                return value.MenuSeqNo + '번';
            }
        }
    }])
    .filter('questionXslt', ['xslt', function (xslt) {
        return function (value) {
            var id = value.uuid;
            var node, data;

            if (value == undefined) {
                return '';
            }

            var trycount = 10;

            var updateDom = function () {
                trycount--;

                if (node == undefined) {
                    if (trycount > 0) {
                        setTimeout(function () {
                            var el = $("#xml" + id);
                            node = el[0];
                            updateDom();
                        }, 100);
                    }
                    return;
                }

                if (data == undefined) {
                    var doc = xslt.dom(value.Xml);
                    var val = xslt.questionTransform(doc);

                    var html = new XMLSerializer().serializeToString(val);

                    if (node.hasChildNodes != undefined) {
                        while (node.hasChildNodes()) {
                            node.removeChild(node.lastChild);
                        }
                    }
                    data = val;
                }

                node.appendChild(data);

                el.data('xslt' + id, val);

                var svgwebs = $("#xml" + id + " .svgweb");
                var svg = true;

                for (var index = 0; index < svgwebs.length; index++) {
                    try {
                        var svgweb = svgwebs[index];
                        var span = svgweb.parentNode;
                        var svgxml = svgweb.innerHTML;
                        if (svg) {
                            span.className += span.className ? ' svgxml' : 'svgxml';
                            span.innerHTML = svgxml;
                        }
                    } catch (e) {
                    }
                }

                var svgs = document.getElementsByTagName("svg");

                for (var index = 0; index < svgs.length; index++) {
                    var svg = svgs[index];
                    var span = svg.parentNode;
                    var height = svg.getAttribute('height');
                    var width = svg.getAttribute('width');
                    if (height != null && height != undefined) {
                        var vertical_height = -parseFloat(height) / 2.0 + 5.0;
                        span.style.verticalAlign = vertical_height + "px";
                    }
                }
            };

            var el = $("#xml" + id);
            node = el[0];
            if (node == undefined) {
                setTimeout(function () {
                    var el = $("#xml" + id);
                    node = el[0];
                    updateDom();
                }, 100);
            } else {
                data = el.data('xslt' + id);
                setTimeout(updateDom, 0);
            }
            return "";
        };
    }])
    .filter('sectionXslt', ['xslt', function (xslt) {
        return function (value, id) {
            var id = value.uuid;
            var node, data;

            if (value == undefined) {
                return '';
            }

            var trycount = 10;

            var updateDom = function () {
                trycount--;

                if (node == undefined) {
                    if (trycount > 0) {
                        setTimeout(function () {
                            var el = $("#xml" + id);
                            node = el[0];
                            updateDom();
                        }, 100);
                    }
                    return;
                }

                if (data == undefined) {
                    var doc = xslt.dom(value.Xml);
                    var val = xslt.sectionTransform(doc);

                    var html = new XMLSerializer().serializeToString(val);

                    if (node.hasChildNodes != undefined) {
                        while (node.hasChildNodes()) {
                            node.removeChild(node.lastChild);
                        }
                    }
                    data = val;
                }

                node.appendChild(data);

                el.data('xslt' + id, val);

                var svgwebs = $("#xml" + id + " .svgweb");
                var svg = true;

                for (var index = 0; index < svgwebs.length; index++) {
                    try {
                        var svgweb = svgwebs[index];
                        var span = svgweb.parentNode;
                        var svgxml = svgweb.innerHTML;
                        if (svg) {
                            span.className += span.className ? ' svgxml' : 'svgxml';
                            span.innerHTML = svgxml;
                        }
                    } catch (e) {
                    }
                }

                var svgs = document.getElementsByTagName("svg");

                for (var index = 0; index < svgs.length; index++) {
                    var svg = svgs[index];
                    var span = svg.parentNode;
                    var height = svg.getAttribute('height');
                    var width = svg.getAttribute('width');
                    if (height != null && height != undefined) {
                        var vertical_height = -parseFloat(height) / 2.0 + 5.0;
                        span.style.verticalAlign = vertical_height + "px";
                    }
                }
            };

            var el = $("#xml" + id);
            node = el[0];
            if (node == undefined) {
                setTimeout(function () {
                    var el = $("#xml" + id);
                    node = el[0];
                    updateDom();
                }, 100);
            } else {
                data = el.data('xslt' + id);
                setTimeout(updateDom, 0);
            }
            return "";
        };
    }])
	.filter('shortdate', [function () {
		return function (value) {
			try {
				if (value == "") {
					return "";
				}
				var then = new Date(parseInt(value.substr(6)))
				var year = then.getYear() - 100 + 2000;
				var month = then.getMonth() + 1;
				var date = then.getDate();
				return year + "-" + month + "-" + date;
			}
			catch (e) {
				return new Date();
			}
		}
	}])
	.filter('IsDone', [function () {
		return function (value) {
			try {
				if (value) {
					return "수행";
				}
				else {
					return "미수행";
				}
			}
			catch (e) {
				return "";
			}
		}
	}])
	.filter('CountOfTest', [function () {
		return function (value) {
			try {
				if (value == 0) {
					return "";
				}
				else {
					return value + "회";
				}
			}
			catch (e) {
				return "";
			}
		}
	}])
	.filter('DoubleStringFormat', [function () {
		return function (value) {
			console.log(value);
			try {
				console.log("filter decimal point")
				return value.toFixed(1);
			}
    		catch (e) {
    			console.log(e);
				return "오류발생";
			}
		}
	}])
	.filter('priceUnit', [function () {
		return function (value) {
			if (value >= 1000) {
				if (value % 1000 == 0) {
					return parseInt(value / 1000) + "," + "000";
				} else {
					return parseInt(value / 1000) + "," + value % 1000;
				}
			} else {
				return value;
			}
		}
	}])
    .filter('blogTitle', [function () {
        return function (value) {
            if (value.length >= 15) {
                return value.substring(0, 14) + '....';
                 
            } else {
                return value;
            }
        }
    }])
;
