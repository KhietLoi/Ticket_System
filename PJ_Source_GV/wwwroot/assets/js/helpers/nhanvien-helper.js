var NhanVienHelper = {
    bindAutoComplete: function (maNVSelector, hoTenSelector, cccdSelector) {
        // Existing MaNV handler
        $(maNVSelector).off('input.nhanvien').on('input.nhanvien', function () {
            var maNV = $(this).val().trim();
            console.log('Searching for MaNV:', maNV);

            if (!maNV) {
                $(hoTenSelector).val('');
                $(cccdSelector).val('');
                return;
            }

            $(hoTenSelector).attr('placeholder', 'Không tồn tại dữ liệu');
            $(cccdSelector).attr('placeholder', 'Không tồn tại dữ liệu');

            $.ajax({
                url: '/api/NhanVien/GetByMaNV/' + encodeURIComponent(maNV),
                method: 'GET',
                contentType: 'application/json',
                success: function (response) {
                    console.log('API Response:', response);

                    var hoTenValue = response.HoTen || response.hoTen;
                    var cccdValue = response.CCCD || response.cccd;

                    if (response && hoTenValue) {
                        $(hoTenSelector)
                            .val(hoTenValue)
                            .removeClass('is-invalid')
                            .addClass('is-valid');
                        console.log('HoTen set to:', hoTenValue);
                    } else {
                        $(hoTenSelector)
                            .val('')
                            .removeClass('is-valid')
                            .addClass('is-invalid');
                        console.log('No HoTen found in response');
                    }

                    if (response && cccdValue) {
                        $(cccdSelector)
                            .val(cccdValue)
                            .removeClass('is-invalid')
                            .addClass('is-valid');
                        console.log('CCCD set to:', cccdValue);
                    } else {
                        $(cccdSelector)
                            .val('')
                            .removeClass('is-valid')
                            .addClass('is-invalid');
                        console.log('No CCCD found in response');
                    }
                },
                error: function (xhr, status, error) {
                    console.error('API Error:', { xhr: xhr, status: status, error: error });
                    $(hoTenSelector)
                        .val('')
                        .removeClass('is-valid')
                        .addClass('is-invalid');
                    $(cccdSelector)
                        .val('')
                        .removeClass('is-valid')
                        .addClass('is-invalid');
                }
            });
        });

        // New CCCD handler
        $(cccdSelector).off('input.nhanvien').on('input.nhanvien', function () {
            var cccd = $(this).val().trim();
            console.log('Searching for CCCD:', cccd);

            if (!cccd) {
                $(hoTenSelector).val('');
                return;
            }

            $(hoTenSelector).attr('placeholder', 'Không tồn tại dữ liệu');

            $.ajax({
                url: '/api/NhanVien/GetByCCCD/' + encodeURIComponent(cccd),
                method: 'GET',
                contentType: 'application/json',
                success: function (response) {
                    console.log('API Response:', response);

                    var hoTenValue = response.HoTen || response.hoTen;

                    if (response && hoTenValue) {
                        $(hoTenSelector)
                            .val(hoTenValue)
                            .removeClass('is-invalid')
                            .addClass('is-valid');
                        console.log('HoTen set to:', hoTenValue);
                    } else {
                        $(hoTenSelector)
                            .val('')
                            .removeClass('is-valid')
                            .addClass('is-invalid');
                        console.log('No HoTen found in response');
                    }
                },
                error: function (xhr, status, error) {
                    console.error('API Error:', { xhr: xhr, status: status, error: error });
                    $(hoTenSelector)
                        .val('')
                        .removeClass('is-valid')
                        .addClass('is-invalid');
                }
            });
        });

        // Handle paste events for both fields
        $(maNVSelector + ', ' + cccdSelector).on('paste', function (e) {
            setTimeout(function () {
                $(e.target).trigger('input');
            }, 0);
        });

        console.log('AutoComplete bound to:', maNVSelector, 'and', cccdSelector);
    }
};