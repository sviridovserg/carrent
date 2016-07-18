// Write your Javascript code.
var utils = {};
(function () {
    utils.bindCarBrandsAndModels = function () {
        function showBrandModels(brand) {
            if (!brand) {
                $('#modelList>option').show();
                return;
            }
            $('#modelList').val(null);
            $('#modelList>option').hide();
            $('#modelList>option[brand-id=' + brand + ']').show();
            $('#modelList>option[value=""]').show();
        }
        $(document).ready(function () {
            var selectedBrand = $('#brandList').val();
            var selectedModel = $('#modelList').val();
            if (selectedBrand) {
                showBrandModels(selectedBrand);
                $('#modelList').val(selectedModel);
            }
            $('#brandList').change(function (evt) {
                showBrandModels($('#brandList').val());
            });

        });
    };

    utils.disableElementsOnReady = function (selector) {
        $(document).ready(function () {
            $(selector).attr('disabled', true);
        });
    };

})();