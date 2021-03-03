$(function () {
    if (typeof $.validator != "undefined") {
        $.validator.setDefaults({ ignore: '.NoValidation' });
    }
    CustomDatePickers();
    //CustomTimePickers();
    HelperDropDowns();
    HelperCascadingDropDowns();
    InitDialogs();
    ProductListDialog();
    InitButtons();
    CurrentDate();
    CustomDataTable();
    Deleteproduct();
    $(document).ajaxStart(function () { $("#spinner").show(); }).ajaxStop(function () { $("#spinner").hide(); });
    $('.DatePicker').attr("autocomplete", "off");
    $('select').each(function () {
        $(this).select2({
            theme: 'bootstrap4',
            width: 'style',
            placeholder: $(this).attr('placeholder'),
            allowClear: Boolean($(this).data('allow-clear'))
        });
    });
});

function CustomDataTable() {
    $("#dataTables-example").DataTable({
        responsive: true,
        "order": [0, "desc"],
        drawCallback: function () {
            $('#dataTables-example_wrapper .row:last-child').addClass('mb-1 align-products-baseline');
        }
    });
}

function CustomDatePickers() {
    $(".DatePicker").datepicker({
        todayHighlight: true,
        changeMonth: true,
        changeYear: true,
        format: 'dd-M-yyyy',
        autoclose: true,
        reset: true
    });
}

//function CustomTimePickers() {
//    $(".TimePicker").timepicki({ reset: true });
//}

function ParseDate(input) {
    var months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    var parts = input.split('-');
    return new Date(parts[2], months.indexOf(parts[1]), parts[0]);
}

function HelperDropDowns() {
    var dropdownElements = $('select.Dropdown:not(.DropdownInited)');
    $.each(dropdownElements, function (index, element) {
        var dropdownEl = $(element);
        var url = dropdownEl.attr('data-url');
        var selected = dropdownEl.attr('data-selected');
        var dataCache = dropdownEl.attr('data-cache') ? true : false;
        $.ajax({
            url: url,
            type: 'GET',
            cache: dataCache,
            success: function (jsonData, textStatus, XMLHttpRequest) {
                var Listproducts = '<option value="">--select--</option>';
                $.each(jsonData, function (i, product) {
                    if (selected && selected == product.Value) {
                        Listproducts += "<option selected='selected' value='" + product.Value + "'>" + product.Text + "</option>";
                    }
                    else {
                        Listproducts += "<option value='" + product.Value + "'>" + product.Text + "</option>";
                    }
                });
                dropdownEl.html(Listproducts).addClass("DropdownInited");
            }
        });
    });
}

function HelperCascadingDropDowns() {
    var dependentElements = $('select.Cascading:not(.DropdownInited)');
    $.each(dependentElements, function (index, element) {
        var dependentEl = $(element);
        var parentEl = $('#' + dependentEl.attr('data-parent'));
        var url = dependentEl.attr('data-url');
        var selected = dependentEl.attr('data-selected');
        var dataCache = dependentEl.attr('data-cache') ? true : false;
        var loadDropDownproducts = function () {
            if (!parentEl.val()) {
                if (selected) {
                    setTimeout(loadDropDownproducts, 300);
                }
                return;
            }
            $.ajax({
                url: url + parentEl.val(),
                type: 'GET',
                cache: dataCache,
                success: function (jsonData, textStatus, XMLHttpRequest) {
                    var Listproducts = '<option></option>';
                    $.each(jsonData, function (i, product) {
                        if (selected && selected == product.Value) {
                            Listproducts += "<option selected='selected' value='" + product.Value + "'>" + product.Text + "</option>";
                        }
                        else {
                            Listproducts += "<option value='" + product.Value + "'>" + product.Text + "</option>";
                        }
                    });
                    dependentEl.html(Listproducts).addClass("DropdownInited");
                }
            });
        };
        parentEl.change(loadDropDownproducts);
        if (selected) {
            loadDropDownproducts();
        }
    });
}

function InitDialogs() {
    $('a.Dialog:not(.DialogInited)').on("click", function () {
        var url = $(this).attr('href');
        $.ajax({
            url: url,
            type: 'GET',
            cache: false,
            success: function (responseText, textStatus) {
                var html = '' +
                    '<div class="modal fade" id="addNewForm" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">' +
                    '<div class="modal-dialog">' +
                    '<div class="modal-content">' +
                    '<div class="modal-body">' +
                    responseText +
                    '</div>' +
                    '</div>' +
                    '</div>' +
                    '</div>';
                var dialogWindow = $(html).appendTo('body');
                dialogWindow.modal({ backdrop: 'static' });
            }
        });
        return false;
    }).addClass("DialogInited");
}

