$(document).ready(function () {
    $(".selectpicker").selectpicker().on('loaded.bs.select', function () {
        let that = $(this).data('selectpicker'),
            inner = that.$menu.children('.inner');
        $(".divider, .dropdown-header").find("span.text").append(' <div class="btn-group btn-group-sm btn-block"><button type="button" class="actions-btn bs-select-all btn btn-default">' + that.options.selectAllText + '</button>'
            + '&nbsp;&nbsp;<button type="button" class="actions-btn bs-deselect-all btn btn-default">' + that.options.deselectAllText + '</button></div>');
        
        // remove default event
        inner.off('click', '.divider, .dropdown-header button');
        // add new event
        inner.on('click', '.divider, .dropdown-header button', function (e) {
            // original functionality
            e.preventDefault();
            e.stopPropagation();

            if (that.options.liveSearch) {
                that.$searchbox.trigger('focus');
            } else {
                that.$button.trigger('focus');
            }

            // extended functionality
            let position0 = that.isVirtual() ? that.selectpicker.view.position0 : 0,
                clickedData = that.selectpicker.current.data[$(this).parents(".divider, .dropdown-header").index() + position0];
            var $selectOptions = that.$element.find('option');
            var selected = $(this).hasClass("bs-select-all");

            that.$element.addClass('bs-select-hidden');

            for (var i = 0; i < that.selectpicker.current.elements.length; i++) {
                var liData = that.selectpicker.current.data[i],
                    index = that.selectpicker.current.map.originalIndex[i], // faster than $(li).data('originalIndex')
                    option = $selectOptions[index];

                if (option && !option.disabled && liData.type !== 'divider' && liData.optID === clickedData.optID) {
                    option.selected = selected;
                }
            }

            that.$element.removeClass('bs-select-hidden');

            that.setOptionStatus();
            that.$element.triggerNative('change');
        });
    });
});