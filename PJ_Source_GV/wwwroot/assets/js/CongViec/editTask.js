
var fromEdit = {};
var listDuAn = [];
var listGiangVien = [];
var listThanhVienQuanLy = [];
$(document).ready(function () {
    LoadDuAn();

    KTApp.unblock('.blockui-loading', {});
    if (!$("#editTaskContent").html() && $("#editTaskContent").html() == "Không lấy được form edit") {
        toastr.error("ERROR TaskManager - FormEdit: Không lấy được form edit");
    } else {
        $("#selProject").change(function () {
            let idDuAnChon = $(this).val();

            let duAnChon = listDuAn.find(e => e.ID == idDuAnChon);
            if (duAnChon
                && duAnChon.ID > 0
                && duAnChon.NhomID
                && listNhomDuAn
                && listNhomDuAn.length > 0) {
                let nhomDamNhiem = listNhomDuAn.find(e => e.ID == duAnChon.NhomID);
                console.log("nhomDamNhiem", nhomDamNhiem);
                if (nhomDamNhiem && nhomDamNhiem.ID > 0) {
                    nhomDamNhiemDuAnThaoTac = nhomDamNhiem;
                    LoadLevel(null, !nhomDamNhiem.IsTinhDiemLevel);
                    LoadType(null, !nhomDamNhiem.IsTinhDiemTyple);
                    LoadDeadline(null, !nhomDamNhiem.IsTinhDiemDeadline);

                    if (!nhomDamNhiem.IsTinhDiemLevel && !nhomDamNhiem.IsTinhDiemTyple && !nhomDamNhiem.IsTinhDiemDeadline) {
                        $("#inputScoreGroup").hide("fast");
                        $("#inputScore").val(0);
                    } else {
                        $("#inputScoreGroup").show("fast");

                    }
                } else {
                    $("#inputScoreGroup").show("fast");

                    nhomDamNhiemDuAnThaoTac = null;
                    LoadLevel();
                    LoadType();
                    LoadDeadline();
                }
            }
            else {
                $("#inputScoreGroup").show("fast");

                nhomDamNhiemDuAnThaoTac = null;
                LoadLevel();
                LoadType();
                LoadDeadline();
            }
            LoadThanhVien();

        });

        $("#selectTrangThai").change(function () {
            $("#done-time-input").val(null);
            if ($("#selectTrangThai").val() == 4 || $("#selectTrangThai").val() == 3) {
                $("#selDateDone").show();
            } else {
                $("#selDateDone").hide();
            }
        });

        $("#selType, #selLevel, #selDeadlineID").change(function () {
            CalculatorPoint();
        });

        reDrawDatepicker('datepicker');

        fromEdit = FormValidation.formValidation(
            document.getElementById('editTask_form'),
            {
                fields: {
                    inputTieuDe: {
                        validators: {
                            notEmpty: {
                                message: "Không được bỏ trống nội dung"
                            },
                            stringLength: {
                                min: 5,
                                max: 100,
                                message: 'Tiêu đều phải có từ 5 đến tối đa 100 ký tự'
                            }
                        },
                        
                    },
                    selProject: {
                        validators: {
                            notEmpty: {
                                message: 'Phải chọn Dự án'
                            },
                        }
                    }

                    //selDeadlineID: {
                    //    validators: {
                    //        notEmpty: {
                    //            message: 'Phải chọn Deadline'
                    //        },
                    //    }
                    //},
                },

                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    bootstrap: new FormValidation.plugins.Bootstrap({
                        //eleInvalidClass: '',
                        //eleValidClass: '',
                    })
                }
            }
        );
    }
});

