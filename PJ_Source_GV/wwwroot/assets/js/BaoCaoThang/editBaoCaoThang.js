var fromEditBaoCaoThang = {};

-$(document).ready(function () {

    //Xử lý chèn modal edit task
    $.ajax({
        url: '/BaoCaoThang/FormEdit',
        method: 'POST',
        contentType: false,
        caches: false,
        processData: false,
        success: function (resp) {
            if (!resp) {
                $("#editBaoCaoThangContent").html("Không lấy được form edit");
            } else {
                $("#editBaoCaoThangContent").html(resp);
            }
        },
        error: function (e) {
            toastr.error("ERROR TaskManager - FormEdit: " + e.status + " - " + e.statusText);
            KTApp.unblock('.blockui-loading', {});
            if (e.status == 500 || e.status == 404) {
                setTimeout(function () {
                    if (confirm("Bạn có muốn load lại trang và thử lại?")) {
                        location.reload();
                    }
                }, 2000);
            }
        }
    }).then(function () {
        KTApp.unblock('.blockui-loading', {});
        if (!$("#editBaoCaoThangContent").html() && $("#editBaoCaoThangContent").html() == "Không lấy được form edit") {
            toastr.error("ERROR TaskManager - FormEdit: Không lấy được form edit");
        } else {
            $("#selProject").change(function () {
                LoadThanhVien();
            });

            $("#selectTrangThaiBaoCaoThang").change(function () {
                if ($("#selectTrangThaiBaoCaoThang").val() == 4 || $("#selectTrangThaiBaoCaoThang").val() == 3) {
                    $("#selDateDone").show();
                } else {
                    $("#selDateDone").hide();
                }
            });

            $("#selType, #selLevel, #selDeadlineID").change(function () {
                CalculatorPoint();
            });

            reDrawDatepicker('datepicker');
            
            fromEditBaoCaoThang = FormValidation.formValidation(
                document.getElementById('editBaoCaoThang_form'),
                {
                    fields: {
                        inputTieuDeBaoCaoThang: {
                            validators: {
                                notEmpty: {
                                    message: "Không được bỏ trống nội dung"
                                }
                            }
                        },
                        dateInputThangBaoCao: {
                            validators: {
                                notEmpty: {
                                    message: "Bắt buộc phải chọn thời gian báo cáo"
                                }
                            }
                        }
                    },

                    plugins: {
                        trigger: new FormValidation.plugins.Trigger(),
                        bootstrap: new FormValidation.plugins.Bootstrap({
                            eleInvalidClass: '',
                            eleValidClass: '',
                        })
                    }
                }
            );
        }
    });
});

async function SaveBaoCaoThang(isTiepTucTao) {
    fromEditBaoCaoThang.validate().then(function (status) {
        if (status === 'Valid') {
            if (!$("#dateInputThangBaoCao").val()) {
                alert("Yêu cầu nhập tháng báo cáo");
                return;
            }

            $("#EditModalBaoCaoThang").modal("hide");

            let frmData = new FormData();
            frmData.append("TieuDe", $("#inputTieuDeBaoCaoThang").val());
            frmData.append("NoiDung", $("#inputContentBaoCaoThang").val());
            frmData.append("GhiChu", $("#noteBaoCaoThang").val());
            
            frmData.append("NgayBaoCao", $("#dateInputThangBaoCao").val().toDateFormMY("/").toPost());
            frmData.append("TrangThaiID", $("#selectTrangThaiBaoCaoThang").val());
            frmData.append("ID", baoCaoThangViewEdit.ID);
            let resultSubmit = false;
            KTApp.block('.blockui-loading', {});
            $.ajax({
                url: 'BaoCaoThang/CreateOrEdit',
                method: 'POST',
                data: frmData,
                contentType: false,
                cache: false,
                processData: false,
                success: function (resp) {
                    if (resp.result) {
                        resultSubmit = true;
                        if (resp.ID) {
                            toastr.success("Tạo thành công Task ID: " + resp.ID);
                        } else {
                            toastr.success("Cập nhập thành công");
                        }
                        
                        resultSave = true;
                    } else {
                        toastr.error(resp.messages);
                    }
                },
                error: function (e) {
                    toastr.error("ERROR CreateOrEditTask: " + e.status + " - " + e.statusText);
                    KTApp.unblock('.blockui-loading', {});
                    if (e.status == 500 || e.status == 404) {
                        setTimeout(function () {
                            if (confirm("Bạn có muốn load lại trang và thử lại?")) {
                                location.reload();
                            }
                        }, 2000);
                    }
                }
            }).then(function () {
                KTApp.unblock('.blockui-loading', {});

                setTimeout(function () {
                    if (isTiepTucTao && resultSubmit) {
                        $("#inputTieuDeBaoCaoThang").val("");
                        $("#inputContentBaoCaoThang").val("");
                        $("#noteBaoCaoThang").val("");
                        $("#EditModalBaoCaoThang").modal("show");
                    }
                },600);
                
                return resultSave;
            });
        } else {
            
        }
        return resultSave;
    });
}

