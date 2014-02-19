
/* Additional Functions */
jQuery.fn.extend({
    unLazy: function () {
        this.on("click", function () {
            forceResize();
        })
    }
});


jQuery.fn.extend({
    ActionPanel: function () {
        this.on("click", function (event) {
            event.preventDefault();

            var formname = $(this).attr("data-panelregion");

            // Clear the action-panel area
            $(".b-rightside-panel .panel-content").empty();

            // Fill with the correct form
            var form = $("#action-forms").find("#" + formname).clone();

            // Change the fieldset class to it's id
            var fld = form.find("fieldset");
            fld.attr("id", fld.attr("class"));

            $(".b-rightside-panel .panel-content").append(form);
            $(".b-rightside-panel .panel-content .text-prefill").TextPrefill();
            $(".b-rightside-panel .panel-content .validator").ValidateAndSubmit();
            $(".b-rightside-panel .panel-content .error, .b-rightside-panel .panel-content .complete").on("click", function () {
                $(this).hide("fade");
            });


            $(".b-rightside-panel .panel-content #btn-login").LoginForm();
            $(".b-rightside-panel .panel-content #btn-recover-password").LoginRecoverPassword();
            $(".b-rightside-panel .panel-content a.forgotten-password").on("click", function (event) {
                event.preventDefault();
                $(this).hide("fade", { complete: function () {
                    $(".b-rightside-panel .panel-content .form-password-recover").show("fade");
                }
                });
            });

            $("#btn-change-password").ChangePassword();

            $(".account-add-user").on("click", function (event) {
                event.preventDefault();
                if ($(this).hasClass("clicked")) {
                    $(this).removeClass("clicked").find("span").removeClass("icon-minus-sign").addClass("icon-plus-sign");
                    $(this).parents("fieldset").find(".form-expand").slideUp(500);
                    var h = $(".b-rightside-panel").height() - $(".b-rightside-panel .close").outerHeight() - $(".b-rightside-panel .panel-content h1").outerHeight() - $(".b-rightside-panel .panel-content p").outerHeight() - $(".b-rightside-panel .panel-content a.account-add-user").outerHeight() - $(".b-rightside-panel .panel-content h2.account-users").outerHeight() - 50;
                    $("ul#account-users").height(h);
                }
                else {
                    $(this).addClass("clicked").find("span").removeClass("icon-plus-sign").addClass("icon-minus-sign");
                    $(this).parents("fieldset").find(".form-expand").slideDown(300, function () {
                        var h = $(".b-rightside-panel").height() - $("#form-users .form-expand").outerHeight() - $(".b-rightside-panel .close").outerHeight() - $(".b-rightside-panel .panel-content h1").outerHeight() - $(".b-rightside-panel .panel-content p").outerHeight() - $(".b-rightside-panel .panel-content a.account-add-user").outerHeight() - $(".b-rightside-panel .panel-content h2.account-users").outerHeight() - 50;
                        $("ul#account-users").height(h);
                    });

                }
            });

            $('.close-login-open-contact').on("click", function (event) {
                event.preventDefault();
                var el = $(this);
                $('.b-rightside-panel').hide("fade", { complete: function () {
                        el.attr("data-panel", "true").attr("data-panelregion", "action-contact-form").unbind("click").ActionPanel().trigger("click");
                    } 
                });
            })


            $("ul#account-users").VUIUsers();
            $("#btn-add-user").AddVUIUser();

            // Open the action panel
            $(".b-rightside-panel").show("fade", 1000);
        });

        return this.each(function () { });
    }
});

jQuery.fn.extend({
    VUIUsers: function () {
        var el = $(this);
        el.empty();
        var xmlActionsURL = '/vui/vui-xml-actions/';
        $.ajax({
            url: xmlActionsURL + "?a=vus",
            type: "POST",
            dataType: 'json',
            success: function (data) {
                if (data.response == "valid") {
                    if (data.count > 0) {
                        for (var i = 0; i < data.data.length; i++) {
                            el.append("<li><a title=\"Delete this user\"class=\"account-user-delete\" href=\"#\" data-id=\"" + data.data[i].emailaddress + "\"><span class=\"icon-minus-sign\"></span></a> " + data.data[i].emailaddress + "</li>");
                        }
                        $("h2.account-users").show();
                        el.find("a.account-user-delete").DeleteVUIUser();

                        // Height of rhpanel - minus padding
                        var h = $(".b-rightside-panel").height() - $(".b-rightside-panel .close").outerHeight() - $(".b-rightside-panel .panel-content h1").outerHeight() - $(".b-rightside-panel .panel-content p").outerHeight() - $(".b-rightside-panel .panel-content a.account-add-user").outerHeight() - $(".b-rightside-panel .panel-content h2.account-users").outerHeight() - 50;

                        // console.log("Avalable height:" + h);
                        el.height(h);
                    }
                }
            }
        });

        return this.each(function () { });
    }
});

