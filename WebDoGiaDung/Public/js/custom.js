$('.product-slider').slick({
    infinite: true,
    autoplay: true,
    autoplaySpeed: 2000,
    slidesToShow: 4,
    slidesToScroll: 1,
    nextArrow: '<button class="btn-slider btnNext-slider"><i class="fa-solid fa-chevron-right"></i></button>',
    prevArrow: '<button class="btn-slider btnPrev-slider"><i class="fa-solid fa-chevron-left"></i></button>'
});
       
$('.slider-for').slick({
    slidesToShow: 1,
    slidesToScroll: 1,
    arrows: false,
    fade: true,
    asNavFor: '.slider-nav'
});
$('.slider-nav').slick({
    slidesToShow: 3,
    slidesToScroll: 1,
    asNavFor: '.slider-for',
    dots: false,
    centerMode: true,
    focusOnSelect: true,
    nextArrow: '<button class="btn-slider btnNext"><i class="fa-solid fa-chevron-right"></i></button>',
    prevArrow: '<button class="btn-slider btnPrev"><i class="fa-solid fa-chevron-left"></i></button>'
});

$('.related-product').slick({
    infinite: true,
    autoplay: true,
    autoplaySpeed: 2000,
    slidesToShow: 4,
    slidesToScroll: 1,
    nextArrow: '<button class="btn-slider btnNext" style="top: 45%;"><i class="fa-solid fa-chevron-right"></i></button>',
    prevArrow: '<button class="btn-slider btnPrev" style="top: 45%;"><i class="fa-solid fa-chevron-left"></i></button>'
});
     

// Back to top
$('.back-top').on('click', function () {
    $('html, body').animate({ scrollTop: 0 }, '300');
    return false;
});

// target js
function WIYN() {
    Swal.fire({
        icon: 'warning',
        title: "Bạn cần đăng nhập để tiếp tục!",
        showCancelButton: true,
        confirmButtonColor: '#4a90e2',
        cancelButtonColor: '',
        confirmButtonText: 'Đăng nhập'
    }).then((result) => {
        if (result.value) {
            $('#exampleModal').modal('show');
        }
    })
}
function AwToast(icon, content, timer) {
    Swal.fire({
        icon: icon,
        title: content,
        timer: timer
    })
}

$('#modal').modal({ backdrop: 'static', keyboard: false })
$(document).ready(function () {
    $('.dk-lg').click(function () {
        $('#exampleModal').modal('hide');
        $('#exampleModal').on('hidden.bs.modal', function () {
            $(this).find("input").val('').end();
        });
    })
    $('.dn-lg').click(function () {
        $('#exampleModal1').modal('hide');
        $('#exampleModal1').on('hidden.bs.modal', function () {
            $(this).find("input").val('').end();
        });
    })

})

// Notification FE
$(window).on('load', function () {
    const mgs_type = $('input[name=u_mgs_type]').val();
    const msg = $('input[name=u_mgs]').val();
    if (mgs_type != '' && msg != '') {
        switch (mgs_type) {
            case 'success':
                AwToast(mgs_type, msg, 10000)
                break;
            case 'warning':
                AwToast(mgs_type, msg, 10000)
                break;
            case 'danger':
                AwToast(mgs_type, msg, 10000)
                break;
        }
    }
});

$(function () {
    $('.notready').click(function (e) {
        e.preventDefault();
        Swal.fire({
            icon: 'warning',
            title: 'Chức năng chưa sẵn sàng!',
            timer: 5000
        })
    })
})


$(document).on('click', '#btn-register', function (e) {
    e.preventDefault();
    $.ajax({
        url: "/account/register",
        type: "POST",
        data: {
            user: {
                Email: $('#Email').val(),
                Address: $('#Address').val(),
                FullName: $('#FullName').val(),
                UserName: $('#UserName').val(),
                Password: $('#Pass').val(),
                Phone: $('.Phone').val(),
            }
        },
        dataType: 'json',
        success: function (data) {
            if (data.Code == 0) {
                $('#exampleModal1').modal("hide")
                swal({
                    icon: "success",
                    title: data.Message,
                    timer: 5000
                });
                return;
            } else if (data.Code == 1) {
                swal({
                    icon: "warning",
                    title: data.Message,
                    timer: 5000
                });
                return;
            }
            swal({
                icon: "warning",
                title: data.Message,
                timer: 5000
            });
        }
    });
});


