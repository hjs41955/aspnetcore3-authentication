var config = {
    userStore: new Oidc.WebStorageStateStore({ store: window.localStorage }),       //added in ep16  (telling it where to read it from)
    authority: "https://localhost:44305/",
    client_id: "client_id_js",
    redirect_uri: "https://localhost:44345/Home/SignIn",
    //response_type: "id_token token",                                              //removed in ep20
    post_logout_redirect_uri: "https://localhost:44345/Home/Index",                 //added in ep18
    response_type: "code",                                                          //added in ep20
    scope: "openid rc.scope ApiOne ApiTwo"      //added in ep16
};

var userManager = new Oidc.UserManager(config);

var signIn = function () {
    userManager.signinRedirect();
};

var signOut = function() {                      //added in ep18
    userManager.signoutRedirect();
};

userManager.getUser().then(user => {
    console.log("user:", user);
    if (user) {
        axios.defaults.headers.common["Authorization"] = "Bearer " + user.access_token;
    }
});

var callApi = function () {
    axios.get("https://localhost:44337/secret")
        .then(res => {
            console.log(res);
        });
};

var refreshing = false;     //added in ep16

axios.interceptors.response.use(                //added in ep16
    function (response) { return response; },
    function (error) {
        console.log("axios error:", error.response);

        var axiosConfig = error.response.config;

        //if error response is 401 try to refresh token
        if (error.response.status === 401) {
            console.log("axios error 401");

            // if already refreshing don't make another request
            if (!refreshing) {
                console.log("starting token refresh");
                refreshing = true;

                // do the refresh
                return userManager.signinSilent().then(user => {    //this userManager is smart enough to know if the user ever logged on. If this silent is called, it uses the user name and pwd previously entered to sign in (and calls the IdentityServer to get the new tokens) without user entering again
                    console.log("new user:", user);
                    //update the http request and client
                    axios.defaults.headers.common["Authorization"] = "Bearer " + user.access_token;     //this is for the baseline
                    axiosConfig.headers["Authorization"] = "Bearer " + user.access_token;               //this is for the current request
                    //retry the http request
                    return axios(axiosConfig);
                });
            }
        }

        return Promise.reject(error);
    });