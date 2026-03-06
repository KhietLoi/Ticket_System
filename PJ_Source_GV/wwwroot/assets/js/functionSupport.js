String.prototype.toDateDMY = function (char) {
    var dateParts = this.split(char);

    // month is 0-based, that's why we need dataParts[1] - 1
    var dateObject = new Date(+dateParts[2], dateParts[1] - 1, +dateParts[0]);

    return dateObject;
}

String.prototype.toDateFormMY = function (char) {
    var dateParts = this.split(char);

    // month is 0-based, that's why we need dataParts[1] - 1
    var dateObject = new Date(+dateParts[1], dateParts[0] - 1, +1);

    return dateObject;
}

Date.prototype.toStringDMY = function (char) {
    var dd = String(this.getDate()).padStart(2, '0');
    var mm = String(this.getMonth() + 1).padStart(2, '0'); //January is 0!
    var yyyy = this.getFullYear();

    return dd + char + mm + char + yyyy;
}

Date.prototype.toStringMY = function (char) {
    var mm = String(this.getMonth() + 1).padStart(2, '0'); //January is 0!
    var yyyy = this.getFullYear();

    return mm + char + yyyy;
}

Date.prototype.toPost = function () {
    var dd = String(this.getDate()).padStart(2, '0');
    var mm = String(this.getMonth() + 1).padStart(2, '0'); //January is 0!
    var yyyy = this.getFullYear();

    return yyyy + '-' + mm + '-' + dd;
}

Date.prototype.addDays = function (days) {
    var date = new Date(this.valueOf());
    date.setDate(date.getDate() + days);
    return new Date(date);
}

$(document).ready(function () {
    $(".datepicker").datepicker({
        todayBtn: "linked",
        autoclose: true,
        todayHighlight: true,
        format: 'dd/mm/yyyy'
    });
});

function reDrawDatepicker(nameClass) {
    $(`.${nameClass}`).datepicker({
        todayBtn: "linked",
        autoclose: true,
        todayHighlight: true,
        format: 'dd/mm/yyyy'
    });
}

function reDrawDatepickerMonth(nameClass) {
    $(`.${nameClass}`).datepicker({
        todayBtn: "linked",
        autoclose: true,
        todayHighlight: true,
        format: "mm/yyyy",
        startView: "months",
        minViewMode: "months"
    });
}

function getFirstDayOfWeek(d) {
    // 👇️ clone date object, so we don't mutate it
    const date = new Date(d);
    const day = date.getDay(); // 👉️ get day of week

    // 👇️ day of month - day of week (-6 if Sunday), otherwise +1
    const diff = date.getDate() - day + (day === 0 ? -6 : 1);

    return new Date(date.setDate(diff));
}