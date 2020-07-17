// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(function () {
    //// MASONRY, DO LATER ////////
    //// init Masonry
    //var $container = $('.grid-products').masonry({
    //    // options
    //    itemSelector: '.product-item',
    //    percentPosition: true
    //});

    //var masonryUpdate = function () {
    //    setTimeout(function () {
    //        $('grid-products').masonry();
    //    }, 10000);
    //}
    //// bind sort select
    //$('#product-sort').on('change', function () {
    //    var sortByValue = $(this).val();
    //    $.ajax({
    //        type: 'GET',
    //        url: '/Home/GetProducts' + createQueryString(sortByValue, null),
    //        success: function (result) {

    //            var items = [];
    //            // create new item elements
    //            for (i = 0; i < result.length; i++) 
    //            {
    //                var name = result[i].name;
    //                var price = result[i].price;
    //                var item = createProductItem(name, price);
    //                items.push(item);
    //            }
    //            var $items = $(items);
    //            masonryUpdate();
    //            $('.grid-products').append($('<div class="grid-sizer"></div>'));
    //            // append items to grid);
    //            $('.grid-products').masonry('remove', $('.product-item')).masonry();
    //            $('.grid-products').empty();
    //            $('.grid-products').append($items).masonry('appended', $items).masonry();
    //        }
    //    });
    //});

    //function createQueryString(sortOrder, searchString) {
    //    var queryString = '';
    //    if (sortOrder != null) {
    //        queryString = queryString + '?sortOrder=' + (sortOrder == 'Name' ? 'name_asc' : "price_asc");
    //    }
    //    if (searchString != null) {
    //        queryString = queryString + '&searchString=' + searchString;
    //    }
    //    return queryString;
    //}
    //function createProductItem(name, price) {

    //    var div1 = document.createElement('div');
    //    div1.className = "product-btns";
    //    var div1_1 = '<button class="main-btn icon-btn"><i class="fa fa-heart"></i></button>';
    //    var div1_2 = '<button class="main-btn icon-btn"><i class="fa fa-exchange"></i></button>';
    //    var div1_3 = '<button class="primary-btn add-to-cart"><i class="fa fa-shopping-cart"></i> Add to Cart</button>';
    //    div1.innerHTML = div1_1 + div1_2 + div1_3;

    //    var div2 = document.createElement('div');
    //    div2.className = "product-body";
    //    var div2_1 = '<h3 class="product-price price">' + price.toString() + '</h3>';
    //    var div2_2 = '<h2 class="product-name name"><a href="#">' + name + '</a></h2>';
    //    div2.innerHTML = div2_1 + div2_2 + div1.outerHTML;

    //    var div3 = document.createElement('div');
    //    div3.className = "product-thumb";
    //    var div3_1 = '<button class="main-btn quick-view"><i class="fa fa-search-plus"></i> Quick view</button>';
    //    var div3_2 = '<img src="/images/product01.jpg" alt="">';
    //    div3.innerHTML = div3_1 + div3_2;

    //    var div4 = document.createElement('div');
    //    div4.className = "product product-single";
    //    div4.innerHTML = div3.outerHTML + div2.outerHTML;

    //    var div5 = document.createElement('div');
    //    div5.className = "product-item";
    //    div5.innerHTML = div4.outerHTML;
    //    return div5;
    //}
    /////////////////

    // init Masonry
    var $container = $('.grid-products').masonry({
        // options
        itemSelector: '.product-item'
    });

    $container.imagesLoaded(function () {
        $container.masonry();
    });
    $(".product-quantity-input").on("change", function () {
        var price = parseFloat($(this).closest("td").prev().find("strong").text());
        var quantity = parseInt($(this).val());
        $(this).closest("td").next().find("strong").text((price * quantity).toFixed(2));
    });

});