function ProductListDialog() {
    $('a.ProductListDialog:not(.ProductListDialogInited)').on("click", function () {
        var url = $(this).attr('href');
        $.ajax({
            url: url,
            type: 'GET',
            cache: false,
            success: function (responseText, textStatus) {
                var html = '' +
                    '<div class="modal fade" id="addNewForm" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">' +
                    '<div class="modal-dialog">' +
                    '<div class="modal-content">' +
                    '<div class="modal-body">' +
                    responseText +
                    '</div>' +
                    '</div>' +
                    '</div>' +
                    '</div>';
                var dialogWindow = $(html).appendTo('body');
                dialogWindow.modal({ backdrop: 'static' });
            }
        });
        return false;
    }).addClass("ProductListDialogInited");
}

function ShowDialogs(url) {
    $.ajax({
        url: url,
        type: 'GET',
        cache: false,
        success: function (responseText, textStatus, XMLHttpRequest) {
            var html = '' +
                '<div class="modal fade" id="addNewForm" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">' +
                '<div class="modal-dialog">' +
                '<div class="modal-content">' +
                '<div class="modal-body">' +
                responseText +
                '</div>' +
                '</div>' +
                '</div>' +
                '</div>';
            var dialogWindow = $(html).appendTo('body');
            dialogWindow.modal({ backdrop: 'static' });
        }
    });
}

function CloseModal(el, dataAction, dataUrl) {
    if (dataAction == "formsubmit") {
        $("#" + dataUrl).submit();
    }
    else if (dataAction == "refreshparent") {
        window.parent.location = window.parent.location;
    }
    else if (dataAction == "refreshself") {
        window.location = window.location;
    }
    else if (dataAction == "redirect") {
        window.location = dataUrl;
    }
    else if (dataAction == "function") {
        eval(dataUrl);
    }
    var win = $(el).closest(".modal");
    win.modal("hide");
    setTimeout(function () {
        win.next(".modal-backdrop").remove();
        win.remove();
    }, 500);
}

function ShowResult(msg, status, dataAction, dataUrl) {
    var html = '' +
        '<div id="userUpdate" class="modal fade">' +
        '<div class="modal-dialog">' +
        '<div class="modal-content">' +
        '<div class="modal-header modal-title-' + status + '">' +
        '<h4 class="modal-title">POS</h4>' +
        '</div>' +
        '<div class="modal-body">' +
        '<p>' + msg + '</p>' +
        '</div>' +
        '<div class="modal-footer">' +
        '<button type="button" class="btn btn-primary" data-dismiss="modal" onclick="CloseModal(this, \'' + dataAction + '\',\'' + dataUrl + '\')">OK</button>' +
        '</div>' +
        '</div>' +
        '</div>' +
        '</div>';
    var dialogWindow = $(html).appendTo('body');
    dialogWindow.modal({ backdrop: 'static' });
}

function CustomAsk(msg, dataAction, dataUrl) {
    var html = '' +
        '<div id="userConfirm" class="modal fade">' +
        '<div class="modal-dialog">' +
        '<div class="modal-content">' +
        '<div class="modal-header">' +
        '<h4 class="modal-title">POS</h4>' +
        '</div>' +
        '<div class="modal-body">' +
        msg +
        '</div>' +
        '<div class="modal-footer">' +
        '<button type="button" class="btn btn-primary" onclick="CloseModal(this, \'' + dataAction + '\',\'' + dataUrl + '\')">YES</button>' +
        '<button type="button" class="btn btn-primary btn-close" onclick="CloseModal(this)">NO</button>' +
        '</div>' +
        '</div>' +
        '</div>' +
        '</div>';
    var dialogWindow = $(html).appendTo('body');
    dialogWindow.modal({ backdrop: 'static' });
}

function InitButtons() {
    $('.AddRow:not(.AddRowInited)').on("click", function () {
        var url = $(this).attr('data-url');
        var container = $(this).attr('data-container');
        $.ajax({
            url: url,
            type: 'POST',
            cache: false,
            success: function (html) {
                $("#" + container).append(html);
            }
        });
        return false;
    }).addClass("AddRowInited");

    $('.RemoveRow:not(.RemoveRowInited)').on("click", function () {
        $(this).parents("div.row:first").remove();
        return false;
    }).addClass("RemoveRowInited");
}

window.Deleteproduct = function () {
    $(".btnRemove").on('click', function () {
        var url = $(this).attr("data-url");
        CustomAsk('Are you sure to delete?', 'redirect', url);
    });
};

// Jahangir

