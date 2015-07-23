$(window).scroll(function () {
    changeNavBar();
});
changeNavBar();

function changeNavBar() {
    var scrollPosition = $(document).scrollTop();
    if (scrollPosition <= 10) {
        $($('.hetekee-nav-panel')[0]).addClass("transperency-nav");
    } else {
        $($('.hetekee-nav-panel')[0]).removeClass("transperency-nav");
    }
}

var container = $('.hetekee-video-container').height();
var video = $('.hetekee-video').height();

var margin = (container - video) / 2;
$('.hetekee-video').css('margin-top', margin + 'px');