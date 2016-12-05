
$(document).ready(function () {
        $(document).on('click', '.tag-btn', function () {
            processSelection(Number($(this).data('tag')),"ArticleTags",$(this), "SelectedTags", "UnselectedTags");
        });
        $(document).on('click', '.file-tag-btn', function () {
            processSelection(Number($(this).data('tag')), "FileTags", $(this), "SelectedFileTags", "UnselectedFileTags");
        });
        $(document).on('click', '.file-btn', function () {
            processSelection(Number($(this).data('tag')), "ArticleMediaKitFiles", $(this), "SelectedFiles", "UnselectedFiles");
        });

        $("#openNewOwner").click(function () {
            $("#newOwner").show();
            $("#openNewOwner").hide();
            $("#OwnerId").hide();
        });

        $("#btnAddTag").click(function () {
            addTag("newTag", "SelectedTags", "UnselectedTags", "ArticleTags","");
        });
        $("#btnAddFileTag").click(function () {
            addTag("newFileTag", "SelectedFileTags", "UnselectedFileTags", "FileTags","file-");
        });

        function addTag(newTagElement, selectedTagsElement, unselectedTagsElement, allTags, tagPrefix) {
            var tempTagName = $("#" + newTagElement).val();
            if (tempTagName.length > 0) {
                $.ajax({
                    type: "GET",
                    url: "/Tags/CreateAjax?tagName=" + $("#" + newTagElement).val(),
                    contentType: false,
                    processData: false,
                    success: function (response) {

                        var responseText = JSON.stringify(response);
                        var responseData = JSON.parse(responseText);
                        if (!responseData["status"]) {
                            $("<div class=\"btn btn-sm btn-default tag-btn\" data-tag=\"" + responseData["id"] + "\">" + responseData["tag"] + "</div>").appendTo("#UnselectedTags");
                            $("<div class=\"btn btn-sm btn-default file-tag-btn\" data-tag=\"" + responseData["id"] + "\">" + responseData["tag"] + "</div>").appendTo("#UnselectedFileTags");
                            $("#" + unselectedTagsElement).children().last().remove();
                            $("<div class=\"btn btn-sm btn-default " + tagPrefix + "tag-btn\" data-tag=\"" + responseData["id"] + "\">" + responseData["tag"] + "</div>").appendTo("#" + selectedTagsElement);
                            $("#" + newTagElement).val("");
                            $("#" + allTags).val($("#" + allTags).val() + responseData["id"] + ",");
                        } else {
                            alert(responseData["message"]);
                        }
                    },
                    error: function (err) { alert("There was an error creating the tag."); }
                });
            } else {
                alert("Enter a tag before clicking the 'Add' button.");
            }
        }

        $("#upload").click(function (evt) {
            var fileUpload = $("#files").get(0);
            var files = fileUpload.files;
            var data = new FormData();
            for (var i = 0; i < files.length ; i++) {
                data.append(files[i].name, files[i]);
            }
            var description = $("#Description").val();
            var mediaType = $("#MediaType").val();
            var url = $("#url").val();
            var ownerId = $("#OwnerId").val();
            var fileTags = $("#FileTags").val();
            var copyrightDate = $("#copyrightDate").val();
            var altText = $("#altText").val();
            var ownerName = $("#ownerName").val();
            if (ownerName) {
                var address = $("#address").val();
                var email = $("#email").val();
                var phone = $("#phone").val();
                var socialMedia = $("#socialMedia").val();
                var website = $("#website").val();
            } else {
                ownerName = "";
            }





            data.append("description",description);
            data.append("mediaType", mediaType);
            data.append("url", url);
            data.append("ownerId", ownerId);
            data.append("FileTags", fileTags);
            data.append("copyrightDate", copyrightDate);
            data.append("altText", altText);
            if (ownerName) {
                data.append("ownerName", ownerName);
                data.append("address", address);
                data.append("email", email);
                data.append("phone", phone);
                data.append("socialMedia", socialMedia);
                data.append("website", website);
            }

            $.ajax({
                type: "POST",
                url: "/MediaKitFiles/UploadFilesAjax",
                contentType: false,
                processData: false,
                dataType:"json",
                data: data,
                success: function (message) {
                    var temp = JSON.stringify(message);
                    
                    var responseData = JSON.parse(temp);
                    $("#fileModal").modal('hide');
                    if ($("#SelectedFiles").length>0) {
                        if (responseData["mediaType"] == "image") {
                            $("<img class=\"file-btn\" data-tag=\"" + responseData["mediaKitFileId"] + "\" src=\"/mediakitfiles/" + responseData["url"] + "\" />").appendTo("#SelectedFiles");
                        } else {
                            $("<div class=\"btn btn-default file-btn\" data-tag=\"" + responseData["mediaKitFileId"] + "\">" + responseData["url"] + "</div>").appendTo("#SelectedFiles");
                        }
                        $("#OwnerId").val("");
                    } else {
                        var tags = "";
                        var tempDate = new Date(responseData["copyrightDate"]+"Z");
                        for (var x = 0; x < responseData["tagNames"].length; x++) {
                            tags += "<div class=\"btn btn-sm btn-default\">" + responseData["tagNames"][x]["name"] + "</div>"
                        }
                        $("<div class=\"mediakitFileWrapper\">" +  responseData["iconURL"] + "<p><a href=\"~/MediaKitFiles/Edit/" + responseData["mediaKitFileId"] + "\">Edit File</a> | <a href=\"~/MediaKitFiles/Delete/" + responseData["mediaKitFileId"] + "\">Delete File</a><br/>" + responseData["description"] + "<br/>&copy; " + tempDate.getFullYear() + "<br/><strong>tags</strong><br/>" + tags + "</p></div>").appendTo("#mediaKitFiles");
                    }
                    $("#ArticleMediaKitFiles").val($("#ArticleMediaKitFiles").val() + responseData["mediaKitFileId"] + ",");

                    $("#files").val(",");
                    $("#Description").val("");
                    $("#MediaType").val("");
                    $("#url").val("");
                    
                    $("#FileTags").val(",");

                    $("#ownerName").val("");
                    $("#address").val("");
                    $("#email").val("");
                    $("#phone").val("");
                    $("#socialMedia").val("");
                    $("#website").val("");
                    $("#altText").val("");
                    $("#copyrightDate").val("");
                    $("#newOwner").hide();
                    $("#openNewOwner").show();
                    $("#OwnerId").show();
                    resetSelections("#SelectedFileTags .btn", "UnselectedFileTags");
                },
                error: function () {
                    alert("There was error uploading files!");
                }
            });
        });

        $("#addOwner").click(function (evt) {

            var data = new FormData();
            var ownerName = $("#ownerName").val();
            var address = $("#address").val();
            var email = $("#email").val();
            var phone = $("#phone").val();
            var social = $("#socialMedia").val();
            var website = $("#website").val();

            data.append("name", ownerName);
            data.append("address", address);
            data.append("email", email);
            data.append("phone", phone);
            data.append("socialMedia", social);
            data.append("website", website);

            $.ajax({
                type: "POST",
                url: "/Owners/CreateAjax",
                contentType: false,
                processData: false,
                data: data,
                success: function (message) {
                    alert(message);
                },
                error: function () {
                    alert("There was error uploading files!");
                }
            });
        });
    });