async function SaveTask(isTiepTucTao) {
    fromEdit.validate().then(function (status) {
        if (status === 'Valid') {
            $("#EditModalTask").modal("hide");

            let frmData = new FormData();
            frmData.append("DuAnID", $("#selProject").val());
            frmData.append("TieuDe", $("#inputTieuDe").val());
            frmData.append("NoiDung", $("#inputContent").val());
            frmData.append("TypeID", $("#selType").val());
            frmData.append("DeadlineID", $("#selDeadlineID").val());
            frmData.append("LevelID", $("#selLevel").val());
            frmData.append("ListMaNhanVienThucHien", $("#selLecturerChange").val().join());
            frmData.append("GhiChu", $("#noteTextarea").val());
            let ngayBatDauLam = !$("#start-datetime-input").val() ? '' : $("#start-datetime-input").val().toDateDMY("/").toPost();
            frmData.append("NgayBatDauLam", ngayBatDauLam);
            frmData.append("ThoiGianBatDau", $("#start-time-input").val());
            let ngayXong = !$("#done-datetime-input").val() ? '' : $("#done-datetime-input").val().toDateDMY("/").toPost();
            frmData.append("NgayXong", ngayXong);
            frmData.append("ThoiGianXong", $("#done-time-input").val());
            let ngayDeadlineDukien = !$("#deadline-datetime-input").val() ? '' : $("#deadline-datetime-input").val().toDateDMY("/").toPost();
            frmData.append("NgayDeadlineDukien", ngayDeadlineDukien);
            frmData.append("IsImportant", $('#checkboxQuanTrong').is(":checked"));
            frmData.append("IsMucTieuChatLuongDonVi", $('#checkboxMucTieuChatLuongDonVi').is(":checked"));
            frmData.append("IsMucTieuChatLuongTruong", $('#checkboxMucTieuChatLuongTruong').is(":checked"));
            frmData.append("IsKeHoachNamHocDonVi", $('#checkboxKeHoachNamHocDonVi').is(":checked"));
            frmData.append("IsPhatSinhNgoaiKeHoach", $('#checkboxPhatSinhNgoaiKeHoach').is(":checked"));
            frmData.append("TrangThaiID", $("#selectTrangThai").val());
            frmData.append("Score", $("#inputScore").val());
            if (taskViewEdit && taskViewEdit.ListMaThanhVienDuyet && taskViewEdit.ListMaThanhVienDuyet.includes(maNV)) {
                frmData.append("IsConfirm", $('#checkboxDuyet').is(":checked"));
            }

            frmData.append("ID", taskViewEdit.ID);
            let resultSubmit = false;
            KTApp.block('.blockui-loading', {});
            $.ajax({
                url: 'TaskManager/CreateOrEditTask',
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

                        //if (isTiepTucTao) {
                        //    $("#inputTieuDe").val("");
                        //    $("#inputContent").val("")
                        //    $("#EditModalTask").modal("show");
                        //}
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
                        $("#inputTieuDe").val("");
                        $("#inputContent").val("")
                        $("#EditModalTask").modal("show");
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

function CalculatorPoint() {
    if (!nhomDamNhiemDuAnThaoTac) {
        if (!listTypes) {
            toastr.error("ERROR không có dữ liệu listTypes");
            $("#inputScore").val(0);
            return;
        }

        if (!listLevels) {
            toastr.error("ERROR không có dữ liệu listLevels");
            $("#inputScore").val(0);
            return;
        }

        if (!listDeadlines) {
            toastr.error("ERROR không có dữ liệu listDeadlines");
            $("#inputScore").val(0);
            return;
        }

        if ($("#selType").val() && $("#selLevel").val() && $("#selDeadlineID").val()) {
            let pointType = listTypes.find(e => e.ID == $("#selType").val()).Score;
            let pointLevel = listLevels.find(e => e.ID == $("#selLevel").val()).TrongSo;
            let pointDeadline = listDeadlines.find(e => e.ID == $("#selDeadlineID").val()).TrongSo;
            let point = pointType * (1 + (pointLevel + pointDeadline) / 10);
            $("#inputScore").val(point);
        } else {
            $("#inputScore").val(0);
        }
    } else {
        if (nhomDamNhiemDuAnThaoTac.IsTinhDiemTyple && !listTypes) {
            toastr.error("ERROR không có dữ liệu listTypes");
            $("#inputScore").val(0);
            return;
        }

        if (nhomDamNhiemDuAnThaoTac.IsTinhDiemLevels && !listLevels) {
            toastr.error("ERROR không có dữ liệu listLevels");
            $("#inputScore").val(0);
            return;
        }

        if (nhomDamNhiemDuAnThaoTac.IsTinhDiemDeadlines && !listDeadlines) {
            toastr.error("ERROR không có dữ liệu listDeadlines");
            $("#inputScore").val(0);
            return;
        }

        let pointType = 0;
        if ($("#selType").val()) {
            pointType = listTypes.find(e => e.ID == $("#selType").val()).Score;
        }
        
        let pointLevel = 0;
        if ($("#selLevel").val()) {
            pointLevel = listLevels.find(e => e.ID == $("#selLevel").val()).TrongSo;
        }

        let pointDeadline = 0;
        if ($("#selDeadlineID").val()) {
            pointDeadline = listDeadlines.find(e => e.ID == $("#selDeadlineID").val()).TrongSo;
        }

        let point = pointType * (1 + (pointLevel + pointDeadline) / 10);
        $("#inputScore").val(point);
       
    }
    
    return;

}

var taskViewEdit = {};
var listDuAnCuaNguoiDung = [];
var nhomDamNhiemDuAnThaoTac = {};
function ShowEditTask(taskEdit) {
    taskViewEdit = taskEdit;
    //listDuAnCuaNguoiDung = listDuAnUser;
    if(!taskEdit || taskEdit.ID <= 0) {
        $("#selProject").val('default');
        $("#selProject").selectpicker("refresh");
        $("#btnDelete").hide();

        $("#selDeadlineID").val(9);
        $("#selDeadlineID").selectpicker("refresh");

        $("#selLevel").selectpicker('val', 2);
        $("#selLevel").selectpicker("refresh");

        $("#selType").selectpicker('val', listTypes.find(t => t.SoThuTu == Math.max(...listTypes.map(e => e.SoThuTu))).ID);
        $("#selType").selectpicker("refresh");

        $("#selectTrangThai").selectpicker('val', 1);
        $("#selectTrangThai").selectpicker("refresh");

        $('#checkboxQuanTrong').prop("checked", false);
        $('#checkboxMucTieuChatLuongDonVi').prop("checked", false);
        $('#checkboxMucTieuChatLuongTruong').prop("checked", false);
        $('#checkboxKeHoachNamHocDonVi').prop("checked", false);
        $('#checkboxPhatSinhNgoaiKeHoach').prop("checked", false);
        $("#start-time-input").val(null);
        $("#done-time-input").val(null);

        $("#btnTaoMoiLienTuc").show();

        CalculatorPoint();

    } else {
        $("#selProject").selectpicker('val', taskEdit.DuAnID);

        LoadDeadline(taskEdit.DeadlineID);
        LoadLevel(taskEdit.LevelID);
        LoadType(taskEdit.TypeID);

        //setTimeout(function () {
        //    $("#selType").selectpicker('val', );

        //    $("#selDeadlineID").selectpicker('val', );

        //    $("#selLevel").selectpicker('val', );
        //}, 500);
        

        $("#selectTrangThai").selectpicker('val', taskEdit.TrangThaiID);

        $('#checkboxQuanTrong').prop("checked", taskEdit.IsImportant);
        $('#checkboxMucTieuChatLuongDonVi').prop("checked", taskEdit.IsMucTieuChatLuongDonVi);
        $('#checkboxMucTieuChatLuongTruong').prop("checked", taskEdit.IsMucTieuChatLuongTruong);
        $('#checkboxKeHoachNamHocDonVi').prop("checked", taskEdit.IsKeHoachNamHocDonVi);
        $('#checkboxPhatSinhNgoaiKeHoach').prop("checked", taskEdit.IsPhatSinhNgoaiKeHoach);

        $("#checkboxDuyet").prop("checked", taskEdit.IsConfirm);
        
        $("#inputScore").val(taskEdit.Score);

        $("#btnDelete").show();

        $("#btnTaoMoiLienTuc").hide();

    }

    $("#inputContent").val(!taskEdit && !taskEdit.NoiDung ? "" : taskEdit.NoiDung);
    $("#inputTieuDe").val(!taskEdit && !taskEdit.TieuDe ? "" : taskEdit.TieuDe);

    if (!taskEdit || !taskEdit.ListMaNhanVienThucHien) {
        if (!taskEdit || taskEdit.ID <= 0) {
            $("#selLecturerChange").selectpicker('val', maNV);
        }
    }

    $("#noteTextarea").val(!taskEdit && !taskEdit.GhiChu ? "" : taskEdit.GhiChu);

    if (!taskEdit || !taskEdit.NgayBatDauLam) {
        let now = new Date();
        
        $("#start-datetime-input").val(now.toStringDMY("/"));
    } else {
        let ngayBatDauLam = new Date(taskEdit.NgayBatDauLam);
        if (!ngayBatDauLam) {
            $("#start-datetime-input").val(null);
            $("#start-time-input").val(null);
        } else {
            $('#start-datetime-input').datepicker("setDate", ngayBatDauLam);
            $("#start-time-input").val(taskEdit.NgayBatDauLam.slice(11, 16));
        }
    }

    if (!taskEdit || !taskEdit.NgayXong) {
        $("#done-datetime-input").val(null);
    } else {
        let ngayXong = new Date(taskEdit.NgayXong);
        if (!ngayXong) {
            $("#done-datetime-input").val(null);

        } else {
            $("#done-datetime-input").datepicker("setDate", ngayXong);
            $("#done-time-input").val(taskEdit.NgayXong.slice(11,16));

        }
    }

    if (!taskEdit || !taskEdit.NgayDeadlineDukien) {
        $("#deadline-datetime-input").val(null);
    } else {
        let ngayDeadlineDukien = new Date(taskEdit.NgayDeadlineDukien);
        if (!ngayDeadlineDukien) {
            $("#deadline-datetime-input").val(null);

        } else {
            $("#deadline-datetime-input").val(ngayDeadlineDukien.toStringDMY("/"));
        }
    }

    if (taskEdit.ID > 0 && taskEdit.ListMaThanhVienDuyet.includes(maNV)) {
        //$("#inputDuyet").show();
        $("#checkboxDuyet").prop('disabled', false);
        $("#inputScore").prop('disabled', false);

        $("#btnDelete").show();

    } else {
        //$("#inputDuyet").hide();
        $("#checkboxDuyet").prop('disabled', true);
        $("#inputScore").prop('disabled', taskEdit.IsConfirm);

    }

    if ($("#selectTrangThai").val() == 4 || $("#selectTrangThai").val() == 3) {
        $("#selDateDone").show();
    } else {
        $("#selDateDone").hide();
    }

    LoadThanhVien();
    $("#EditModalTask").modal("show");
   
}

function LoadLevel(idSelect,isAnKhongHienThi) {
    if (isAnKhongHienThi) {
        $("#selLevel").html('');
        $("#selLevel").selectpicker('refresh');
        $("#selLevelGroup").hide('fast');
        listLevels = [];
        fromEdit.removeField("selLevel");
        var settinValidators = {}
        

        return;
    }

    fromEdit.addField("selType", {
        validators: {
            notEmpty: {
                message: 'Phải chọn Cấp độ'
            }
        }
    });
    $("#selLevelGroup").show('fast');

    KTApp.block('.blockui-loading', {});
    $.ajax({
        url: 'DictionaryManager/GetAllLevelsByDuAnID',
        method: 'post',
        data: { DuAnID: $("#selProject").val() },
        success: function (resp) {
            let option = "";
            if (resp.Result) {
                resp.Data.forEach(e => option += `<option value="${e.ID}">${e.TenLevel}</option>`);
                listLevels = resp.Data;
            } else {
                toastr.error("Lỗi không tải được dữ liệu Level");
                listLevels = [];
            }

            $("#selLevel").html(option);
            $("#selLevel").selectpicker('refresh');

            if (listLevels.length > 0 && !idSelect) {
                if (!taskViewEdit.ID) {
                    $("#selLevel").selectpicker('val', 2);
                } else {
                    $("#selLevel").selectpicker('val', listLevels[0].ID);
                }
            } else {
                $("#selLevel").selectpicker('val', idSelect);
            }

            $("#selLevel").selectpicker('refresh');

        },
        error: function (e) {
            $("#selLevel").html('');
            $("#selLevel").selectpicker('refresh');
            listLevels = [];

            toastr.error("ERROR GetAllLevelsByDuAnID - DictionaryManager: " + e.status + " - " + e.statusText);
            
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
    });
}

function LoadType(idSelect, isAnKhongHienThi) {
    if (isAnKhongHienThi) {
        $("#selType").html('');
        $("#selType").selectpicker('refresh');
        $("#selTypeGroup").hide('fast');
        fromEdit.removeField("selType");
        listLevels = [];
        return;
    }

    fromEdit.addField("selType", {
        validators: {
            notEmpty: {
                message: 'Phải chọn Loại'
            }
        }
    });
    $("#selTypeGroup").show("fast")
    KTApp.block('.blockui-loading', {});

    $.ajax({
        url: 'DictionaryManager/GetAllTypesByDuAnID',
        method: 'post',
        data: { DuAnID: $("#selProject").val() },
        success: function (resp) {
            let option = "";
            if (resp.Result) {
                resp.Data.forEach(e => option += `<option value="${e.ID}">${e.TenType}</option>`);
                listTypes = resp.Data;
            } else {
                toastr.error("Lỗi không tải được dữ liệu Type");
                listTypes = [];
            }

            $("#selType").html(option);
            $("#selType").selectpicker('refresh');

            if (listTypes.length > 0 && !idSelect) {
                if (!taskViewEdit.ID) {
                    $("#selType").selectpicker('val', listTypes.find(t => t.SoThuTu == Math.max(...listTypes.map(e => e.SoThuTu))).ID);
                } else {
                    $("#selType").selectpicker('val', listTypes[0].ID);
                }
                
            } else {
                $("#selType").selectpicker('val', idSelect);
            }
            $("#selType").selectpicker('refresh');

        },
        error: function (e) {
            $("#selType").html('');
            $("#selType").selectpicker('refresh');
            listTypes = [];

            toastr.error("ERROR GetAllTypesByDuAnID - DictionaryManager: " + e.status + " - " + e.statusText);

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
    });
}

function LoadDeadline(idSelect, isAnKhongHienThi) {
    if (isAnKhongHienThi) {
        $("#selDeadlineID").html('');
        $("#selDeadlineID").selectpicker('refresh');
        $("#selDeadlineGroup").hide('fast');
        fromEdit.removeField("selDeadlineID");
        listLevels = [];
        return;
    }

    fromEdit.addField("selDeadlineID", {
        validators: {
            notEmpty: {
                message: 'Phải chọn Deadline'
            }
        }
    });
    $("#selDeadlineGroup").show('fast');
    KTApp.block('.blockui-loading', {});
    $.ajax({
        url: 'DictionaryManager/GetAllDeadlinesByDuAnID',
        method: 'post',
        data: { DuAnID: $("#selProject").val() },
        success: function (resp) {
            let option = "";
            if (resp.Result) {
                resp.Data.forEach(e => option += `<option value="${e.ID}">${e.TenDeadline}</option>`);
                listDeadlines = resp.Data;
            } else {
                toastr.error("Lỗi không tải được dữ liệu Deadline");
                listDeadlines = [];
            }

            $("#selDeadlineID").html(option);
            $("#selDeadlineID").selectpicker('refresh');

            if (listDeadlines.length > 0 && !idSelect) {
                if (!taskViewEdit.ID) {
                    $("#selDeadlineID").selectpicker('val', 9);
                } else {
                    $("#selDeadlineID").selectpicker('val', listDeadlines[0].ID);
                }
            } else {
                $("#selDeadlineID").selectpicker('val', idSelect);
            }
            $("#selDeadlineID").selectpicker('refresh');

        },
        error: function (e) {
            $("#selDeadlineID").html('');
            $("#selDeadlineID").selectpicker('refresh');
            listDeadlines = [];

            toastr.error("ERROR GetAllDeadlinesByDuAnID - DictionaryManager: " + e.status + " - " + e.statusText);

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
    });
}

function LoadDuAn() {
    KTApp.block('.blockui-loading', {});
    $.ajax({
        url: 'Home/GetListDuAnThamGia',
        method: 'post',
        success: function (resp) {
            listDuAnCuaNguoiDung = resp.ListDuAn;
            listThanhVienQuanLy = resp.ListNhanVienQuanLy;
            
        },
        error: function (e) {
            toastr.error("ERROR GetListDuAnThamGia - Home: " + e.status + " - " + e.statusText);

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
    });
}