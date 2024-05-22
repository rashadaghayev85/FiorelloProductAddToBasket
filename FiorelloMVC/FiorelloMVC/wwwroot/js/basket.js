$(function () {
    
    $(document).on("click", "#products .add-basket", function () {
        let id = parseInt($(this).attr("data-id"))
         
        $.ajax({
            type: "POST",
            url: `home/addproducttobasket?id=${id}`,
            success: function (response) {
                $(".rounded-circle").text(response.count);
                $(".rounded-circle").next().text(`CART($${response.totalPrice})`)
            }
        });
    })


    $(document).on("click", ".clearfix .remove", function () {
        let id = parseInt($(this).attr("data-id"))
         $.ajax({
            type: "POST",
            url: `cart/delete?id=${id}`,
            success: function (response) {
                $(".rounded-circle").text(response.count);
                $(".rounded-circle").next().text(`CART($${response.totalPrice})`)
            }
        });
    })

})