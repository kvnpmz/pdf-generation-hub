return {
    id = "kidney_lab_tracker",
    template = "kidney_lab_tracker",
    output_name = "kidney_lab_tracker",
    completed_date = "2026-11-12",

    header_line1 = "Kidney Lab",
    header_line2 = "& Symptom Tracker",
    subtitle = "Log each result and symptom for your next nephrology visit",
    plan_card_title = "My record",
    plan_card_fields = { "Name", "CKD stage", "Care team", "Next labs" },
    header_note = "Bring this page to every appointment",

    targets = {
        { label = "eGFR", value = "your last result" },
        { label = "Potassium", value = "3.5–5.0 mmol/L" },
        { label = "Phosphorus", value = "2.5–4.5 mg/dL" },
        { label = "Sodium", value = "135–145 mmol/L" },
        { label = "Creatinine", value = "0.6–1.3 mg/dL" },
        { label = "Blood pressure", value = "under 130/80" }
    },
    target_note = "Typical adult ranges — your care team sets your personal targets, so write theirs beside each one",

    log_title = "My lab log",
    log_aside = "One row per lab draw",
    columns = {
        { label = "Date", class = "col-date" },
        { label = "BP", class = "col-bp" },
        { label = "Weight", class = "col-weight" },
        { label = "eGFR", class = "col-lab" },
        { label = "Creatinine", class = "col-lab" },
        { label = "Potassium", class = "col-lab" },
        { label = "Phosphorus", class = "col-phos" },
        { label = "Notes", class = "col-notes" }
    },
    row_count = 12,
    log_note = "eGFR falls as kidney function drops — the direction over several draws matters more than any single result",

    between_title = "Between visits",
    between_aside = "What your numbers do not show",
    symptom_title = "Symptom check",
    symptom_aside = "Tick what you noticed",
    symptoms = {
        "Swelling in hands or feet",
        "Unusual tiredness",
        "Itching",
        "Muscle cramps",
        "Poor appetite",
        "Trouble sleeping",
        "Shortness of breath",
        "Feeling thirsty"
    },

    ask_title = "Ask my care team",
    ask_aside = "Ask at the visit",
    questions = {
        "Is my eGFR stable?",
        "Is my potassium in range?",
        "Any medicine changes?",
        "What should I eat?"
    },

}