jQuery.fn.extend({
    DeleteVUIUser: function () {

        this.on("click", function (event) {
            var el = $(this);
            event.preventDefault();
            var xmlActionsURL = '/vui/vui-xml-actions/';
            var DataObject = { "id": $(this).attr("data-id") };
            $.ajax({
                url: xmlActionsURL + "?a=vud",
                type: "POST",
                dataType: 'json',
                data: DataObject,
                success: function (data) {
                    if (data.response == "valid") {
                        el.parent().remove();
                    }
                }
            });
        });
    }
});

function bindEnter(panel, button) {
    panel.keyup(function (event) {
        var code = event.which;
        if (code == 13) {
            event.preventDefault();
            button.trigger("click");
        }
    });
}

jQuery.fn.extend({
    AddVUIUser: function () {
        var fld = $(this).parents("#account-user-form");

        fld.find("#user-email").ValidateEmail();

        bindEnter(fld, $("#btn-add-user"));

        this.on("click", function (event) {
            var el = $(this);
            el.attr("disabled", "disabled");
            if (fld.find("#user-first").val() != "" && fld.find("#user-last").val() != "" && fld.find("#user-email").val() != "" && fld.find(".invalid").length == 0) {
                var xmlActionsURL = '/vui/vui-xml-actions/';
                var DataObject = { "first": fld.find("#user-first").val(), "last": fld.find("#user-last").val(), "email": fld.find("#user-email").val() };
                $.ajax({
                    url: xmlActionsURL + "?a=vua",
                    type: "POST",
                    dataType: 'json',
                    data: DataObject,
                    success: function (data) {
                        //// console.log("Logging in: " + data.response + "; " + data.data)
                        if (data.response == "valid") {
                            fld.find("ul#account-users").append("<li><a title=\"Delete this user\"class=\"account-user-delete\" href=\"#\" data-id=\"" + fld.find("#user-email").val() + "\"><span class=\"icon-minus-sign\"></span></a> " + fld.find("#user-email").val() + "</li>");
                            fld.find("a.account-user-delete[data-id='" + fld.find("#user-email").val() + "']").DeleteVUIUser();
                            fld.find("#user-first").val("");
                            fld.find("#user-last").val("");
                            fld.find("#user-email").val("");
                        }
                        else {
                            $(fld).find(".data-error").addClass("form-error", {
                                complete: function () {
                                    $(this).find("span.message").html(data.data);
                                    $(this).show();
                                    $(this).click(function () { $(this).hide(); });
                                    setTimeout(function () { $(fld).find(".form-error").hide("fade"); }, 3500);
                                }
                            });
                        }
                        el.removeAttr("disabled");
                    }
                });
            }
            else {
                $(fld).find(".error").addClass("form-error", {
                    complete: function () {
                        $(this).show();
                        $(this).click(function () { $(this).hide(); });
                        setTimeout(function () { $(fld).find(".error").hide("fade"); }, 3500);
                    }
                });
                el.removeAttr("disabled");
            }

        });
        return this.each(function () { });
    }
});