function CheckDuplicate(productId) {
    var flag = false;
    for (var i = 0; i < $(".productId").length; i++) {
        if ($($(".productId")[i]).val() === productId) {
            flag = true;
        }
    }
    return flag;
}

function Guid() {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000)
            .toString(16)
            .substring(1);
    }
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
        s4() + '-' + s4() + s4() + s4();
}

function GetPromotionDiscountAmount(promotionalDiscounts, netamount) {
    var remainder = parseFloat(netamount) || 0;
    var times = Number(0);
    var promotionDiscount = Number(0);
    var promotionPoint = Number(0);
    promotionalDiscounts.forEach(function (result) {
        if (remainder >= result.SaleAmount) {
            times = parseInt(remainder / result.SaleAmount);
            remainder = remainder % (result.SaleAmount * times);
            promotionPoint += result.EquivalantPoint * times;
            promotionDiscount += result.DiscountAmount * times;
        }
    });
    $("#EarningPoint").val(promotionPoint.toFixed(2));
    $("#EarningPointAmount").val(promotionDiscount.toFixed(2));
}

function GetPointAmountByPoint(promotionalDiscounts, points) {
    var remainder = parseFloat(points) || 0;
    var times = Number(0);
    var pointAmount = Number(0);
    promotionalDiscounts.forEach(function (result) {
        if (remainder >= result.EquivalantPoint) {
            times = parseInt(remainder / result.EquivalantPoint);
            remainder = remainder % (result.EquivalantPoint * times);
            pointAmount += result.DiscountAmount * times;
        }
    });
    $("#ExpensePointAmount").val(pointAmount.toFixed(2));
}

function getUrlVars() {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}

function CurrentDate() {
    var mNames = new Array("Jan", "Feb", "Mar",
        "Apr", "May", "Jun", "Jul", "Aug", "Sep",
        "Oct", "Nov", "Dec");
    var d = new Date();
    var currDate = d.getDate();
    var currMonth = d.getMonth();
    var currYear = d.getFullYear();
    var currentDate = currDate + "-" + mNames[currMonth] + "-" + currYear;
    return currentDate;
}

function CheckCustomer() {
    $(".productName").on('click', function () {
        if ($("#CustomerMobileNumber").val() == "") {
            $(this).val("");
            ShowResult("Please select a customer.", "failure");
            return false;
        }
        return true;
    });
}

function CalculateSuperShopPurchaseSummary() {
    var totalQty = Number(0);
    var totalAmount = Number(0);
    for (var i = 0; i < $(".productId").length; i++) {
        totalQty += parseFloat($($(".quantity")[i]).val()) || 0;
        totalAmount += parseFloat($($(".amount")[i]).val()) || 0;
    }
    $("#TotalQuantity").val(totalQty.toFixed(4));
    $("#TotalAmount").val(totalAmount.toFixed(4));
}

function CalculateSuperShopPurchase() {
    debugger;
    $(".quantity, .purchasePrice:not(.quantityInited, .purchasePriceInited)").on('click keyup change', function () {
        var el = $(this);
        var check = Number(el.val());
        if (check.toString() == "NaN") {
            $(this).val("");
            ShowResult("Pleasse enter number", "failure");
            return false;
        }
        else {
            var productId = el.closest("tr").find(".productId").val();
            if (productId == "") {
                $(this).val(0);
                ShowResult("Pleasse select an product", "failure");
                return false;
            }
            else {
                var qty = parseFloat(el.closest("tr").find(".quantity").val()) || 0;
                var prce = parseFloat(el.closest("tr").find(".purchasePrice").val()) || 0;                
                var totalprcs = qty * prce;
                el.closest("tr").find(".amount").val(parseFloat(totalprcs).toFixed(2));
                CalculateSuperShopPurchaseSummary();
                var discount = parseFloat($("#MemoWiseDiscount").val()) || 0;
                var totalAmount = parseFloat($("#TotalAmount").val()) || 0;
                var netAmount = totalAmount - discount;
                $("#NetAmount").val(netAmount.toFixed(4));
            }
        }
        return false;
    }).addClass("quantityInited, purchasePriceInited");
}

//SuperShop Sale

