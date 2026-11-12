return {
    id = "kidney_lab_essential",
    template = "kidney_lab_essential",
    output_name = "kidney_lab_essential",
    completed_date = "2026-11-12",

    title = "Kidney Lab Log",
    subtitle = "Labs and symptoms between nephrology visits",
    record_fields = { "Name", "Next labs" },
    header_note = "Bring this page to every appointment",

    targets = {
        { label = "eGFR", value = "your last result" },
        { label = "Potassium", value = "3.5–5.0 mmol/L" },
        { label = "Phosphorus", value = "2.5–4.5 mg/dL" },
        { label = "Sodium", value = "135–145 mmol/L" },
        { label = "Creatinine", value = "0.6–1.3 mg/dL" },
        { label = "Blood pressure", value = "under 130/80" }
    },

    log_title = "My lab log",
    log_aside = "One row per lab draw",
    columns = {
        { label = "Date", class = "col-date" },
        { label = "BP", class = "col-bp" },
        { label = "eGFR", class = "col-egfr" },
        { label = "Creatinine", class = "col-creat" },
        { label = "Potassium", class = "col-pot" },
        { label = "Phosphorus", class = "col-phos" },
        { label = "Notes", class = "col-notes" }
    },
    row_count = 13,
    log_note = "The direction over several draws matters more than any single eGFR result",

    symptom_title = "Symptom check",
    symptom_aside = "Tick what you noticed since the last draw",
    symptoms = {
        "Swelling in hands or feet",
        "Unusual tiredness",
        "Itching",
        "Muscle cramps",
        "Poor appetite",
        "Trouble sleeping",
        "Shortness of breath",
        "Feeling thirsty"
    }
}
