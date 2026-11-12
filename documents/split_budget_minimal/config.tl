return {
    id = "split_budget_minimal",
    template = "split_budget_minimal",
    output_name = "split_budget_minimal",
    completed_date = "2026-11-12",

    header_line = "50/30/20",
    subject = "Budget",
    inputs = { "Month", "Income" },

    buckets = {
        {
            name = "Needs",
            pct = 50,
            categories = {
                "Rent",
                "Utilities",
                "Groceries",
                "Transport",
                "Insurance",
                "Debt minimums",
                "Childcare",
                "Phone"
            }
        },
        {
            name = "Wants",
            pct = 30,
            categories = {
                "Dining out",
                "Subscriptions",
                "Travel",
                "Hobbies",
                "Clothing",
                "Gifts",
                "Entertainment",
                "Fitness"
            }
        },
        {
            name = "Savings",
            pct = 20,
            categories = {
                "Emergency fund",
                "Retirement",
                "Investments",
                "Sinking funds",
                "Extra debt",
                "Home or car",
                "Holiday fund",
                "Buffer"
            }
        }
    },

    check_fields = { "Income", "Assigned" },
    check_result = "= $0"
}
