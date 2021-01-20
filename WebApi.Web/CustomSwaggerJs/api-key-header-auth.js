(function () {
    $(function () {
        //$('#input_apiKey').show();
        //$('#input_apiKey').on('change', function () {
        //    var key = this.value;
            
        //    if (key && key.trim() !== '') {
        //        swaggerUi.api.clientAuthorizations.add("key", new SwaggerClient.ApiKeyAuthorization("Authorization", "BASIC " + key, "header"));
        //    }
        //});
        $('#input_apiKey').hide();
        var basicAuthUI =
         '<div class ="input"> Token:<input placeholder ="token" id ="input_token" name ="token" type ="text" size ="35"> </ div>';
        $('#api_selector').html(basicAuthUI);

        $('#input_token').on('change', function () {
            var key = this.value;

            if (key && key.trim() !== '') {
                swaggerUi.api.clientAuthorizations.add("key", new SwaggerClient.ApiKeyAuthorization("Authorization", "BASIC " + key, "header"));
            }
        });
    });
})();

