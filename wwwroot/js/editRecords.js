$(function () {
    $("form#choose-table").submit(function (event) {
        event.preventDefault();
        let form = $("form#edit-form");
        let tableName = $("select#table-name").val();
        if (tableName) {
            $.post("/Stats/ChooseTable", { tableName: tableName }, function (data) {
                let submitBtn = form.find($("div.form-group"));
                form.empty();
                let i = 0;
                $.each(data, function (index, object) {
                    let inputGroup = $("<div class=\"input-group mb-4\"></div>").appendTo(form);
                    $.each(object, function (index, value) {
                        console.dir(value);
                        let col = $("<div class=\"col-md-3\"></div>").appendTo(inputGroup);
                        col.append("<label for=\"" + index + "[" + i + "]\" class=\"control-label\">" + index + "</label>");
                        col.append("<input name=\""+index+"["+i+"]\" value=\""+value+"\" class=\"form-control\">");
                    });
                    i++;
                });

                form.append(submitBtn);
            }).fail(function () {
                form.append("<div>Nie udało się wyświetlić rekordów z wybranej tabeli</div>");
            });
        }
    })
});