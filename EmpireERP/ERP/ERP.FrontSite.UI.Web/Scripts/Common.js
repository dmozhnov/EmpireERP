var About = {
    Init: function () {
        $(document).ready(function () {
            // хак для ie
            if ($.browser.msie) {
                $('#about_slider_container .slide:first').find('.right_quote').css('left', '490px');
                $('#about_slider_container .slide::nth-child(2)').find('.right_quote').css('left', '465px');
                $('#about_slider_container .slide::nth-child(3)').find('.right_quote').css('left', '561px');
                $('#about_slider_container .slide::nth-child(4)').find('.right_quote').css('left', '599px');
            }

            // РЕАКЦИЯ НА ИЗМЕНЕНИЕ ШИРИНЫ ЭКРАНА
            resize_slider_back();

            $(window).resize(function () {
                resize_slider_back();
            });

            function resize_slider_back() {
                if (document.body.clientWidth < 1280 && document.body.clientWidth > 1000) {
                     $('#about_slider_back')
                        .css('width', (document.body.clientWidth) + 'px')
                        .css('background-position', (document.body.clientWidth - 1280) / 2 + 'px 0');
                }
                else if (document.body.clientWidth >= 1280) {
                    $('#about_slider_back')
                        .css('width', '1280px')
                        .css('background-position', '0 0');
                }
                else {
                    $('#about_slider_back')
                        .css('width', '1000px')
                        .css('background-position', '-140px 0');
                }

                if (document.body.clientWidth < 1047 && document.body.clientWidth > 1000) {
                    $('#slider_clouds')
                        .css('width', (document.body.clientWidth) + 'px')
                        .css('background-position', (document.body.clientWidth - 1047) / 2 + 'px 0');
                }
                else if (document.body.clientWidth >= 1047) {
                    $('#slider_clouds')
                        .css('width', '1047px')
                        .css('background-position', '0 0');
                }
                else {
                    $('#slider_clouds')
                        .css('width', '1000px')
                        .css('background-position', '-24px 0');
                }
            }

            // СЛАЙДЕР
            // параметры слайдера
            var curSlideId = 0;

            var sliderConfig = {
                slideWidth: 970,
                slideCount: 4,
                timeInterval: 10000
            }

            // настройка автоматической смены слайдов 
            var intervalId = setInterval(moveSliderNext, sliderConfig.timeInterval);

            // стрелка вперед
            $('#about_slider_next').click(function () {
                clearInterval(intervalId);  // остановка автоматической смены слайдов

                moveSliderNext();
            });

            // стрелка назад
            $('#about_slider_prev').click(function () {
                clearInterval(intervalId); // остановка автоматической смены слайдов

                moveSliderPrev();
            });

            // клик на навигаторе по слайдам
            $('#slider_switch li').click(function () {
                clearInterval(intervalId); // остановка автоматической смены слайдов

                // индекс текущего элемента
                curSlideId = $(this).index();

                setSlide(curSlideId);
            });

            // переход на следующий слайд
            function moveSliderNext() {
                curSlideId++;

                if (curSlideId === sliderConfig.slideCount) {
                    curSlideId = 0;
                }

                setSlide(curSlideId);
            }

            // переход к предыдущему слайду
            function moveSliderPrev() {
                curSlideId--;

                if (curSlideId === -1) {
                    curSlideId = sliderConfig.slideCount - 1;
                }

                setSlide(curSlideId);
            }

            // смена текущего слайда
            function setSlide(slideId) {
                var margin = -slideId * sliderConfig.slideWidth;

                $('#about_slider_container').animate({
                    'margin-left': margin
                });

                $('#slider_switch li').removeClass('selected').eq(slideId).addClass('selected');
            }
        });
    }
};

var Features = {
    Init: function () {
        $(document).ready(function () {
            SetTab($('#tabId').val() - 1, false);

            $('#slider_switch li').click(function () {
                var index = $(this).index();

                SetTab(index, true);
            });

            function SetTab(index, animate) {
                var margin = -index * 970;

                if (animate) {
                    $('#slide_container').animate({
                        'margin-left': margin
                    });
                }
                else {
                    $('#slide_container').css('margin-left',  margin);
                }

                $('#slider_switch li').removeClass('selected').eq(index).addClass('selected');
            }
        });
    }
};

