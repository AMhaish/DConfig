
$(document).ready(function () {
    initialize();
});

var jwindow;
function initialize() {
    jwindow = $(window);
    if (!Array.prototype.indexOf) {
        Array.prototype.indexOf = function (obj, start) {
            for (var i = (start || 0), j = this.length; i < j; i++) {
                if (this[i] === obj) { return i; }
            }
            return -1;
        }
    }

    //Code for enabling DatePicker
    var datePickers = $('.datePicker');
    if (datePickers && datePickers.length > 0) {
        datePickers.datepicker({
            changeMonth: true,
            changeYear: true,
            yearRange: "c-50:+0",
            dateFormat: 'mm/dd/yy',
            altFormat: 'mm/dd/yy',
        });
    }

}