jQuery.fn.extend({
    ChangePassword: function (options) {
        var settings = $.extend({
            password1: "#change-password-1",
            password2: "#change-password-2"
        }, options);

        var re = /(?=.*\d)(?=.*[A-z])(?=.*[^A-z\d])/;

        var fld = $(this).parents("fieldset");

        bindEnter(fld, $("#btn-change-password"));

        $(settings.password1).on("blur", function () {
            var val1 = $(this).val();

            if (val1.length < 6) {
                $(this).addClass("invalid");
                submit = false;
            }
            else if (val1.length > 20) {
                $(this).addClass("invalid");
                submit = false;
            }
            else if (!re.test(val1)) {
                $(this).addClass("invalid");
                submit = false;
            }
            else {
                $(this).removeClass("invalid");
            }
        });
        $(settings.password2).on("blur", function () {
            var val1 = $(settings.password1).val();
            var val2 = $(settings.password2).val();

            if (val1 != val2) {
                $(this).addClass("invalid");
                submit = false;
            }
            else {
                $(this).removeClass("invalid");
            }
        });

        this.on("click", function () {

            // console.log("Here");

            var submit = true;
            var val1 = $(settings.password1).val();
            var val2 = $(settings.password2).val();
            if (val1.length < 6) {
                $(settings.password1).addClass("invalid");
                submit = false;
            }
            else if (val1.length > 20) {
                $(settings.password1).addClass("invalid");
                submit = false;
            }
            else if (!re.test(val1)) {
                $(settings.password1).addClass("invalid");
                submit = false;
            }
            else {
                $(settings.password1).removeClass("invalid");
            }
            if (val1 != val2) {
                $(settings.password2).addClass("invalid");
                submit = false;
            }
            else {
                $(settings.password2).removeClass("invalid");
            }

            // console.log("Submitting: " + submit);

            if (submit) {
                var xmlActionsURL = '/vui/vui-xml-actions/';
                var DataObject = { "pwd": val1 };
                $.ajax({
                    url: xmlActionsURL + "?a=cp",
                    type: "POST",
                    dataType: 'json',
                    data: DataObject,
                    success: function (data) {
                        if (data.response == "valid") {
                            $(fld).find(".complete").addClass("form-complete", {
                                complete: function () {
                                    $(this).show();
                                    setTimeout(function () { $(".b-rightside-panel").hide("fade"); }, 3000);
                                }
                            });
                        }
                        else {
                            $(fld).find(".error").addClass("form-error", {
                                complete: function () {
                                    $(this).show();
                                    setTimeout(function () { $(fld).find(".error").hide("fade"); }, 4000);
                                }
                            });
                        }
                    }
                });
            }
            else {
                $(fld).find(".error").addClass("form-error", {
                    complete: function () {
                        $(this).show();
                        setTimeout(function () { $(fld).find(".error").hide("fade"); }, 4000);
                    }
                });
            }
        });
        return this.each(function () { });
    }
});




jQuery.fn.extend({
    LoginForm: function () {
        var fld = $(this).parents("fieldset");

        bindEnter(fld, $("#btn-login"));

        //fld.find("#username").ValidateEmail();

        this.on("click", function () {
            if ($("#username").val() != "" && $("#password").val() != "" && fld.find(".invalid").length == 0) {
                var xmlActionsURL = '/vui/vui-xml-actions/';
                var remname = "N";
                if ($("#rememberUserName").is(":checked")) {
                    remname = "Y"
                }
                var DataObject = { "user": $("#username").val(), "pass": $("#password").val(), "rem": remname };
                $.ajax({
                    url: xmlActionsURL + "?a=lo",
                    type: "POST",
                    dataType: 'json',
                    data: DataObject,
                    success: function (data) {
                        //// console.log("Logging in: " + data.response + "; " + data.data)
                        if (data.response == "valid") {
                            document.location.href = data.data;
                        }
                        else {
                            $(fld).find(".error").addClass("form-error", {
                                complete: function () {
                                    $(this).show();
                                    setTimeout(function () { $(fld).find(".error").hide("fade"); }, 3500);
                                }
                            });
                        }
                    }
                });
            }
            else {
                $(fld).find(".error").addClass("form-error", {
                    complete: function () {
                        $(this).show();
                        setTimeout(function () { $(fld).find(".error").hide("fade"); }, 3500);
                    }
                });
            }

        });
        return this.each(function () { });
    }
});

jQuery.fn.extend({
    LoginRecoverPassword: function () {
        var fld = $(this).parents("fieldset");


        bindEnter(fld, $("#btn-recover-password"));

        fld.find("#recover-email").ValidateEmail();

        this.on("click", function () {
            var xmlActionsURL = '/vui/vui-xml-actions/';
            var fld = $(this).parents("fieldset");
            var emailAddress = $(fld).find("#recover-email");

            if ($(emailAddress).val() != "" && !$(emailAddress).hasClass("invalid")) {
                var DataObject = { "email": $("#recover-email").val() };
                $.ajax({
                    url: xmlActionsURL + "?a=pr",
                    type: "POST",
                    dataType: 'json',
                    data: DataObject,
                    success: function (data) {
                        //// console.log("Logging in: " + data.response + "; " + data.data)
                        if (data.response == "valid") {
                            $(fld).find(".complete").addClass("form-complete").show("fade");
                        }
                    }
                });
            } 
            else {
                $(fld).find(".error").addClass("form-error", {
                    complete: function () {
                        $(this).show();
                        setTimeout(function () { $(fld).find(".error").hide("fade"); }, 3500);
                    }
                });
            }
        });
        return this.each(function () { });
    }
});

