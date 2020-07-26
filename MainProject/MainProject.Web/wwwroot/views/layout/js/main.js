var mainModel = new Vue({
    vuetify: new Vuetify(),
    el: '#app',
    mixins: [viewModel],
    data: () => ({
        drawer: null,
        pendingRequest: null,
        snackbar: {
            show: false,
            type: 'error',
            timeout: 5000,
            message: null
        },
        notificationsQueue: [],
        changePasswordDialog: {
            show: null,
            oldPassword: null,
            newPassword: null,
            confirmedPassword: null
        },
        userRoles: window.userRoles,
        utils: window.utilities
    }),
    created: function () {
        var self = this;
        axios.interceptors.request.use(
            function (request) {
                if (request.method === 'post') {
                    request.headers['RequestVerificationToken'] = aftoken;
                }
                self.pendingRequest = true;
                return request;
            },
            function (error) {
                self.pendingRequest = false;
                return Promise.reject(error);
            });
        axios.interceptors.response.use(
            function (response) {
                if (response.data.error) {
                    self.pushErrorNotification(response.data.errorMessage);
                }
                self.pendingRequest = false;
                return response;
            },
            function (error) {
                self.pushErrorNotification('Error. Operation could not be completed.');
                self.pendingRequest = false;
                return Promise.reject(error);
            });

        var push = function (type, message) {
            if (!self.utils.isNullOrEmpty(message) && !self.notificationsQueue.find(n => n.message === message)) {
                self.notificationsQueue.push({ message: message, type: type });
                if (!self.snackbar.show) {
                    var notification = self.notificationsQueue.shift();
                    self.snackbar.message = notification.message;
                    self.snackbar.type = notification.type;
                    self.snackbar.show = true;
                }
            }
        };
        self.pushSuccessNotification = function (message) {
            push('success', message);
        };
        self.pushErrorNotification = function (message) {
            push('error', message);
        };
    },
    methods: {
        closeChangePasswordDialog: function () {
            var self = this;
            self.changePasswordDialog.show = false;
        },
        changePassword: function () {
            var self = this;
            if (self.$refs.changePasswordForm.validate()) {
                axios.post('/accounts/changepassword', {
                    oldPassword: self.utils.tryGet(() => self.changePasswordDialog.oldPassword),
                    newPassword: self.utils.tryGet(() => self.changePasswordDialog.newPassword)
                })
                    .then(response => response.data)
                    .then(function (response) {
                        if (!response.error) {
                            self.closeChangePasswordDialog();
                        }
                    });
            }
        }
    },
    watch: {
        snackbar: {
            deep: true,
            handler: function () {
                var self = this;
                if (self.notificationsQueue.length && !self.snackbar.show) {
                    self.$nextTick(() => {
                        var notification = self.notificationsQueue.shift();
                        self.snackbar.message = notification.message;
                        self.snackbar.type = notification.type;
                        self.snackbar.show = true;
                    });
                }
            }
        },
        changePasswordDialog: {
            deep: true,
            handler: function () {
                var self = this;
                if (!self.changePasswordDialog.show) {
                    self.utils.resetProperties(self.changePasswordDialog);
                    self.$refs.changePasswordForm.reset();
                }
            }
        }
    },
    props: {
        source: String
    }
});