function AddHTML(product) {
    var gid = Guid();
    $("#stock").text(product.ProductStock);
    $("#uom").text(product.UOMName);
    $("#price").text(product.RetailPrice);
    $("#tax").text(product.VatRate);
    var html = '<tr>' +
        '<td class="form-group">' +
        '<input type="hidden" name="SaleDetails.index" autocomplete="off" value="' + gid + '" />' +
        '<input class="productName form-control" style="width: 300px; text-align:left;" id="SaleDetails_' + gid + '_ProductName" name="SaleDetails[' + gid + '].ProductName" readonly="readonly" type="text" value="' + product.ProductName + '" />' +
        '<input class="productId" id="SaleDetails_' + gid + '_ProductId" name="SaleDetails[' + gid + '].ProductId" type="hidden" value="' + product.value + '" />' +
        '<input class="productStock" id="SaleDetails_' + gid + '_ProductStock" name="SaleDetails[' + gid + '].ProductStock" type="hidden" value="' + product.ProductStock + '" />' +
        '<input class="purchasePrice" id="SaleDetails_' + gid + '_PurchasePrice" name="SaleDetails[' + gid + '].PurchasePrice" type="hidden" value="' + product.PurchasePrice + '" />' +
        '<input class="vatRate" id="SaleDetails_' + gid + '_VatRate" name="SaleDetails[' + gid + '].VatRate" type="hidden" value="' + product.VatRate + '" />' +
        '<input class="uOMId" id="SaleDetails_' + gid + '_UOMId" name="SaleDetails[' + gid + '].UOMId" type="hidden" value="' + product.UOMId + '" />' +

        '<input id="SaleDetails_' + gid + '_ProductCode" name="SaleDetails[' + gid + '].ProductCode" type="hidden" value="' + product.ProductCode + '" />' +
        '<input id="SaleDetails_' + gid + '_ProductCategoryId" name="SaleDetails[' + gid + '].ProductCategoryId" type="hidden" value="' + product.ProductCategoryId + '" />' +
        '<input id="SaleDetails_' + gid + '_ProductSubCategoryId" name="SaleDetails[' + gid + '].ProductSubCategoryId" type="hidden" value="' + product.ProductSubCategoryId + '" />' +
        '<input id="SaleDetails_' + gid + '_ProductSubsidiaryCategoryId" name="SaleDetails[' + gid + '].ProductSubsidiaryCategoryId" type="hidden" value="' + product.ProductSubsidiaryCategoryId + '" />' +
        '<input id="SaleDetails_' + gid + '_SupplierId" name="SaleDetails[' + gid + '].SupplierId" type="hidden" value="' + product.GradeId + '" />' +         
        '</td> ' +
        '<td class="form-group" style="display: inline-flex;margin: 0;text-align:center">' +
        '<a href="javascript:void(0)" class="decrease"  style="padding: 0 5px;font-size: 22px;font-weight: 900;" onclick="Decrease(this);">-</a>' +
        '<input class="quantity form-control" id="SaleDetails_' + gid + '_Quantity" name="SaleDetails[' + gid + '].Quantity"  type="text" value="0" style="text-align:center;width: 80px;" onclick="this.select()" onchange="QuantityChange(this);" />' +
        '<a href="javascript:void(0)" class="increase" style="padding: 0 5px;font-size: 22px;font-weight: 900;" onclick="Increase(this);">+</a>' +
        '</td> ' +
        '<td class="form-group">' +
        '<input class="salePrice form-control" id="SaleDetails_' + gid + '_SalePrice" name="SaleDetails[' + gid + '].SalePrice" type="number" value="' + product.RetailPrice + '" style="text-align:right;" onclick="this.select()" onchange="PriceChange(this);" />' +
        '</td>' +
        '<td class="form-group">' +
        '<input  class="discountPerUnit form-control" id="SaleDetails_' + gid + '_DiscountPerUnit" name="SaleDetails[' + gid + '].DiscountPerUnit"   type="text"  value="' + product.MaxDiscount + '" style="text-align:right;" onclick="this.select()" onchange="CalculateDiscountAmount(this);" />' +
        '<input class="discountAmount" id="SaleDetails_' + gid + '_DiscountAmount" name="SaleDetails[' + gid + '].DiscountAmount" type="hidden" value="0" />' +
        '<input class="discountAmount" id="SaleDetails_' + gid + '_MaxDiscount" name="SaleDetails[' + gid + '].MaxDiscount" type="hidden" value="' + product.MaxDiscount + '" />' +
        '</td>' +
        '<td class="form-group">' +
        '<input  class="discountInPercentage form-control" id="SaleDetails_' + gid + '_DiscountInPercentage" name="SaleDetails[' + gid + '].DiscountInPercentage"  type="text" style="text-align:right;" value="0.00" onclick="this.select()" onchange="CalculateDiscountInAmount(this);"   />' +
        '<input class="discountInAmount" id="SaleDetails_' + gid + '_DiscountInAmount" name="SaleDetails[' + gid + '].DiscountInAmount" type="hidden" value="0" />' +
        '</td>' +
        '<td class="form-group">' +
        '<input class="amount form-control" style="text-align:right;" id="SaleDetails_' + gid + '_TotalAmount" name="SaleDetails[' + gid + '].TotalAmount" readonly="readonly" type="text" style="text-align:right;"  value="' + product.RetailPrice + '"  />' +
        '</td>' +
        '<td class="form-group" style="text-align: center;">' +
        '<a onclick="return DeleteRow(this)" href="#" class="btn-close"><i class="fa fa-trash"></i></a>' +
        '</td>' +
        '</tr>';
    $("#editorItemRows").append(html);
}