jQuery.fn.extend({
    Logout: function () {
        this.each(function () {
            $(this).on("click", function (event) {
                event.preventDefault();
                var xmlActionsURL = '/vui/vui-xml-actions/';

                var DataObject = {};
                $.ajax({
                    url: xmlActionsURL + "?a=lu",
                    type: "POST",
                    dataType: 'json',
                    data: DataObject,
                    success: function (data) {
                        document.location.href = data.data;
                    }
                });
            });
        });
    }
});



jQuery.fn.extend({
    QuietFieldSubmit: function (options) {
        var settings = $.extend( {
            action : "sud"
        } ,options);
        this.on("blur", function (event) {

            if ($(this).val().trim() == '') {
                $(this).val($(this).attr("data-initval"));
            }
            if ($(this).val().trim() !== $(this).attr("data-initval")) {
                if($(this).val().trim() !== $(this).attr("data-dirtyval")) {
                    $(this).addClass("input-loading");
                    var xmlActionsURL = '/vui/vui-xml-actions/';
                    var el = $(this);
                    var val = el.val();
                    var DataObject = { "field": el.attr("data-field"), "data": val };
                    $.ajax({
                        url: xmlActionsURL + "?a=" + settings.action,
                        type: "POST",
                        dataType: 'json',
                        data: DataObject,
                        success: function (data) {
                            el.attr("data-initval", val);
                            el.removeClass("input-loading");
                        },
                        error: function (data) {
                            el.removeClass("input-loading");
                            el.addClass("input-error");
                        }
                    });
                }
            }
            
        });
        return this.each(function () {

        });
    }
});


jQuery.fn.extend({
    TextPrefill: function () {
        this.each(function () {
            if ($(this).attr("value") == "") {
                $(this).attr("value", $(this).attr("data-initval"));
            }
            else {
                $(this).val($(this).attr("value"));
            }
            $(this).on("focus", function () {
                if ($(this).val() == $(this).attr("placeholder")) {
                    $(this).val("");
                }
            });
            $(this).on("blur", function () {
                if ($(this).val().trim() == '') {
                    $(this).val($(this).attr("placeholder"));
                }
                else {
                    $(this).removeClass("invalid");
                }
            });
        });
        return this.each(function () { });
    }
});