$(document).on('click', '#btn-login', function (e) {
    e.preventDefault();

    const User = $('input[name="UserName"]').val().trim();
    const Password = $('input[name="Password"]').val().trim();

    console.log(User, Password)
    if (User == '') {
        $('#nameHelp').html('Vui lòng nhập Email hoặc Số điện thoại!');
        return false;
    }
    else if (Password == '') {
        $('#passHelp').html('Vui lòng nhập mật khẩu!');
        return false;
    }
    $.ajax({
        url: "/account/userlogin",
        type: 'POST',
        data: { user: User, password: Password },
        dataType: 'json',
        success: function (s) {
            if (s.s == 1) {
                $('#nameHelp').text('Thông tin tài khoản không chính xác');
                return;
            } else if (s.s == 2) {
                $('#passHelp').text('Mật khẩu không chính xác');
                return;
            } else if (s.s == 0) {
                window.location.reload();
                return;
            }
        }
    });
});

$(function () {
    $('.shop input').click(function () {
        $('#nameHelp').html('');
        $('#passHelp').html('');
    })
});

$(document).ready(function () {
    $('.checkAuth').click(function (e) {
        e.preventDefault();
        $.ajax({
            url: "/cart/checkauth",
            type: 'POST',
            success: function (response) {
                if (response == 1) {
                    window.location.href = "gio-hang";
                } else {
                    WIYN();
                }
            }
        });
    });
})

$(document).on('click', '#buy-btn', function (e) {
    e.preventDefault();

    $.ajax({
        url: 'cart/add',
        type: 'POST',
        data: {
            pid: $(this).data('id'),
            qty: $('.etd').val()
        },
        dataType: 'json',
        success: function (response) {
            if (response.result == 1) {
                $('.slc').text(parseInt($('.slc').text()) + 1);
                AwToast('success', 'Đã thêm sản phẩm vào giỏ hàng');
                $(".swal2-confirm").click(function () {
                    window.location.reload();
                })
            } else if (response.result == 2) {
                AwToast('success', 'Đã cập nhật lại số lượng sản phẩm trong giỏ hàng');
            }
        }
    })
})

$(document).on('click', '.update', function (e) {
    e.preventDefault();

    $.ajax('cart/update', {

        type: 'POST',
        data: { pid: $(this).data('id'), option: $(this).data('options') },
        dataType: 'json',
        success: function (res) {
            if (res == 1 || res == 2) {
                $('#cart-checkout').load('module/icart');
                return;
            } else if (res == 3) {
                $('#cart-checkout').load('module/icart');
                AwToast('info', 'Đã xóa sản phẩm khỏi giỏ hàng!', 5000);
                return;
            }
            AwToast('danger', '404!', 5000);
        }
    })
})

function validateForm() {
    if ($('#email').val() == null || $('#email').val() == '') {
        Swal.fire('Vui lòng nhập email!');
        return false;
    } else if (($('#address').val().trim() == null || $('#address').val().trim() == '')) {
        Swal.fire('Vui lòng nhập địa chỉ nhận hàng!');
        return false;
    }
    return true;
}

$(document).ready(function () {
    $('#check-out').click(function (e) {
        e.preventDefault();
        if (($('#fullname').val().trim() == null || $('#fullname').val().trim() == '')) {
            Swal.fire('Vui lòng nhập tên khách hàng!');
            return false;
        }else if (($('#phone').val().trim() == null || $('#phone').val().trim() == '')) {
            Swal.fire('Vui lòng nhập số điện thoại!');
            return false;
        } else if ($('#email').val() == null || $('#email').val() == '') {
            Swal.fire('Vui lòng nhập email!');
            return false;
        } else if (($('#address').val().trim() == null || $('#address').val().trim() == '')) {
            Swal.fire('Vui lòng nhập địa chỉ nhận hàng!');
            return false;
        } 
        $.ajax({
            url: "/cart/payment",
            type: 'POST',
            data: {
                FullName: $('input[name=FullName]').val(),
                Phone: $('input[name=Phone]').val(),
                Email: $('input[name=CustomerEmail]').val(),
                Address: $('input[name=CustomerAddress]').val(),
            },
            dataType: 'json',
            success: function (pig) {
                if (!pig) {
                    console.log(pig);
                } else {
                    window.location.href = "account/order";
                }
            }
        })
    })
});