function CalculateSuperShopSaleSummary() {
    var totalQty = Number(0);
    var totalDiscountAmount = Number(0);
    var totalDiscountInAmount = Number(0);
    var totalAmount = Number(0);
    var totalVat = Number(0);
    for (var i = 0; i < $(".productId").length; i++) {
        totalQty += parseFloat($($(".quantity")[i]).val()) || 0;
        totalDiscountAmount += parseFloat($($(".discountAmount")[i]).val()) || 0;
        totalDiscountInAmount += parseFloat($($(".discountInAmount")[i]).val()) || 0;
        totalAmount += parseFloat($($(".amount")[i]).val()) || 0;
        var vatRate = parseFloat($($(".vatRate")[i]).val()) || 0;
        var vatAmount = ((vatRate * totalAmount) / 100);
        totalVat += parseFloat(vatAmount);
    }
    $("#TotalQuantity").val(totalQty.toFixed(4));
    $("#TotalAmount").val(totalAmount.toFixed(4));
    $("#ProductDiscount").val(parseFloat(totalDiscountAmount + totalDiscountInAmount).toFixed(4));
    $("#TotalVat").val(totalVat.toFixed(4));

    var cusDisRate = parseFloat($("#CustomerDiscountInPercentage").val()) || 0;
    var cusDisAmount = parseFloat((totalAmount * cusDisRate) / 100) || 0;
    $("#CustomerDiscountInAmount").val(cusDisAmount);

    var ovarallDiscount = parseFloat($("#OverAllDiscount").val()) || 0;
    var totalPlus = totalVat;
    var totalMinus = totalDiscountAmount + totalDiscountInAmount + ovarallDiscount + cusDisAmount;
    var netAmount = parseFloat(totalAmount - totalMinus + totalPlus) || 0;
    if (typeof promotionalDiscountList != 'undefined') {
        var promotionalDiscounts = promotionalDiscountList.sort((a, b) => parseInt(b.SaleAmount) - parseInt(a.SaleAmount));
        GetPromotionDiscountAmount(promotionalDiscounts, netAmount);
    }

    $("#NetAmount").val(netAmount.toFixed(4));

    $("h3").text(parseInt(netAmount.toFixed(0)));

    var paidAmt = parseFloat($("#PaidAmount").val()) || 0;

    var changeAmt = paidAmt - netAmount;
    var dueAmt = netAmount - paidAmt;


    if (changeAmt > 0) {
        $("#ChangeAmount").val((changeAmt).toFixed(2));
    } else {
        $("#ChangeAmount").val(0);
    }

    if (paidAmt >= netAmount) {
        $("#DueAmount").val(0);
    } else {
        $("#DueAmount").val((dueAmt).toFixed(2));
    }
}

function CalculateMobileShopSaleSummary() {
    var totalQty = Number(0);
    var totalDiscountAmount = Number(0);
    var totalDiscountInAmount = Number(0);
    var totalAmount = Number(0);
    var totalVat = Number(0);
    for (var i = 0; i < $(".productId").length; i++) {
        totalQty += parseFloat($($(".quantity")[i]).val()) || 0;
        totalDiscountAmount += parseFloat($($(".discountAmount")[i]).val()) || 0;
        totalDiscountInAmount += parseFloat($($(".discountInAmount")[i]).val()) || 0;
        totalAmount += parseFloat($($(".amount")[i]).val()) || 0;
        var vatRate = parseFloat($($(".vatRate")[i]).val()) || 0;
        var vatAmount = ((vatRate * totalAmount) / 100);
        totalVat += parseFloat(vatAmount);
    }
    $("#TotalQuantity").val(totalQty.toFixed(4));
    $("#TotalAmount").val(totalAmount.toFixed(4));
    $("#ProductDiscount").val(parseFloat(totalDiscountAmount + totalDiscountInAmount).toFixed(4));
    $("#TotalVat").val(totalVat.toFixed(4));

    var cusDisRate = parseFloat($("#CustomerDiscountInPercentage").val()) || 0;
    var cusDisAmount = parseFloat((totalAmount * cusDisRate) / 100) || 0;
    $("#CustomerDiscountInAmount").val(cusDisAmount);

    var ovarallDiscount = parseFloat($("#OverAllDiscount").val()) || 0;
    var totalPlus = totalVat;
    var totalMinus = totalDiscountAmount + totalDiscountInAmount + ovarallDiscount + cusDisAmount;
    var netAmount = parseFloat(totalAmount - totalMinus + totalPlus) || 0;
    if (typeof promotionalDiscountList != 'undefined') {
        var promotionalDiscounts = promotionalDiscountList.sort((a, b) => parseInt(b.SaleAmount) - parseInt(a.SaleAmount));
        GetPromotionDiscountAmount(promotionalDiscounts, netAmount);
    }

    $("#NetAmount").val(netAmount.toFixed(4));
    $("h3").text(netAmount.toFixed(0));

    var paidAmt = parseFloat($("#PaidAmount").val()) || 0;

    var changeAmt = paidAmt - netAmount;
    var dueAmt = netAmount - paidAmt;


    if (changeAmt > 0) {
        $("#ChangeAmount").val((changeAmt).toFixed(4));
    } else {
        $("#ChangeAmount").val(0);
    }

    if (paidAmt >= netAmount) {
        $("#DueAmount").val(0);
    } else {
        $("#DueAmount").val((dueAmt).toFixed(4));
    }
}