jQuery.fn.extend({
    ValidateEmail: function() {
        this.each(function() {
            $(this).on("blur", function () {
                var email = $(this).val();
                email = email.replace(/'/g, "");
                if(!isValidEmailAddress(email)) {
                    $(this).addClass("invalid");
                }
                else {
                    $(this).removeClass("invalid");
                }
            });            
        }); 
        return this.each(function () { });
    }
});



jQuery.fn.extend({
    ValidateAndSubmit: function () {
        this.each(function () {

            var fld = $(this).parents("fieldset");
            bindEnter(fld, $(this));

            $(this).on("click", function () {
                var group = $(this).attr("data-groupname");
                $(".required-field[data-group='" + group + "']").each(function () {
                    if ($(this).hasClass("text-prefill")) {
                        if ($(this).val() == $(this).attr("data-initval")) {
                            $(this).addClass("invalid");
                        }
                        else {
                            $(this).removeClass("invalid");
                        }
                    }
                    if ($(this).hasClass("email-field")) {
                        if (!isValidEmailAddress($(this).val())) {
                            $(this).addClass("invalid");
                        }
                        else {
                            $(this).removeClass("invalid");
                        }
                    }
                    if ($(this).val().trim() == '') {
                        $(this).addClass("invalid");
                    }
                });
                if ($("#formgroup-" + group + " .required-field.invalid[data-group='" + group + "']").length > 0) {
                    $("#formgroup-" + group + " .error").addClass("form-error", {
                        complete: function () {
                            $(this).show();
                            setTimeout(function () { $("#formgroup-" + group + " .error").hide("fade"); }, 3500);
                        }
                    });
                }

                else {
                    $("#formgroup-" + group).addClass("submitting");

                    var action = $("#formgroup-" + group).attr("data-action");
                    var xmlActionsURL = '/vui/vui-xml-actions/';
                    var datastring = [];

                    fld.find("[data-group='" + group + "']").each(function () {
                        var name = $(this).attr("itemprop");
                        var val = $(this).val();
                        datastring.push('"' + name + '":"' + val + '"');
                    });
                    var DataObject = jQuery.parseJSON("{ " + datastring.join() + " }");

                    $.ajax({
                        url: xmlActionsURL + "?a=" + action,
                        type: "POST",
                        dataType: 'json',
                        data: DataObject,
                        success: function (data) {
                            if (data.response == "valid") {
                                $("#formgroup-" + group + " .complete").addClass("form-complete");
                            }
                            else {
                                $("#formgroup-" + group + " .error").addClass("form-error");
                            }
                        },
                        error: function (data) {
                            $("#formgroup-" + group + " .error").addClass("form-error");
                        },
                        complete: function () {
                            $("#formgroup-" + group).removeClass("submitting");
                        }

                    });

                }
            });
        });
        return this.each(function () { });
    }
});


jQuery.fn.extend({
    ServiceFinder: function () {
        this.on("click", function () {
            if ($("#dd-service-finder").val() != "") {
                $("#dd-service-finder").find("option:selected").each(function () {
                    var url = $(this).attr("data-url");
                    // Temporary fix
                    url = url.replace("/vui/", "/vui3/");
                    document.location.href = url;
                });
            }
        });
    }
});

jQuery.fn.extend({
    GetScreenshots: function () {
        this.on("click", function (event) {
            event.preventDefault();
            var aid = $(this).attr("data-id");
            var sid = $(this).attr("data-service");
            var xmlActionsURL = '/vui/vui-xml-actions/';
            var DataObject = { "analysis": aid, "service": sid };
            $.ajax({
                url: xmlActionsURL + "?a=sii",
                type: "POST",
                dataType: 'json',
                data: DataObject,
                success: function (data) {
                    var outhtml = "";
                    if (data.response == "valid") {
                        if (data.data.ScreenshotsList.resultCount > 0) {
                            var screenshots = data.data.ScreenshotsList.screenshots
                            for (i = 0; i < screenshots.length; i++) {
                                outhtml += '<a href="' + screenshots[i].ImageURL_full + '"><img src="' + screenshots[i].ImageURL_th + '" /></a>';
                            }
                        }
                    }
                    $("#lightbox").append(outhtml);
                }
            });
        });
    }
});


jQuery.fn.extend({
    reSelecter: function () {
        this.selecter = null;
        this.unbind("change");
        this.removeClass("selecter-element");
        this.parent().find("div").remove();
        $(this).selecter({ cover: true });
        return this;
    }
});


jQuery.fn.extend({
    vuiLightboxChange: function () {
        this.on("change", function (event) {
            // // console.log("service: [" + $("#dd-lightbox-service").val() + "] device [" + $("#dd-lightbox-device").val() + "] function [" + $("#dd-lightbox-pagetype").val() + "]");
            var lbid = "lightbox-1"

            var xmlActionsURL = '/vui/vui-xml-actions/';
            var DataObject = { "service": $("#dd-lightbox-service").val(), "device": $("#dd-lightbox-device").val(), "pagetype": $("#dd-lightbox-pagetype").val() };
            $.ajax({
                url: xmlActionsURL + "?a=lis",
                type: "POST",
                dataType: 'json',
                data: DataObject,
                success: function (data) {

                    // Only build on a good response from the JSON handler
                    if (data.response == "valid") {
                        drawLightbox(data, lbid, { isloadingmore: false });
                    }
                }
            });
        });
    }
});


jQuery.fn.extend({
    vuiLoadMore: function () {
        this.on("click", function (event) {
            var lbid = "lightbox-1"

            var startfrom = $("#"+lbid + " .lightbox-gallery .carousel ul li.lightbox-item").length + 1;

            var xmlActionsURL = '/vui/vui-xml-actions/';
            var DataObject = { "service": $("#dd-lightbox-service").val(), "device": $("#dd-lightbox-device").val(), "pagetype": $("#dd-lightbox-pagetype").val(), "startfrom": startfrom };
            $.ajax({
                url: xmlActionsURL + "?a=lis",
                type: "POST",
                dataType: 'json',
                data: DataObject,
                success: function (data) {
                    // Only build on a good response from the JSON handler
                    if (data.response == "valid") {
                        drawLightbox(data, lbid, { isloadingmore: true });
                    }
                }
            });
        });
    }
});


jQuery.fn.extend({
    vuiLightbox: function (options) {

        var settings = $.extend({
            screenshots: "analysis", //Could be "favourites" or "search"
            pageTypeSelector: "",
            deviceSelector: ""
        }, options);

        this.on("click", function (event) {
            event.preventDefault();
            var lbid = "lightbox-1"// + aid.toString();

            if (settings.screenshots === "favourites") {

                var xmlActionsURL = '/vui/vui-xml-actions/';
                var DataObject = { "analysis": analysisId, "service": serviceId };
                $.ajax({
                    url: xmlActionsURL + "?a=fc",
                    type: "POST",
                    dataType: 'json',
                    data: DataObject,
                    success: function (data) {

                        // Only build on a good response from the JSON handler
                        if (data.response == "valid") {

                            // Set DropDowns
                            drawLightbox(data, lbid, { isloadingmore: false, serviceName: serviceName,
                                device: device,
                                analysisTag: analysisTag,
                                serviceId: serviceId,
                                analysisId: analysisId,
                                screenshots: "favourites"
                            });
                        }
                    }
                });
            }
            else if (settings.screenshots == "search") {
                var xmlActionsURL = '/vui/vui-xml-actions/';
                var DataObject = { "service": "", "device": $(settings.deviceSelector).val(), "pagetype": $(settings.pageTypeSelector).val() };
                $.ajax({
                    url: xmlActionsURL + "?a=lis",
                    type: "POST",
                    dataType: 'json',
                    data: DataObject,
                    success: function (data) {
                        if (data.response == "valid") {
                            // Set DropDowns
                            drawLightbox(data, lbid, { isloadingmore: false,
                                device: $(settings.deviceSelector).val(),
                                pageType: $(settings.pageTypeSelector).val()
                            });
                        }
                    }
                });
            }
            else if (settings.screenshots == "analysis") {

                var serviceName = $(this).attr("data-service"),
                    analysisTag = $(this).attr("data-analysis"),
                    analysisId = $(this).attr("data-id"),
                    serviceId = $(this).attr("data-serviceid"),
                    device = $(this).attr("data-device");

                // Otherwise populate from JSON and build
                var xmlActionsURL = '/vui/vui-xml-actions/';
                var DataObject = { "analysis": analysisId, "service": serviceId };
                $.ajax({
                    url: xmlActionsURL + "?a=sii",
                    type: "POST",
                    dataType: 'json',
                    data: DataObject,
                    success: function (data) {

                        // Only build on a good response from the JSON handler
                        if (data.response == "valid") {

                            // Set DropDowns
                            drawLightbox(data, lbid, { isloadingmore: false, serviceName: serviceName,
                                device: device,
                                analysisTag: analysisTag,
                                serviceId: serviceId,
                                analysisId: analysisId
                            });
                        }
                    }
                });
            }
        });

        return this.each(function () {

        });
    }
});
function addCommas(nStr) {
    nStr += '';
    x = nStr.split('.');
    x1 = x[0];
    x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
}

jQuery.fn.extend({
    AddServiceFavourite: function () {
        this.on("click", function (event) {
            event.preventDefault();
            if ($("ul.favourite-services li").length < 5) {
                var serviceId = $("#select-favourite-service").val();
                if ($("ul.favourite-services a[data-id='" + serviceId + "']").length == 0) {
                    var el = $(this);
                    var xmlActionsURL = '/vui/vui-xml-actions/';
                    var DataObject = { "id": serviceId };
                    $.ajax({
                        url: xmlActionsURL + "?a=fsv",
                        type: "POST",
                        dataType: 'json',
                        data: DataObject,
                        success: function (data) {
                            $("ul.favourite-services").append("<li><a title=\"Remove this from your favourites\" href=\"#\" data-id=\"" + data.data.id + "\"><span class=\"icon-minus-sign\"></span></a> " + data.data.serviceName + "</li>");
                            $("ul.favourite-services a[data-id='" + data.data.id + "']").DeleteServiceFavourite();

                            $(".b-logged-service .tabs li.choose").before("<li data-id='" + data.data.id + "'><a href='#'>" + data.data.serviceName + "</a></li>");
                            var pn = $("#favourite-template div.pane").clone();

                            pn.find("[rel='data-date']").each(function () { $(this).text(data.data.niceDate); });
                            if (data.data.numScreenshots <= 0) {
                                pn.find("[rel='ss']").remove();
                            } else {
                                pn.find("[rel='ss']")
                                    .find("span[rel='data-1']").text(addCommas(data.data.numScreenshots)).end()
                                    .find("span[rel='data-2']").text(data.data.numScreenshotDevices);
                            }
                            if (data.data.benchmarkAverage <= 0) {
                                pn.find("[rel='be']").remove();
                            } else {
                                pn.find("[rel='be']")
                                    .find("span[rel='data-1']").text(data.data.benchmarkAverage);
                            }
                            if (data.data.ratingAverage <= 0) {
                                pn.find("[rel='ra']").remove();
                            } else {
                                pn.find("[rel='ra']")
                                    .find("span[rel='data-1']").text(data.data.ratingAverage.toFixed(1));
                            }
                            if (data.data.twitterFollowers <= 0) {
                                pn.find("[rel='tw']").remove();
                            } else {
                                pn.find("[rel='tw']")
                                    .find("span[rel='data-1']").text(addCommas(data.data.twitterFollowers));
                            }
                            if (data.data.facebookLikes <= 0) {
                                pn.find("[rel='fb']").remove();
                            } else {
                                pn.find("[rel='fb']")
                                    .find("span[rel='data-1']").text(addCommas(data.data.facebookLikes));
                            }
                            if (data.data.ytSubscriberCount <= 0) {
                                pn.find("[rel='yt']").remove();
                            } else {
                                pn.find("[rel='yt']")
                                    .find("span[rel='data-1']").text(addCommas(data.data.ytSubscriberCount));
                            }


                            $(".b-logged-service .panes .pane.favs").before(pn);

                            $(".b-logged-service .b-tabs .tabs").unbind("tabs").tabs("div.panes > div", {
                                initialIndex: $("ul.favourite-services li").length
                            });
                        }
                    });
                }
            }
        });

        return this.each(function () { });
    }
});
jQuery.fn.extend({
    DeleteServiceFavourite: function () {
        this.on("click", function (event) {
            event.preventDefault();
            var serviceId = $(this).attr("data-id");
            var el = $(this);
            var xmlActionsURL = '/vui/vui-xml-actions/';
            var DataObject = { "id": serviceId };
            $.ajax({
                url: xmlActionsURL + "?a=fuv",
                type: "POST",
                dataType: 'json',
                data: DataObject,
                success: function (data) {
                    $("ul.favourite-services").find("a[data-id='" + serviceId + "']").parents("li").remove();
                    $(".tabs li[data-id='" + serviceId + "']").fadeOut(500, function(){ $(this).remove() });
                    $(".panes .pane[data-id='" + serviceId + "']").remove();
                }
            });
        });
        return this.each(function () { });
    }
});

jQuery.fn.extend({
    ServiceFavourite: function () {
        this.on("click", function (event) {
            event.preventDefault();
            var serviceId = $(this).attr("data-id");
            var el = $(this);
            if (el.hasClass("is-favourite")) {
                var xmlActionsURL = '/vui/vui-xml-actions/';
                var DataObject = { "id": serviceId };
                $.ajax({
                    url: xmlActionsURL + "?a=fuv",
                    type: "POST",
                    dataType: 'json',
                    data: DataObject,
                    success: function (data) {
                        //// console.log("Delete service from favourites: " + data.response + "; " + data.data);
                        // $("#favourite-message").text(data.response + " " + data.data);
                        el
                            .find("span").removeClass("icon-minus-sign").addClass("icon-plus-sign").end()
                            .removeClass("is-favourite")
                            .attr("title", "Add to your favourite services");
                    }
                });
            }
            else {
                var xmlActionsURL = '/vui/vui-xml-actions/';
                var DataObject = { "id": serviceId };
                $.ajax({
                    url: xmlActionsURL + "?a=fsv",
                    type: "POST",
                    dataType: 'json',
                    data: DataObject,
                    success: function (data) {
                        //// console.log("Add service to favourites: " + data.response + "; " + data.data);
                        if (data.response == "valid") {
                            el
                                .find("span").addClass("icon-minus-sign").removeClass("icon-plus-sign").end()
                                .addClass("is-favourite")
                                .attr("title", "Remove from your favourite services");
                        }
                        //else
                        //    $(this).removeClass("icon-trash").addClass("icon-paper-clip").removeClass("is-favourite");
                    }
                });
            }
        });

        return this.each(function () {
            if ($(this).hasClass("is-favourite")) {
                $(this).find("span").removeClass("icon-plus-sign").addClass("icon-minus-sign").end().attr("title", "Remove from your favourite services");
            } 
        });
    }
});

jQuery.fn.extend({
    vuiFavourite: function () {
        this.on("click", function (event) {
            event.preventDefault();
            var imageId = $(this).parents(".lightbox-item").attr("data-id");
            var el = $(this);
            if (el.hasClass("is-favourite")) {
                var xmlActionsURL = '/vui/vui-xml-actions/';
                var DataObject = { "id": imageId, "collection": "" };
                $.ajax({
                    url: xmlActionsURL + "?a=fu",
                    type: "POST",
                    dataType: 'json',
                    data: DataObject,
                    success: function (data) {
                        //// console.log("Delete image from favourites: " + data.response + "; " + data.data);
                        // $("#favourite-message").text(data.response + " " + data.data);
                        el.removeClass("icon-mins-sign").addClass("icon-paper-clip").removeClass("is-favourite").attr("title", "Add to your favourite screenshots");
                        $(".b-right-nav .favs-count").text(data.count);
                    }
                });
            }
            else {

                var xmlActionsURL = '/vui/vui-xml-actions/';
                var DataObject = { "id": imageId, "collection": "" };
                $.ajax({
                    url: xmlActionsURL + "?a=fi",
                    type: "POST",
                    dataType: 'json',
                    data: DataObject,
                    success: function (data) {
                        //// console.log("Add image to favourites: " + data.response + "; " + data.data);
                        if (data.response == "valid") {
                            el.addClass("icon-minus-sign").removeClass("icon-paper-clip").addClass("is-favourite").attr("title", "Remove from your favourite screenshots");
                            $(".b-right-nav .favs-count").text(data.count);
                        }
                        //else
                        //    $(this).removeClass("icon-trash").addClass("icon-paper-clip").removeClass("is-favourite");
                    }
                });
            }
        });

        return this.each(function () {
            if ($(this).hasClass("is-favourite")) {
                $(this).removeClass("icon-paper-clip").addClass("icon-trash").attr("title", "Remove from your favourite screenshots");
            }
        });
    }
});

function UpdateFavouriteImages() {
    var target = ".screenshots-carousel"
    if($(target).length > 0) {
        
        var xmlActionsURL = '/vui/vui-xml-actions/';
        $.ajax({
            url: xmlActionsURL + "?a=fc",
            type: "POST",
            dataType: 'json',
            success: function (data) {
                //if($(target).find(".carousel").length == 0) {
                //    $(target).find(".shadow-right").appendAfter('<div class="carousel"></div>');
                //}
                screenshotUnCarousel();
                $(target).find(".shadow-right").after('<div class="carousel"><ul></ul></div>');
                for (var i = 0; i < data.data.ScreenshotsList.resultCount; i++) {
                    var el = $("#favourite-image-template li").clone();
                    el
                        .find("[rel='servicename']").text(data.data.ScreenshotsList.screenshots[i].ServiceName).end()
                        .find("[rel='pagetype']").text(data.data.ScreenshotsList.screenshots[i].PageType).end()
                        .find("[rel='device']").text(data.data.ScreenshotsList.screenshots[i].Device).end()
                        .find("[rel='importtag']").text(data.data.ScreenshotsList.screenshots[i].ImportTag).end()
                        .find(".img img").attr("src", data.data.ScreenshotsList.screenshots[i].ImageURL_th).end()
                    $(target).find(".carousel ul").append(el);
                }
                screenshotCarousel();
            }
        });
    }
}





jQuery.event.special.swipe.settings = {
    threshold: 0.1,
    sensitivity: 9
};