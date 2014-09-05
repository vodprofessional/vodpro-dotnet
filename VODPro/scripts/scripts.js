
/*$(document).ready(function () {

});
*/

function Login(submitBtn) {

    var id = submitBtn.id;
    var usernameid = id.substring(0, id.indexOf('btnSubmit')) + 'username';
    var pwdid = id.substring(0, id.indexOf('btnSubmit')) + 'password';
    
    var userval = $('#' + usernameid).val();
    var pwdval = $('#' + pwdid).val();

   
    var DataObject = { username: userval, password: pwdval };
    var html = returnXML("/xml/actions.aspx?action=login", DataObject);
    alert(html);
    location.reload(true);

}

function returnXML(url, DataObject) {
    var html = $.ajax({
        url: url,
        type: "POST",
        data: DataObject,
        error: function (x, e) {
            if (x.status == 0) {
                alert('You are offline!!\n Please Check Your Network.');
            } else if (x.status == 404) {
                alert('Requested URL not found.');
            } else if (x.status == 500) {
                alert('Internel Server Error.');
                //		$("#OTGerror").html(x.responseText);
            } else if (e == 'parsererror') {
                alert('Error.\nParsing JSON Request failed.');
            } else if (e == 'timeout') {
                alert('Request Time out.');
            } else {
                alert('Unknow Error.\n' + x.responseText);
            }
        }, async: false
    }).responseText;
    return html;
}


/*
function saveQuestion() {
	var id = $('#edit').attr("rel");
	tinyMCE.triggerSave();
	var question = $('#contentEditor').val();
	var DataObject = { id: id, question: Encoder.htmlEncode(question) };
	returnXML("/xml/ajax-actions/?action=saveQuestion", DataObject);
	alert("Question saved");
	location.reload(true);
}


function returnXML(url, DataObject) {
    var html = $.ajax({
        url: url,
        type: "POST",
        data: DataObject,
        error: function (x, e) {
            if (x.status == 0) {
                alert('You are offline!!\n Please Check Your Network.');
            } else if (x.status == 404) {
                alert('Requested URL not found.');
            } else if (x.status == 500) {
                alert('Internel Server Error.');
		//		$("#OTGerror").html(x.responseText);
            } else if (e == 'parsererror') {
                alert('Error.\nParsing JSON Request failed.');
            } else if (e == 'timeout') {
                alert('Request Time out.');
            } else {
                alert('Unknow Error.\n' + x.responseText);
            }
        }, async: false
    }).responseText;
    return html;
}


function addQuestion() {
    tinyMCE.triggerSave();
    var t = $('#title').val();
    var q = $('#question').val();
    var d = $('#difficulty').val();
    var p = $('#paper').val();
    var tg = $('#tags').val();
    var pr = $('#provider').val();
    var ty = $('#type').val();
    var s = $('#source').val();

    if (t == '') {
        alert("Please complete the title field");
    }

    if (q == '') {
        alert("Please complete the question text area");
    }

    else {
        Encoder.EncodeType = "entity";
        var url = "/xml/ajax-actions/?action=add&tags=" + tg;
        $("#ajaxLoading").show();
        var DataObject = { action: "add", title: t, difficulty: d, tags: tg, type: ty, provider: pr, paper: p, source: s, question: Encoder.htmlEncode(q) };
        $.ajax({
            type: "POST",
            url: url,
            data: DataObject,
            error: function (x, e) {
                if (x.status == 0) {
                    alert('Please Check Your Network.\n You maybe offline');
                } else if (x.status == 404) {
                    alert('Requested URL not found.\n Please contact the administrator');
                } else if (x.status == 500) {
                    alert('Internal Server Error.\n Please contact the administrator');
                } else if (e == 'parsererror') {
                    alert('Error.\nParsing JSON Request failed.\n Contact the administrator');
                } else if (e == 'timeout') {
                    alert('Request Time out.\n Contact the administrator');
                } else {
                    alert('Unknow Error.\n' + x.responseText);
                }
            },
            success: function (msg) {
                $(msg).find("response").each(function () {
                    window.location = $(this).find("url").text()
                });
            }
        });
    }
}

*/