function Increase(el) {
    var productId = $(el).closest("tr").find("input.productId").val();
    var product = productList.find(function (product) { return product.value === productId; });
    $("#uom").text(product.UOMName);
    $("#stock").text(product.ProductStock);
    $("#vatRate").text(product.VatRate);
    $("#price").text(product.RetailPrice);
    var qtryField = $(el).closest("tr").find("input.quantity");
    var qty = qtryField.val();
    qty = parseFloat(qty) + 1;
    var stock = parseFloat(product.ProductStock);
    if (qty > stock) {
        ShowResult("Product out of stock.", "failure");
        qtryField.val(qty - 1);
    }
    else
        qtryField.val(qty.toFixed(4));
    var priceField = $(el).closest("tr").find("input.salePrice");
    var unitPrice = parseFloat(priceField.val()) || 0;
    var amountField = $(el).closest("tr").find("input.amount");
    CalculateAmount(amountField, qty, unitPrice);
    qty = 0;
    CalculateDiscountInAmount($(el).closest("tr").find("input.discountInPercentage"));
}

function Decrease(el) {
    var productId = $(el).closest("tr").find("input.productId").val();
    var product = productList.find(function (product) { return product.value === productId; });
    $("#uom").text(product.UOMName);
    $("#stock").text(product.ProductStock);
    $("#vatRate").text(product.VatRate);
    $("#price").text(product.RetailPrice);
    var qtryField = $(el).closest("tr").find("input.quantity");
    var qty = qtryField.val();
    qty = parseFloat(qty) - 1;
    if (qty < 0) {
        qty = 0;
    }
    qtryField.val(qty.toFixed(4));
    var priceField = $(el).closest("tr").find("input.salePrice");
    var unitPrice = parseFloat(priceField.val()) || 0;
    var amountField = $(el).closest("tr").find("input.amount");
    CalculateAmount(amountField, qty, unitPrice);
    qty = 0;
    CalculateDiscountInAmount($(el).closest("tr").find("input.discountInPercentage"));
}

function CalculateAmount(el, qty, unitPrice) {
    var amount = (qty * unitPrice);
    $(el).val(amount.toFixed(4));
}

function CalculateDiscountAmount(el) {
    var discount = parseFloat($(el).val()) || 0;
    var quantity = parseFloat($(el).closest("tr").find("input.quantity").val()) || 0;
    var discountAmount = parseFloat(quantity * discount) || 0;
    $(el).closest("tr").find("input.discountAmount").val(discountAmount);
    CalculateSuperShopSaleSummary();
}

function CalculateDiscountInAmount(el) {
    var discount = parseFloat($(el).val()) || 0;
    var amount = parseFloat($(el).closest("tr").find("input.amount").val()) || 0;
    var discountAmount = parseFloat(((parseFloat(amount) * parseFloat(discount)) / 100)) || 0;
    $(el).closest("tr").find("input.discountInAmount").val(discountAmount);
    CalculateSuperShopSaleSummary();
}

function DeleteRow(lnk) {
    if (typeof lnk != 'undefined') {
        var row = lnk.parentNode.parentNode;
        var rowIndex = row.rowIndex - 1;
        var myTableee = document.getElementById('tbl').tBodies[0];
        var tabRowCount = myTableee.rows.length;
        if (tabRowCount > 1) {
            var myTable = document.getElementById('tbl');
            myTable.deleteRow(rowIndex + 1);
            CalculateSuperShopSaleSummary();
        }
        else {
            ShowResult("At least one row needed!", "failure");
        }
    }
}

