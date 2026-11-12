return {
    id = "kidney_lab_minimal",
    template = "kidney_lab_minimal",
    output_name = "kidney_lab_minimal",
    completed_date = "2026-11-12",

    title = "Kidney Lab Log",
    record_fields = { "Name", "Next labs" },

    columns = {
        { label = "Date", class = "col-date" },
        { label = "BP", class = "col-bp" },
        { label = "eGFR", class = "col-egfr" },
        { label = "Creatinine", class = "col-creat" },
        { label = "Potassium", class = "col-pot" },
        { label = "Phosphorus", class = "col-phos" }
    },
    row_count = 14
}
