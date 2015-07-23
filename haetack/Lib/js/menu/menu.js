var home = $('#home').length > 0;
var service = $('#service').length > 0;
var news = $('#news').length > 0;
var aboutus = $('#aboutus').length > 0;

//if ($(window).width() > 601) {
//    $('.mn-has-sub').hover(function () {
//        var margin = $(this).position().left;
//        if ($(this).hasClass('aboutus')) {
//            var menuWidth = $(this).width();
//            var subWidth = $($($(this).parent()).children('.mn-sub')).children('li').width();

//            $('.hetekee-sub-nav').css('padding-left', margin + (menuWidth - subWidth) / 2 + 'px');
//        } else {
//            $('.hetekee-sub-nav').css('padding-left', margin + 'px');
//        }
//    });
//}


// menu selected
if (home) {
    $($('.hetekee-nav-item')[0]).addClass("hetekee-color selected");
} else if (service) {
    $('.hetekee-nav-panel').addClass('hetekee-menu-selected');
    $($('.hetekee-nav-item')[1]).addClass("hetekee-color selected");
    $($('.hetekee-nav-item')[1]).find("img.hetekee-nav-item-on:first").fadeIn("fast");
} else if (news) {
    $('.hetekee-nav-panel').addClass('hetekee-menu-selected');
    $($('.hetekee-nav-item')[2]).addClass("hetekee-color selected");
    $($('.hetekee-nav-item')[2]).find("img.hetekee-nav-item-on:first").fadeIn("fast");
} else if (aboutus) {
    $('.hetekee-nav-panel').addClass('hetekee-menu-selected');
    $($('.hetekee-nav-item')[3]).addClass("hetekee-color selected");
    $($('.hetekee-nav-item')[3]).find("img.hetekee-nav-item-on:first").fadeIn("fast");
} else {
    $($('.hetekee-nav-item')[0]).addClass("hetekee-color selected");
}

if ($(window).width() > 700) {
   
} else {
    $("img.hetekee-nav-item-on").stop(true, true).delay(100).fadeOut("fast");
}




var delayTime = 1000;
var fadeOutTime = 50;
$('.hetekee-nav-language').hover(function () {
    $('.hetekee-nav-bottom-arrow').attr('src', "/Lib/images/menu/menu_icon_kor_green.png");
    //all 0.27s cubic-bezier(0.000, 0.000, 0.580, 1.000)
    //$('.hetekee-nav-bottom-arrow').fadeOut(fadeOutTime, function () {
    //    $('.hetekee-nav-bottom-arrow').attr('src', "/Lib/images/menu/menu_icon_kor_green.png");
    //    $('.hetekee-nav-bottom-arrow').fadeIn(delayTime);
    //});
    
});
$('.hetekee-nav-language').mouseout(function () {
    $('.hetekee-nav-bottom-arrow').attr('src', "/Lib/images/menu/menu_icon_kor.png");
});