function QuantityChange(el) {
    
    var product = window.productList.find(function (product) { return product.value === $(el).closest("tr").find("input.productId").val(); });
    $("#uom").text(product.UOMName);
    $("#stock").text(product.ProductStock);
    $("#vatRate").text(product.VatRate);
    $("#price").text(product.RetailPrice);
    var qtryField = $(el).closest("tr").find("input.quantity");
    var qty = parseFloat(qtryField.val());
    qty = isNaN(qty) ? 0 : qty;
    if (qty == 0) {
        $(el).closest("tr").find("input.quantity").val(1);
        ShowResult("Give a valid number.", "failure");
    }
    var stock = parseFloat($(el).closest("tr").find("input.productStock").val());
    stock = isNaN(stock) ? 0 : stock;
    $("#stock").text(stock);
    if (qty > stock) {
        ShowResult("Product out of stock.", "failure");
        qtryField.val(stock);
    }
    var priceField = $(el).closest("tr").find("input.salePrice");
    var unitPrice = parseFloat(priceField.val()) || 0;
    $("#price").text(unitPrice);
    var amountField = $(el).closest("tr").find("input.amount");
    CalculateAmount(amountField, qty, unitPrice);
    qty = 0;
    CalculateDiscountInAmount($(el).closest("tr").find("input.discountInPercentage"));
}

function PriceChange(el) {
    var product = window.productList.find(function (product) { return product.value === $(el).closest("tr").find("input.productId").val(); });
    $("#uom").text(product.UOMName);
    $("#stock").text(product.ProductStock);
    $("#vatRate").text(product.VatRate);
    $("#price").text(product.RetailPrice);
    var qtryField = $(el).closest("tr").find("input.quantity");
    var qty = parseFloat(qtryField.val());
    qty = isNaN(qty) ? 0 : qty;
    if (qty == 0) {
        $(el).closest("tr").find("input.quantity").val(1);
        ShowResult("Give a valid number.", "failure");
    }
    var stock = parseFloat($(el).closest("tr").find("input.productStock").val());
    stock = isNaN(stock) ? 0 : stock;
    $("#stock").text(stock);
    if (qty > stock) {
        ShowResult("Product out of stock.", "failure");
        qtryField.val(stock);
    }
    var priceField = $(el).closest("tr").find("input.salePrice");
    var unitPrice = parseFloat(priceField.val()) || 0;
    $("#price").text(unitPrice);
    var amountField = $(el).closest("tr").find("input.amount");
    CalculateAmount(amountField, qty, unitPrice);
    qty = 0;
    CalculateDiscountInAmount($(el).closest("tr").find("input.discountInPercentage"));
}
//

function CalculateMobileShopPurchaseSummary() {
    var totalQty = Number(0);
    var totalPrice = Number(0);
    var totalAmount = Number(0);
    for (var i = 0; i < $(".productId").length; i++) {
        totalQty += parseFloat($($(".quantity")[i]).val()) || 0;
        totalPrice += parseFloat($($(".purchasePrice")[i]).val()) || 0;
        totalAmount += parseFloat($($(".amount")[i]).val()) || 0;
    }
    $("#TotalQuantity").val(totalQty.toFixed(4));
    $("#TotalPrice").val(totalPrice);
    $("#TotalAmount").val(parseFloat(totalAmount).toFixed(4));
}

function CalculateMobileShopPurchase() {   
    $(".quantity, .purchasePrice:not(.quantityInited, .purchasePriceInited)").on('click keyup change', function () {
        var el = $(this);
        var check = Number(el.val());
        if (check.toString() == "NaN") {
            $(this).val("");
            ShowResult("Pleasse enter number", "failure");
            return false;
        }
        else {
            var productId = el.closest("tr").find(".productId").val();
            if (productId == "") {
                $(this).val(0);
                ShowResult("Pleasse select an product", "failure");
                return false;
            }
            else {
                var qty = parseFloat(el.closest("tr").find(".quantity").val()) || 0;
                var prce = parseFloat(el.closest("tr").find(".purchasePrice").val()) || 0;
                var totalprcs = qty * prce;
                el.closest("tr").find(".amount").val(parseFloat(totalprcs).toFixed(4));
                CalculateMobileShopPurchaseSummary();

                var discount = parseFloat($("#MemoWiseDiscount").val()) || 0;
                var totalAmount = parseFloat($("#TotalAmount").val()) || 0;
                var netAmount = totalAmount - discount;
                $("#NetAmount").val(netAmount.toFixed(4));
            }
        }
    }).addClass("quantityInited, purchasePriceInited");
}

