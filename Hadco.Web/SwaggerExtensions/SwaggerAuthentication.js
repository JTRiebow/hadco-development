
$(document).ready(function() {
    if ($('body').html().search('login-form') < 0) {
        $('#header').append("<div id='modal-background'></div>" +
            "<div id='modal-content'>" +
                "<p>Login</p>" +
                "<form id='login-form'>" +
                    "<p>Username: <input name='username' type='text' autofocus/></p>" +
                    "<p>Password: <input type='password' name='password'/></p>" +
                    "<div class='btn-container'> " +
                        "<button type='submit' id='login' class='btn-login'>Login </button>" +
                        "<button type='button' id='cancel' class='btn-login'>Cancel </button>" +
                    "</div>" +
                "</form>" +
             "</div>");

        $("#header").append("<div class='swagger-ui-wrap'><a class='btn-login' id='top-login' href='javascript:void(0);'>Login</a></div>");

        $('#login-form').on('submit', function() {
            var username = $('input[name=username]').val();
            var password = $('input[name=password]').val();
            $.ajax({
                type: "POST",
                url: "/token",
                async: false,
                data: "grant_type=password&scope=read&username=" + username + "&password=" + password,
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            }).success(function(response) {
                swaggerUi.api.clientAuthorizations.add("key", new SwaggerClient.ApiKeyAuthorization("Authorization", "Bearer " + response.access_token, "header"));
                $("#top-login").text("Logout");
                $("#modal-content, #modal-background").toggleClass("active");
            }).error(function(response) {
                alert("Login failed, Please check your username or password.");
            });
            return false;
        });

        $("#top-login").on('click', function() {
            if ($("#top-login").text() == "Logout") {
                window.location.reload();
            }
 else {
                $("#modal-content, #modal-background").toggleClass("active");
            }
        });

        $("#cancel").on('click', function() {
            $("#modal-content, #modal-background").toggleClass("active");
        });
    }
});