function LoadThanhVien() {
    let idProject = $("#selProject").val()

    let option = ''
    if (idProject > 0 && listDuAnCuaNguoiDung.length > 0) {
        let duAn = listDuAnCuaNguoiDung.find(e => e.ID == idProject);
        if (duAn) {
            let listMaThanhVien = duAn.ListThanhVien + "," + duAn.MaNhanVienQuanLy;
            $.each(listThanhVienQuanLy, function (index, data) {
                if (listMaThanhVien.includes(data.MaNhanVien)) {
                    option += '<option value="' + data.MaNhanVien + '">' + data.Ten_Email + '</option>';
                }
            });
        }
    } else {
        option = '<option value="" disabled>' + "Không có dữ liệu" + '</option>';
    }

    $("#selLecturerChange").html(option);

    if (!taskViewEdit || taskViewEdit.ID <= 0) {
        $("#selLecturerChange").selectpicker('val', maNV);
    } else {
        if (taskViewEdit.ListMaNhanVienThucHien) {
            $("#selLecturerChange").selectpicker('val', taskViewEdit.ListMaNhanVienThucHien.split(","));
        }
    }
    $("#selLecturerChange").selectpicker("refresh");

}


var baoCaoThangViewEdit = {};
function ShowEditBaoCaoThang(baoCaoThangEdit) {
    baoCaoThangViewEdit = baoCaoThangEdit;

    $("#inputTieuDeBaoCaoThang").val(baoCaoThangViewEdit.TieuDe);
    $("#inputContentBaoCaoThang").val(baoCaoThangViewEdit.NoiDung);
    $("#noteBaoCaoThang").val(baoCaoThangViewEdit.GhiChu);
    if (!baoCaoThangEdit || baoCaoThangEdit.ID <= 0) {
        $("#btnDeleteBaoCaoThang").hide();
        
        $("#selectTrangThaiBaoCaoThang").selectpicker('val', 1);
        $("#btnTaoMoiBCTLienTuc").show();

        $("#dateInputThangBaoCao").val((new Date()).toStringMY("/"));

    } else {
        $("#selectTrangThaiBaoCaoThang").selectpicker('val', baoCaoThangViewEdit.TrangThaiID);

        $("#dateInputThangBaoCao").val((new Date(baoCaoThangViewEdit.NgayBaoCao)).toStringMY("/"));

        if (!baoCaoThangEdit.IsConfirm) {
            $("#btnDeleteBaoCaoThang").show();
        } else {
            $("#btnDeleteBaoCaoThang").hide();

        }
        $("#btnTaoMoiBCTLienTuc").hide();

    }
    $("#selectTrangThaiBaoCaoThang").selectpicker("refresh");
    
    reDrawDatepickerMonth("datepicker-month");
    $("#EditModalBaoCaoThang").modal("show");
   
}

function DeleteBaoCaoThang(id) {
    if (id > 0) {
        if (confirm("Xác nhận xóa dữ liệu ?")) {
            var frmData = new FormData();

            frmData.append("ID", id);

            KTApp.block('.blockui-loading', {});

            return new Promise(resolve => {
                $.ajax({
                    url: 'BaoCaoThang/Delete',
                    method: 'POST',
                    data: frmData,
                    contentType: false,
                    cache: false,
                    processData: false,
                    success: function (resp) {
                        if (resp.Result) {
                            toastr.success("Deleted data");
                        } else {
                            toastr.error("Delete Fail: " + resp.Messages);
                        }
                    },
                    error: function (e) {
                        toastr.error("ERROR Delete - BaoCaoThang: " + e.status + " - " + e.statusText);
                        KTApp.unblock('.blockui-loading', {});
                        if (e.status == 500 || e.status == 404) {
                            setTimeout(function () {
                                if (confirm("Bạn có muốn load lại trang và thử lại?")) {
                                    location.reload();
                                }
                            }, 2000);
                        }
                    }
                }).then(function () {
                    KTApp.unblock('.blockui-loading', {});
                    resolve('Done');
                });
            });
        }
    }
}

function DuyetBaoCaoThang(id) {
    if (id > 0) {
        
        var frmData = new FormData();

        frmData.append("ID", id);

        KTApp.block('.blockui-loading', {});

        return new Promise(resolve => {
            $.ajax({
                url: 'BaoCaoThang/ChuyenTrangThaiDuyet',
                method: 'POST',
                data: frmData,
                contentType: false,
                cache: false,
                processData: false,
                success: function (resp) {
                    if (resp.Result) {
                        toastr.success("Duyệt thành công");
                    } else {
                        toastr.error("ChuyenTrangThaiDuyet Fail: " + resp.Messages);
                    }
                },
                error: function (e) {
                    toastr.error("ERROR ChuyenTrangThaiDuyet - BaoCaoThang: " + e.status + " - " + e.statusText);
                    KTApp.unblock('.blockui-loading', {});
                    if (e.status == 500 || e.status == 404) {
                        setTimeout(function () {
                            if (confirm("Bạn có muốn load lại trang và thử lại?")) {
                                location.reload();
                            }
                        }, 2000);
                    }
                }
            }).then(function () {
                KTApp.unblock('.blockui-loading', {});
                resolve('Done');
            });
        });
    }
}

async function DeleteEdit() {
    KTApp.block('.blockui-loading', {});
    let result = await DeleteBaoCaoThang(baoCaoThangViewEdit.ID);
    KTApp.unblock('.blockui-loading', {});
    $("#EditModalBaoCaoThang").modal("hide");
}