function CalculateSuperShopSaleReturnAmount() {
    var totalAmount = 0;
    var totalQty = 0;

    for (var i = 0; i < $(".returnQuantity").length; i++) {
        totalQty += parseFloat($($(".returnQuantity")[i]).val()) || 0;
        totalAmount += parseFloat($($(".returnAmount")[i]).val()) || 0;
    }
    $("#TotalQuantity").val(totalQty.toFixed(4));
    $("#TotalAmount").val(totalAmount.toFixed(4));     
}
//function CalculateMobileShopSaleSummary() {
//    var totalQty = Number(0);
//    var totalDiscount = Number(0);
//    var totalAmount = Number(0);
//    var totalVat = Number(0);
//    for (var i = 0; i < $(".productId").length; i++) {
//        totalQty += parseFloat($($(".quantity")[i]).val()) || 0;
//        totalDiscount += parseFloat($($(".discountInAmount")[i]).val()) || 0;
//        totalAmount += parseFloat($($(".amount")[i]).val()) || 0;
//        var vatRate = parseFloat($($(".vatRate")[i]).val()) || 0;
//        var vatAmount = vatRate * totalAmount / 100;
//        totalVat += parseFloat(vatAmount);
//    }
//    $("#TotalQuantity").val(totalQty);
//    $("#TotalAmount").val(totalAmount.toFixed(2));
//    $("#TotalDiscount").val(totalDiscount.toFixed(2));
//    $("#TotalVat").val(totalVat.toFixed(2));

//    var cusDisRate = parseFloat($("#CustomerDiscountInPercentage").val()) || 0;

//    var cusDisAmount = ((totalAmount * cusDisRate) / 100);

//    $("#CustomerDiscountInAmount").val(cusDisAmount);

//    var ovarallDiscount = parseFloat($("#OverAllDiscount").val()) || 0;

//    var totalPlus = totalVat;
//    var totalMinus = totalDiscount + ovarallDiscount + cusDisAmount;
//    var netAmount = (totalAmount - totalMinus + totalPlus).toFixed(2);


//    $("#NetAmount").val(netAmount);
//    $("h3").text(netAmount);

//    var paidAmt = parseFloat($("#PaidAmount").val()) || 0;

//    var changeAmt = paidAmt - netAmount;
//    var dueAmt = netAmount - paidAmt;


//    if (changeAmt > 0) {
//        $("#ChangeAmount").val((changeAmt).toFixed(2));
//    } else {
//        $("#ChangeAmount").val(0);
//    }

//    if (paidAmt >= netAmount) {
//        $("#DueAmount").val(0);
//    } else {
//        $("#DueAmount").val((dueAmt).toFixed(2));
//    }
//}

function CalculateMobileShopSale() {
    
    $(".quantity, .purchasePrice:not(.quantityInited, .purchasePriceInited)").on('click keyup change', function () {
        var el = $(this);
        var check = Number(el.val());
        if (check.toString() == "NaN") {
            $(this).val("");
            ShowResult("Pleasse enter number", "failure");
            return false;
        }
        else {
            var productId = el.closest("tr").find(".productId").val();
            if (productId == "") {
                $(this).val(0);
                ShowResult("Pleasse select an product", "failure");
                return false;
            }
            else {
                var qty = parseFloat(el.closest("tr").find(".quantity").val()) || 0;
                var prce = parseFloat(el.closest("tr").find(".purchasePrice").val()) || 0;
                var totalprcs = qty * prce;
                el.closest("tr").find(".amount").val(parseFloat(totalprcs).toFixed(2));
                CalculateMobileShopSaleSummary();

                var discount = parseFloat($("#MemoWiseDiscount").val()) || 0;
                var totalAmount = parseFloat($("#TotalAmount").val()) || 0;
                var netAmount = totalAmount - discount;
                $("#NetAmount").val(netAmount.toFixed(4));
            }
        }
    }).addClass("quantityInited, purchasePriceInited");
}

window.MemoWiseDiscount = function () {
    var discount = parseFloat($("#MemoWiseDiscount").val());

    var totalAmount = parseFloat($("#TotalAmount").val());
    var netAmount = parseFloat(totalAmount - discount);

    $("#NetAmount").val(netAmount.toFixed(4));
};


window.DirectRdlcReportPrint = function (Id, companyId, branchId) {
    var xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function () {
        if (xhr.readyState == XMLHttpRequest.DONE) {
            //location.href = "/SuperShopSale/";
        }
    }
    xhr.open('GET', 'http://localhost:5000?Id=' + Id + '&companyId=' + companyId + '&branchId=' + branchId, true);
    xhr.send(null);
}