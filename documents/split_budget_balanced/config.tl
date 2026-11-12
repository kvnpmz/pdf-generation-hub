return {
    id = "split_budget_balanced",
    template = "split_budget_balanced",
    output_name = "split_budget_balanced",
    completed_date = "2026-11-12",

    header_line = "50/30/20",
    subject = "Budget Worksheet",
    inputs = { "Month", "Take-home pay" },
    bar_aside = "Share of take-home pay",

    column_label = "Plan",

    buckets = {
        {
            name = "Needs",
            pct = 50,
            tag = "Must pay",
            categories = {
                "Rent/mortgage",
                "Utilities",
                "Groceries",
                "Transport",
                "Insurance",
                "Debt minimums",
                "Childcare",
                "Phone",
                "Medical"
            }
        },
        {
            name = "Wants",
            pct = 30,
            tag = "Nice to have",
            categories = {
                "Dining out",
                "Subscriptions",
                "Travel",
                "Hobbies",
                "Clothing",
                "Gifts",
                "Entertainment",
                "Fitness",
                "Personal care"
            }
        },
        {
            name = "Savings",
            pct = 20,
            tag = "Pay yourself first",
            categories = {
                "Emergency fund",
                "Retirement",
                "Investments",
                "Sinking funds",
                "Extra debt",
                "Home or car",
                "Holiday fund",
                "Education fund",
                "Buffer"
            }
        }
    },

    check_fields = { "Income", "Needs", "Wants", "Savings" },
    check_result = "= $0",

    note = "Keep needs at half, wants at a third and savings at a fifth of take-home pay"
}
