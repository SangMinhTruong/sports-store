$(document).ready(function () {

    $('#date-month').datepicker({
        format: "mm/yyyy",
        startView: "months",
        minViewMode: "months",
        orientation: "bottom",
    });

    $('#date-year').datepicker({
        format: "yyyy",
        startView: "years",
        minViewMode: "years",
        orientation: "bottom",
    });

    $('#orders_table').DataTable();
    $('#imports_table').DataTable();

    $.fn.dataTable.moment('MMMM');
    $('#year_table').DataTable();
});