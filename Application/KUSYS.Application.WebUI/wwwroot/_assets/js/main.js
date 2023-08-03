const main = (function () {

    function CreateDatatable(id, options) {
        var defaultOptions = {
            order: [[0, 'desc']],
            destroy: true,
            info: true,
            responsive: true,
            pageLength: 10,
            "lengthMenu": [[10, 20, 30, 50, 100], [10, 20, 30, 50, 100]],
            sDom: '<"row view-filter"<"col-sm-12"<"float-left"l><"float-right"f><"clearfix">>>t<"row view-pager"<"datatablePages"<"text-center"ip>>>',
            language: {
                url: "/_assets/datatables/datatable.turkish.json",

                paginate: {
                    previous: "<i class='ph-arrow-circle-left'></i>",
                    next: "<i class='simple-icon-arrow-right'></i>"
                }
            },
            drawCallback: function () {
                $($(".dataTables_wrapper .pagination li:first-of-type"))
                    .find("a")
                    .addClass("prev");
                $($(".dataTables_wrapper .pagination li:last-of-type"))
                    .find("a")
                    .addClass("next");

                $(".dataTables_wrapper .pagination").addClass("pagination-sm");
                $('.view-filter').remove();
                $('.datatable-outofbox-infotext').text($('.dataTables_info').text());
                $('.dataTables_info').hide();
            }
        };

        var settings = $.extend({}, defaultOptions, options);

        var oTable = $(id).DataTable(settings);

        $('#datatableSearch').keyup(function () {

            var val = $(this).val();
            oTable.search(val).draw();
        });

        $('.datatable-outofbox-lengthtext').on('change', function (e) {
            var val = $(this).val();
        });


        $('.datatable-outofbox-dropdownButton').click(function () {
            $('.datatable-outofbox-lengthtext').text($(this).text());
            oTable.page.len($(this).text()).draw();
        });


        return oTable;
    }

    function BlockPage() {
        $(".blogPage").css("display", "flex");
    }
    function NoBlockPage() {
        $(".blogPage").css("display", "none");
    }
    function ShowLoading() {
        $(".loading").css("display", "block");
    }
    function HideLoading() {
        $(".loading").css("display", "none");
    }

    function ShowError(title, message) {
        if (title == null || title == undefined || title.length <= 0) title = "Hay Aksi";
        if (message == null || message == undefined || message.length <= 0) message = "İşlem tamamlanmadı";

        Swal.fire({
            title: title,
            html: message,
            confirmButtonText: 'Tamam',
        });
    }

    function SuccessMessage(title, message) {
        if (title == null || title == undefined || title.length <= 0) title = "İşlem Başarılı";
        if (message == null || message == undefined || message.length <= 0) message = "İşlem tamamlandı";

        Swal.fire({
            title: title,
            icon:'success',
            html: message,
            confirmButtonText: 'Tamam',
        });
    }

    function ErrorMessageList(title, errors, message) {
        let html = "", item;
        if (title == null || title == undefined || title.length <= 0) { title = "Hatalı işlem" }
        if (errors == null || errors == undefined || errors.length <= 0) { errors = ["Hatalı işlem"] }
        if (message == null || message == undefined || message.length <= 0) { html += "<p>Eksik veya hatalı işlem</p>"; }

        if (errors.length > 0) {
            html += '<ul style="list-style: decimal;">';

            for (k in errors) {
                item = errors[k];
                html += '<li style="margin-bottom:5px;">' + item + '</li>';
            }

            html += '</ul>';
        }

        Swal.fire({
            title: title,
            html: html,
            showCloseButton: true,
            confirmButtonText: 'Tamam',
            confirmButtonAriaLabel: 'Tamam',
        });
    }

    function SuccessActionMessage(title, message, href) {
        if (title == null || title == undefined || title.length <= 0) title = "Başarılı";
        if (message == null || message == undefined || message.length <= 0) message = "İşlem tamamlandı";

        Swal.fire({
            title: title,
            html: message,
            icon: 'success',
            timer: 1500,
            timerProgressBar: true,
            didOpen: () => {
                Swal.showLoading()
                timerInterval = setInterval(() => {
                    const content = Swal.getHtmlContainer()
                    if (content) {
                        const b = content.querySelector('b')
                        if (b) {
                            b.textContent = Swal.getTimerLeft()
                        }
                    }
                }, 100)
            },
            willClose: () => {
                clearInterval(timerInterval);
                if (href == null || href == undefined || href.length <= 0) location.reload();
                location.href = href;
            }
        });
    }

    function ErrorActionMessage(title, message, href) {
        if (title == null || title == undefined || title.length <= 0) title = "Hay Aksi!";
        if (message == null || message == undefined || message.length <= 0) message = "Bir eksiklik var";

        Swal.fire({
            title: title,
            html: message,
            timer: 1500,
            timerProgressBar: true,
            didOpen: () => {
                Swal.showLoading()
                timerInterval = setInterval(() => {
                    const content = Swal.getHtmlContainer()
                    if (content) {
                        const b = content.querySelector('b')
                        if (b) {
                            b.textContent = Swal.getTimerLeft()
                        }
                    }
                }, 100)
            },
            willClose: () => {
                clearInterval(timerInterval);
                if (href == null || href == undefined || href.length <= 0) location.reload();
                location.href = href;
            }
        });
    }

    const AjaxHelper = (settings, callback) => {
        let returnOBJ = {};
        $.ajax(settings)
            .done(function (e) {
                returnOBJ.error = false;
                returnOBJ.code = "";
                returnOBJ.data = e;
                callback(e);
            })
            .fail(function (e) {
                returnOBJ.error = true;
                returnOBJ.code = "1";
                returnOBJ.data = "";
                callback(e);
            });
    };

    return {
        CreateDatatable: CreateDatatable,
        BlockPage: BlockPage,
        NoBlockPage: NoBlockPage,
        ShowLoading: ShowLoading,
        HideLoading: HideLoading,
        ShowError: ShowError,
        AjaxHelper: AjaxHelper,
        SuccessMessage: SuccessMessage,
        ErrorMessageList: ErrorMessageList,
        SuccessActionMessage: SuccessActionMessage,
        ErrorActionMessage: ErrorActionMessage,
    };

}());

