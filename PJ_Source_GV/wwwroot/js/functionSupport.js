String.prototype.toDate = function (char) {
    var dateParts = this.split(char);

    // month is 0-based, that's why we need dataParts[1] - 1
    var dateObject = new Date(+dateParts[2], dateParts[1] - 1, +dateParts[0]);

    return dateObject;
}

Date.prototype.toStringDMY = function (char) {
    var dd = String(this.getDate()).padStart(2, '0');
    var mm = String(this.getMonth() + 1).padStart(2, '0'); //January is 0!
    var yyyy = this.getFullYear();

    return dd + char + mm + char + yyyy;
}

Date.prototype.toPost = function () {
    var dd = String(this.getDate()).padStart(2, '0');
    var mm = String(this.getMonth() + 1).padStart(2, '0'); //January is 0!
    var yyyy = this.getFullYear();

    return yyyy + '-' + mm + '-' + dd;
}

function RenderDatepicker(classnameDatepicker, formatInputDate) {
    $(`.${classnameDatepicker}`).datepicker({
        format: formatInputDate,
    });
}