$.fn.selectpicker_addItem = function (placehoder = "") {
    var content = "<input type='text' class='bss-input' onKeyDown='event.stopPropagation();' onKeyPress='addSelectInpKeyPress(this,event)' onClick='event.stopPropagation()' placeholder='" + placehoder + "'> <span class='glyphicon glyphicon-plus addnewicon mlm' onClick='addSelectItem(this,event,1);'></span>";

    var divider = $('<option/>')
        .addClass('divider')
        .data('divider', true);


    var addoption = $('<option/>', { class: 'addItem' })
        .data('content', content);

    $(this).selectpicker('destroy');
    $(this).append(divider)
        .append(addoption)
        .selectpicker();
};

function addSelectItem(t, ev) {
    ev.stopPropagation();

    var bs = $(t).closest('.bootstrap-select')
    var txt = bs.find('.bss-input').val().replace(/[|]/g, "");
    txt = $(t).prev().val().replace(/[|]/g, "");
    if ($.trim(txt) == '') return;

    // Changed from previous version to cater to new
    // layout used by bootstrap-select.
    var p = bs.find('select');
    var o = $('option', p).eq(-2);
    o.before($("<option>", { "selected": true, "text": txt, "value": txt }));
    p.selectpicker('refresh');
}

function addSelectInpKeyPress(t, ev) {
    ev.stopPropagation();

    // do not allow pipe character
    if (ev.which == 124) ev.preventDefault();

    // enter character adds the option
    if (ev.which == 13) {
        ev.preventDefault();
        addSelectItem($(t).next(), ev);
    }
}