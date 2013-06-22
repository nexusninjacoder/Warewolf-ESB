﻿// Make this available to chrome debugger
//@ sourceURL=SaveViewModel.js  

function SaveViewModel(saveUri, baseViewModel, saveFormID, environment) {
    var self = this;
    
    var $saveForm = $("#" + saveFormID);
    var $resourceFoldersScrollBox = $("#resourceFoldersScrollBox");
    var $resourceFoldersScrollBoxHeight = 240;
    var $resourceFolders = $("#resourceFolders");
    var $resourceName = $("#resourceName");
    var $newFolderDialog = $("#newFolderDialog");
    var $newFolderName = $("#newFolderName");
    var $dialogSaveButton = null;
    
    //2013.06.08: Ashley Lewis for PBI 9458
    var $fixedFloatingDiv = $("#SaveDialogEnvironment");
    self.currentEnvironment = ko.observable(environment);
    self.inTitleEnvironment = false;
    
    self.onSaveCompleted = null;
    self.isWindowClosedOnSave = true;
    self.viewModel = baseViewModel;
    self.data = baseViewModel.data;
    self.isEditing = ko.observable(baseViewModel.isEditing);

    //2013.06.20: Ashley Lewis for bug 9786 - default folder selection + resource name help text
    self.defaultFolderName = "UNASSIGNED";
    self.helpDictionaryID = "SaveDialog";
    self.helpDictionary = {};
    self.titleSearchString = "Service";
    self.updateHelpText = function (id) {
        var text = self.helpDictionary[id];
        text = text ? text : "";
        utils.appendSaveValidationSpan(baseViewModel.titleSearchString || self.titleSearchString, text);
    };
    $.post("Service/Help/GetDictionary" + window.location.search, self.helpDictionaryID, function (result) {
        self.helpDictionary = result;
        self.updateHelpText("default");
    });
    
    //2013.06.21: Ashley Lewis for bug 9786 - filter invalid characters
    self.attemptedResourceName = ko.computed({
        read: function () {
            return self.data.resourceName();
        },
        write: function (value) {
            if (!self.isValidName(value)) {
                self.data.resourceName(self.RemoveInvalidCharacters(value));
                self.data.resourceName.valueHasMutated();
            }
            else {
                self.data.resourceName(value);
            }
        },
        owner: self
    });

    self.resourceNames = ko.observableArray();
    self.resourceFolders = ko.observableArray();
    self.searchFolderTerm = ko.observable("");
    self.searchFolderResults = ko.computed(function () {
        var term = self.searchFolderTerm().toLowerCase();
        if (term == "") {
            return self.resourceFolders();
        }
        return ko.utils.arrayFilter(self.resourceFolders(), function (folder) {
            return folder.toLowerCase().indexOf(term) !== -1;
        });
    });    
    
    $.post("Service/Resources/PathsAndNames" + window.location.search, self.data.resourceType(), function (result) {
        if (!result.Paths) {
            self.resourceFolders([]);
        } else {
            self.resourceFolders(result.Paths);
            self.resourceFolders.sort(utils.caseInsensitiveSort);
        }
        if (!result.Names) {
            self.resourceNames([]);
        } else {
            self.resourceNames(result.Names);
            self.resourceNames.sort(utils.caseInsensitiveSort);
        }
        if (SaveViewModel.IsStandAlone) {            
            self.showDialog(true, null);        
        }
    });
    self.clearFilter = function () {
        self.searchFolderTerm("");
    };
    self.hasFilter = ko.computed(function () {
        return self.searchFolderTerm() !== "";
    });
    utils.makeClearFilterButton("clearSaveFilterButton");

    self.isValidName = function (name) {
        var result = /^[a-zA-Z0-9._\s-]+$/.test(name);
        return result;
    }; 

    self.RemoveInvalidCharacters = function (name) {
        var newName = name;
        while (!self.isValidName(newName) && newName !== "") {
            newName = newName.replace(/[^a-zA-Z0-9._\s-]/, "");
        }
        if (newName == "") {
            self.updateHelpText("default");
        }
        return newName;
    };

    self.enableSaveButton = function (enabled) {
        if ($dialogSaveButton) {
            $dialogSaveButton.button("option", "disabled", !enabled);
        }
    };

    self.isNewFolderNameValid = function () {
        var name = $newFolderName.val().toLowerCase();
        var isValid = false;
        if (name !== "" && self.isValidName(name)) {
            var matches = ko.utils.arrayFilter(self.resourceFolders(), function (folder) {
                return folder.toLowerCase() === name;
            });
            isValid = matches.length == 0;
        }
        self.enableNewFolderOkButton(isValid);
        return isValid;
    };      

    self.isResourceNameValid = function () {
        var name = self.data.resourceName() != null ? self.data.resourceName().toLowerCase() : "";
        var isValid = name !== "" && self.isValidName(name);
        if (!self.isEditing() && isValid) {
            // check for duplicates
            var matches = ko.utils.arrayFilter(self.resourceNames(), function (resourceName) {
                return resourceName.toLowerCase() === name;
            });
            if (matches.length == 0) {
                isValid = true;
            } else {
                isValid = false;
                self.updateHelpText("DuplicateFound");
        }
        }
        self.enableSaveButton(isValid);
        return isValid;
    };    

    $newFolderName.keyup(function (e) {
        if (self.isNewFolderNameValid()) {
            // ENTER key pressed
            if (e.keyCode == 13) {
                $newFolderDialog.dialog("close");
                self.addNewFolder();
            }
        }
    }).keydown(function (e) {
        if (e.which == 13) {
            e.preventDefault();
        }
    });
    
    $resourceName.keyup(function (e) {
        // ENTER key pressed
        if (e.keyCode == 13) {
            self.save();
        }
    }).keydown(function (e) {
        if (e.which == 13) {
            e.preventDefault();
        }
    });

    self.newFolder = function () {
        $newFolderName.val("");
        self.enableNewFolderOkButton(false);
        $newFolderDialog.dialog("open");
    };

    self.addNewFolder = function () {
        var folderName = $newFolderName.val();
        if (folderName !== "") {
            self.resourceFolders.push(folderName);
            self.resourceFolders.sort(self.caseInsensitiveSort);
            self.data.resourcePath(folderName);
            self.selectFolder(folderName);
        }
    };       

    self.selectFolder = function (folderName) {
        utils.selectAndScrollToListItem(folderName.toUpperCase(), $resourceFoldersScrollBox, $resourceFoldersScrollBoxHeight);
    };

    self.isFormValid = function () {
        var isValidPath = self.data.resourcePath() ? true : false;
        var isValid = isValidPath && self.isResourceNameValid();
        self.enableSaveButton(isValid);
        if (isValid) {
            self.updateHelpText("");
        }
        return isValid;
    };

    self.isSaveFormValid = ko.computed(function () {
        return self.isFormValid();
    });

    self.createNewFolderDialog = function () {
        $newFolderDialog.dialog({
            resizable: false,
            autoOpen: false,
            modal: true,
            position: utils.getDialogPosition(),
            buttons: {
                "Add Folder": function () {
                    self.addNewFolder();
                    $(this).dialog("close");
                },
                Cancel: function () {
                    $(this).dialog("close");
                }
            }
        });

        var $newFolderOkButton = $(".ui-dialog-buttonpane button:contains('Add Folder')");
        self.enableNewFolderOkButton = function (enabled) {
            $newFolderOkButton.button("option", "disabled", !enabled);
        };
    };       

    utils.registerSelectHandler($resourceFolders, function (selectedItem) {
        self.data.resourcePath(selectedItem);
    });

    self.save = function () {
        if (!self.isFormValid()) {
            return;
        }

        var jsonData = ko.toJSON(self.data);
        if (saveUri) {
            $.post(saveUri + window.location.search, jsonData, function (result) {
                if (!result.IsValid) {
                    $saveForm.dialog("close");
                    if (self.onSaveCompleted != null) {
                        self.onSaveCompleted(result);
                    }
                    if (self.isWindowClosedOnSave) {
                        studio.saveAndClose(result);
                    } else {
                        studio.save(result);
                    }
                }
            });
        } else {
            //This is done because C# escapes double qoutes
            jsonData = jsonData.replace(/"/g, "'");
            if (self.isWindowClosedOnSave) {
                studio.saveAndClose(jsonData);
            } else {
                studio.save(jsonData);
            }
        }
    };
    
    self.showDialog = function (isWindowClosedOnSave, onSaveCompleted) {
        self.isEditing(baseViewModel.isEditing);
        self.isFormValid();
        self.isWindowClosedOnSave = isWindowClosedOnSave;
        self.onSaveCompleted = onSaveCompleted;
        $saveForm.dialog("open");

        //2013.06.09: Ashley Lewis for PBI 9458 - Show server in dialog title
        if (self.currentEnvironment() && self.inTitleEnvironment == false) {
            $fixedFloatingDiv.hide();
            utils.appendSaveEnviroSpan(baseViewModel.titleSearchString || "Service", self.currentEnvironment());
            self.inTitleEnvironment = true;
        }
    };    

    self.createDialog = function () {
        $saveForm.dialog({
            resizable: false,
            autoOpen: false,
            height: 456,
            width: 600,
            modal: true,            
            position: utils.getDialogPosition(),
            open: function (event, ui) {                
                self.enableSaveButton(self.data.resourceName());
                var resourcePath = self.data.resourcePath();
                if (resourcePath) {
                    self.selectFolder(resourcePath);
                } else {
                    //2013.06.20: Ashley Lewis for bug 9786 - default folder selection
                    self.data.resourcePath(self.defaultFolderName);
                    self.resourceFolders.splice(0, 0, self.defaultFolderName);
                    self.selectFolder(self.defaultFolderName);
                }          
            },
            buttons: [{
                text: "Save",
                tabindex: 3,
                click: self.save
            }, {
                text: "Cancel",
                tabindex: 4,
                click: function () {
                    if (saveUri) {
                        $(this).dialog("close");
                    } else {
                        studio.cancel();
                    }
                }
            }]
        });

        try {
        $dialogSaveButton = $("div[aria-describedby=" + saveFormID + "] .ui-dialog-buttonpane button:contains('Save')");
        $dialogSaveButton.attr("tabindex", "105");
        $dialogSaveButton.next().attr("tabindex", "106");
        }catch(e){
            //for testing without an html front end, jquery throws exception
            if (e.message === "Syntax error, unrecognized expression: div[aria-describedby=test form] .ui-dialog-buttonpane button:contains('Save')") {
                console.log("no html front end");
            } else {
                throw e;
            }
        }
    };

    self.createDialog();
    self.createNewFolderDialog();
};

SaveViewModel.IsStandAlone = true;

SaveViewModel.create = function (saveUri, baseViewModel, containerID) {    
    // MUST get dialog content synchronously!
    $.ajax("Views/Dialogs/SaveDialog.htm", {
        async: false,        
        complete: function (jqXhr) {
            
            // MUST set this BEFORE getting html otherwise you will get standalone version!
            SaveViewModel.IsStandAlone = false;
            $("#" + containerID).html(jqXhr.responseText);
        }
    });
    
    // apply jquery-ui themes
    $("button").button();

    // ensure form id is unique
    var saveFormID = containerID + "SaveForm";
    $("#" + containerID + " #saveForm").attr("id", saveFormID);
    var saveForm = document.getElementById(saveFormID);

    var model = new SaveViewModel(saveUri, baseViewModel, saveFormID, baseViewModel.currentEnvironment());
    ko.applyBindings(model, saveForm);
    
    return model;
};


SaveViewModel.showStandAlone = function () {
    var resourceID = getParameterByName("rid");
    var resourceType = getParameterByName("type");
    var resourcePath = getParameterByName("path");
    var name = getParameterByName("name");
    var title = getParameterByName("title");

    var baseViewModel = {
        saveTitle: "<b>" + title + "" + "</b>",
        isEditing: resourceID ? resourceID != "" : false,
        data: {
            resourceID: ko.observable(resourceID),
            resourceType: ko.observable(resourceType),
            resourceName: ko.observable(name),
            resourcePath: ko.observable(resourcePath)
        },
        titleSearchString: "Workflow"
    };

    // apply jquery-ui themes
    $("button").button();

    // ensure form id is unique
    var saveFormID = "saveForm";
    var saveForm = document.getElementById(saveFormID);
    
    $("#" + saveFormID).wrap("<div id='SaveContainer' style='width:610px; height: 455px' />");

    var model = new SaveViewModel(null, baseViewModel, saveFormID, utils.decodeFullStops(getParameterByName("envir")));
    
    ko.applyBindings(model, saveForm);
    //model.showDialog(true, null);       
};

