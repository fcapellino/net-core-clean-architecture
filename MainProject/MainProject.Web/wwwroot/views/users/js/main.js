var viewModel = {
    data: () => ({
        usersTable: {
            filters: {
                searchQuery: null,
                roleId: null
            },
            loading: false,
            options: {},
            totalItemCount: 0,
            itemsList: [],
            headers: [
                { text: 'Actions', value: '[actions]', sortable: false },
                { text: 'Id', value: '[id]', sortable: false },
                { text: 'Name', value: '[firstname]', sortable: true },
                { text: 'Surname', value: '[lastname]', sortable: true },
                { text: 'Email', value: '[email]', sortable: true },
                { text: 'Role', value: '[role.name]', sortable: true },
                { text: 'State', value: '[isdisabled]', sortable: true }
            ]
        },
        usersRoleList: [],
        userDialog: {
            show: null,
            readonly: null,
            deletion: null,
            data: {
                taskId: null,
                id: null,
                firstName: null,
                lastName: null,
                email: null,
                role: null,
                isDisabled: null
            }
        },
        loaderDialog: {
            show: null,
            progressValue: null,
            progressMessage: null
        }
    }),
    mounted: function () {
        var self = this;
        self.getUsersList.call();
        self.getUsersRoleList.call();
    },
    methods: {
        getUsersList: function () {
            var self = this;
            if (self.utils.tryGet(() => !window.getUsersListDebounced)) {
                window.getUsersListDebounced = _.debounce(function () {
                    self.usersTable.loading = true;

                    const { searchQuery, roleId } = self.usersTable.filters;
                    const { sortBy, sortDesc, page, itemsPerPage } = self.usersTable.options;

                    var parameters = {
                        searchQuery: self.utils.tryGet(() => searchQuery),
                        orderByColumn: self.utils.tryGet(() => sortBy ? `${[...sortBy].shift().replace(/[\[\]']+/g, '')} ${[...sortDesc].shift() ? 'desc' : 'asc'}` : null),
                        page: self.utils.tryGet(() => page),
                        pageSize: self.utils.tryGet(() => itemsPerPage),
                        roleId: self.utils.tryGet(() => roleId)
                    };

                    axios.get('/users/getuserslist', {
                        params: parameters
                    })
                        .then(response => response.data)
                        .then(function (response) {
                            if (!response.error) {
                                var resources = response.resources;
                                self.usersTable.totalItemCount = resources.totalItemCount;
                                self.usersTable.itemsList = resources.itemsList;
                            }
                        })
                        .catch(error => {
                            console.log(error);
                        })
                        .finally(function () {
                            self.usersTable.loading = false;
                        });
                }, 300);
            }
            window.getUsersListDebounced.call();
        },
        getUsersRoleList: function () {
            var self = this;
            axios.get('/users/getusersrolelist', {})
                .then(response => response.data)
                .then(function (response) {
                    if (!response.error) {
                        var resources = response.resources;
                        self.usersRoleList = resources.itemsList;
                    }
                })
                .catch(error => {
                    console.log(error);
                });
        },
        getItemIndex: function (item) {
            var self = this;
            var row = self.usersTable.itemsList.findIndex(u => u.id === item.id) + 1;
            return ((self.usersTable.options.page - 1) * self.usersTable.options.itemsPerPage + row).toString().padStart(3, '0');
        },
        showUserDialog: function (options) {
            var self = this;
            if (self.utils.tryGet(() => options.user)) {
                self.userDialog.data.id = options.user.id;
                self.userDialog.data.firstName = options.user.firstName;
                self.userDialog.data.lastName = options.user.lastName;
                self.userDialog.data.isDisabled = options.user.isDisabled;
                self.userDialog.data.email = options.user.email;
                self.userDialog.data.role = self.usersRoleList.find(x => x.id === options.user.role.id);
            }

            self.userDialog.data.taskId = self.utils.generateGuid();
            self.userDialog.deletion = self.utils.tryGet(() => options.deletion);
            self.userDialog.readonly = self.utils.tryGet(() => options.readonly);
            self.userDialog.show = true;
        },
        closeUserDialog: function () {
            var self = this;
            self.userDialog.show = null;
            self.userDialog.deletion = null;
            self.userDialog.readonly = null;
            self.utils.resetProperties(self.userDialog.data);
            self.$refs.userDataForm.reset();
        },
        executeUserDialogRequest: async function () {
            var self = this;
            try {
                if (self.$refs.userDataForm.validate()) {
                    var bodyData = {
                        taskId: self.utils.tryGet(() => self.userDialog.data.taskId),
                        id: self.utils.tryGet(() => self.userDialog.data.id),
                        firstName: self.utils.tryGet(() => self.userDialog.data.firstName),
                        lastName: self.utils.tryGet(() => self.userDialog.data.lastName),
                        email: self.utils.tryGet(() => self.userDialog.data.email),
                        isDisabled: self.utils.tryGet(() => self.userDialog.data.isDisabled),
                        role: self.utils.tryGet(() => self.userDialog.data.role.name)
                    };

                    var endpoints = function () {
                        return self.userDialog.data.id ? self.userDialog.deletion ?
                            {
                                nhub: "delete-registered-user-request",
                                post: "/users/deleteregistereduser"
                            } :
                            {
                                nhub: "edit-registered-user-request",
                                post: "/users/editregistereduser"
                            } :
                            {
                                nhub: "register-new-user-request",
                                post: "/users/registernewuser"
                            };
                    }.call();
                    self.showLoaderDialog();

                    var connection = new signalR.HubConnectionBuilder()
                        .withUrl("/notificationshub")
                        .configureLogging(signalR.LogLevel.Information)
                        .build();

                    connection.on(endpoints.nhub,
                        (percentage, message) => {
                            self.loaderDialog.progressValue = percentage;
                            self.loaderDialog.progressMessage = message;
                        });

                    await connection.start();
                    await connection.invoke("associatetask", self.utils.tryGet(() => self.userDialog.data.taskId));

                    if (connection.state === signalR.HubConnectionState.Connected) {
                        var response = await axios.post(endpoints.post, bodyData);

                        setTimeout(function () {
                            self.closeLoaderDialog();
                            if (!response.data.error) {
                                self.closeUserDialog();
                                self.getUsersList();
                            }
                        }, 1300);
                    }
                }
            }
            catch (error) {
                self.closeLoaderDialog();
                self.pushErrorNotification('Error. The operation cannot be completed.');
            }
        },
        showLoaderDialog: function () {
            var self = this;
            self.loaderDialog.progressValue = 0;
            self.loaderDialog.progressMessage = 'Initializing...';
            self.loaderDialog.show = true;
        },
        closeLoaderDialog: function () {
            var self = this;
            self.loaderDialog.show = null;
            self.utils.resetProperties(self.loaderDialog);
        }
    },
    watch: {
        'usersTable.filters': {
            deep: true,
            handler: function () {
                var self = this;
                self.usersTable.options.page = 1;
                self.getUsersList();
            }
        },
        'usersTable.options': {
            deep: true,
            handler: function () {
                var self = this;
                self.getUsersList();
            }
        }
    }
};