const LayoutJsHelper = (function () {

    function Passive(url, id) {
        Swal.fire({
            title: 'Emin misiniz?',
            text: "Seçtiğiniz veri sistemde pasif olarak işaretlenecektir.",
            icon: 'warning',
            showCancelButton: true,
            showCloseButton: true,
            confirmButtonText: 'Evet, Pasif Et',
            cancelButtonText: 'İptal',
            buttonsStyling: false,
            customClass: {
                confirmButton: 'btn btn-success mr-3',
                cancelButton: 'btn btn-danger'
            }
        }).then(function (result) {
            if (result.value) {
                $.get("/" + url + "/" + id, function (e) {
                    if (e.isSucceed) {
                        Swal.fire({
                            title: "Başarılı",
                            text: e.message,
                            icon: "success"
                        }).then((willClick) => {
                            location.reload();
                        });
                    }
                    else {
                        Swal.fire(
                            'Hay Aksi',
                            e.message,
                            'warning'
                        )
                    }
                });
            }
        });
    }

    function Active(url, id) {
        Swal.fire({
            title: 'Emin misiniz?',
            text: "Seçtiğiniz veri sistemde aktif olarak işaretlenecektir.",
            icon: 'warning',
            showCancelButton: true,
            showCloseButton: true,
            confirmButtonText: 'Evet, Aktif Et',
            cancelButtonText: 'İptal',
            buttonsStyling: false,
            customClass: {
                confirmButton: 'btn btn-success mr-3',
                cancelButton: 'btn btn-danger'
            }
        }).then(function (result) {
            if (result.value) {
                $.get("/" + url + "/" + id, function (e) {
                    if (e.isSucceed) {
                        Swal.fire({
                            title: "Başarılı",
                            text: e.message,
                            icon: "success"
                        }).then((willClick) => {
                            location.reload();
                        });
                    }
                    else {
                        Swal.fire({
                            title: "Hay Aksi",
                            text: e.message,
                            icon: "warning"
                        });
                    }
                });
            }
        });
    }

    function Delete(url, id) {
        Swal.fire({
            title: 'Emin misiniz?',
            text: "Seçtiğiniz veri sistemden silinecektir.",
            icon: 'warning',
            showCancelButton: true,
            showCloseButton: true,
            confirmButtonText: 'Evet, Sil',
            cancelButtonText: 'İptal',
            buttonsStyling: false,
            customClass: {
                confirmButton: 'btn btn-success mr-3',
                cancelButton: 'btn btn-danger'
            }
        }).then(function (result) {
            if (result.value) {
                $.get("/" + url + "/" + id, function (e) {
                    if (e.isSucceed) {
                        Swal.fire({
                            title: "Başarılı",
                            text: e.message,
                            icon: "success"
                        }).then((willClick) => {
                            location.reload();
                        });
                    }
                    else {
                        Swal.fire({
                            title: "Hay Aksi",
                            text: e.message,
                            icon: "warning"
                        });
                    }
                });
            }
        });
    }

    return {
        Passive: Passive,
        Active: Active,
        Delete: Delete,
    };
}());