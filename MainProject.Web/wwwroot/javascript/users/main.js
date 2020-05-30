var viewModel = {
    data: () => ({
        usersTable: {
            filters: {
                searchQuery: null,
                roleId: null
            },
            loading: false,
            pagination: {
                sortBy: 'role.name',
                descending: false,
                page: 1,
                rowsPerPage: 5
            },
            totalItemCount: 0,
            itemsList: [],
            headers: [
                { text: 'Acciones', align: 'left', sortable: false, width: '10%' },
                { text: 'Índice', align: 'left', sortable: false },
                { text: 'Nombre', align: 'left', value: 'firstname', width: '15%' },
                { text: 'Apellido', align: 'left', value: 'lastname', width: '15%' },
                { text: 'Email', align: 'left', value: 'email' },
                { text: 'Rol', align: 'left', value: 'role.name' },
                { text: 'Estado', align: 'left', value: 'isdisabled' }
            ]
        },
        usersRoleList: [],
        userDialog: {
            show: null,
            readonly: null,
            deletion: null,
            data: {
                id: null,
                firstName: null,
                lastName: null,
                email: null,
                role: null,
                isDisabled: null
            }
        }
    }),
    mounted: function () {
        var self = this;
        self.getUsersList = _.debounce(function () {
            var self = this;
            self.usersTable.loading = true;

            const { searchQuery, roleId } = self.usersTable.filters;
            const { sortBy, descending, page, rowsPerPage } = self.usersTable.pagination;

            var parameters = {
                searchQuery: self.utils.tryGet(() => searchQuery),
                orderByColumn: self.utils.tryGet(() => sortBy ? `${sortBy} ${descending ? 'desc' : 'asc'}` : null),
                page: self.utils.tryGet(() => page),
                pageSize: self.utils.tryGet(() => rowsPerPage),
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
        self.getUsersRoleList();
    },
    methods: {
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
        showUserDialog: function (user, readonly, deletion) {
            var self = this;
            if (user) {
                self.userDialog.data.id = user.id;
                self.userDialog.data.firstName = user.firstName;
                self.userDialog.data.lastName = user.lastName;
                self.userDialog.data.isDisabled = user.isDisabled;
                self.userDialog.data.email = user.email;
                self.userDialog.data.role = self.usersRoleList.find(x => x.id === user.role.id);
            }

            self.userDialog.deletion = deletion;
            self.userDialog.readonly = readonly;
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
                        id: self.utils.tryGet(() => self.userDialog.data.id),
                        firstName: self.utils.tryGet(() => self.userDialog.data.firstName),
                        lastName: self.utils.tryGet(() => self.userDialog.data.lastName),
                        email: self.utils.tryGet(() => self.userDialog.data.email),
                        isDisabled: self.utils.tryGet(() => self.userDialog.data.isDisabled),
                        role: self.utils.tryGet(() => self.userDialog.data.role.name)
                    };

                    var url = (function () {
                        if (self.userDialog.data.id) {
                            if (self.userDialog.deletion) {
                                return '/users/deleteregistereduser';
                            }
                            else {
                                return '/users/editregistereduser';
                            }
                        }
                        else {
                            return '/users/registernewuser';
                        }
                    }).call();
                    var response = await axios.post(url, bodyData);

                    if (!response.data.error) {
                        self.closeUserDialog();
                        self.getUsersList();
                    }
                }
            }
            catch (error) {
                self.pushErrorNotification('Error. No es posible completar la operación.');
            }
        }
    },
    watch: {
        'usersTable.filters': {
            deep: true,
            handler: function () {
                var self = this;
                self.getUsersList();
            }
        },
        'usersTable.pagination': {
            deep: true,
            handler: function () {
                var self = this;
                self.getUsersList();
            }
        }
    }
};