function processSelection(itemId, allValuesElement, clickedElement, selectedElements, unselectedElements) {
    var allValues = $("#" + allValuesElement).val();
    if (checkSelected(itemId, allValues)) {
        allValues = allValues + itemId + ",";
        $("#" + allValuesElement).val(allValues);
        clickedElement.remove().clone().appendTo('#' + selectedElements);
    } else {
        allValues = allValues.replace(itemId + ",", "");

        if (allValues.substr(0, 1) != ",") {
            allValues = "," + allValues;
        }
        $("#" + allValuesElement).val(allValues);
        clickedElement.remove().clone().prependTo('#' + unselectedElements);
    }
}
function checkSelected(itemId, allItems) {
    if (allItems.indexOf("," + itemId + ",") >= 0) {
        return false;
    } else {
        return true;
    }
}

function resetSelections(childElements,unselectedElements) {
    $(childElements).remove().clone().prependTo('#' + unselectedElements);
}

function checkFile() {
    var el = $("#files");
    var file = (el[0].files ? el[0].files[0] : el[0].value || undefined);
    var supportedFormats = ['image/jpg', 'image/gif', 'image/png', 'application/msword', 'application/pdf', 'application/powerpoint', 'application/excel'];

    if (file && file.type) {
        if (0 > supportedFormats.indexOf(file.type)) {
            alert('Unsupported file format. JPG, GIF, PNG, PDF, DOC, DOCX, XLS, XLSX, PPT, and PPTX are the only file formats accepted.');
            $("#files").val("");
        }

        if (file.size > 31457280) {
            alert('File size too large. File must be smaller than 30MB.');
            $("#files").val("");
        }
    }
}

function formatFileName(inputElement) {
    var tempString = $("#" + inputElement).val();
    var outString = tempString.replace(/[`~!@#$%^&*()_|+\-=?;:'",.<>\{\}\[\]\\\/]/gi, '');
    outString = outString.split(' ').join('_');
    $("#" + inputElement).val(outString);
}