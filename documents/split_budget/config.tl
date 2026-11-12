return {
    id = "split_budget",
    template = "split_budget",
    output_name = "split_budget",
    completed_date = "2026-11-12",

    header_line1 = "50/30/20",
    header_line2 = "Budget Worksheet",
    subtitle = "Split take-home pay into needs, wants and savings, then assign every dollar before the month starts",
    header_fields = { "Month", "Take-home pay", "Other income", "Total to allocate" },

    split_title = "Where the money goes",
    split_aside = "Share of take-home pay",

    columns = { "Plan", "Spent" },

    buckets = {
        {
            name = "Needs",
            pct = 50,
            tag = "Must pay",
            note = "Rent, food, cover and the minimums",
            total_label = "Total needs",
            categories = {
                "Rent/mortgage",
                "Utilities",
                "Groceries",
                "Health cover",
                "Transportation",
                "Debt minimums",
                "Childcare",
                "Phone/internet",
                "Medical costs",
                "Home supplies"
            }
        },
        {
            name = "Wants",
            pct = 30,
            tag = "Nice to have",
            note = "Chosen extras, not obligations",
            total_label = "Total wants",
            categories = {
                "Dining out",
                "Subscriptions",
                "Travel",
                "Hobbies",
                "Clothing",
                "Personal care",
                "Gifts/giving",
                "Home decor",
                "Entertainment",
                "Fitness"
            }
        },
        {
            name = "Savings",
            pct = 20,
            tag = "Pay yourself first",
            note = "Transfers on payday, not month-end",
            total_label = "Total savings",
            categories = {
                "Emergency fund",
                "Retirement",
                "Investments",
                "Sinking funds",
                "Extra debt",
                "Home/car fund",
                "Holiday fund",
                "Education fund",
                "Medical fund",
                "Buffer"
            }
        }
    },

    check_title = "Every dollar assigned",
    check_aside = "Copy the three totals here",
    check_fields = { "Total income", "Needs", "Wants", "Savings" },
    check_result = "Needs + wants + savings must equal income · left over $0",

    rules_title = "When the split does not fit",
    rules_aside = "Fix the percentage before the month starts",
    rules = {
        { label = "Needs over 50%", text = "Budget the real rent first, trim groceries and transport, and only then move the excess into needs — a rent above half of take-home pay means the split needs a higher income, not a smaller food bill." },
        { label = "Wants over 30%", text = "Cap the two largest lines (usually dining and subscriptions) at the amount already in the box and send the difference to savings instead of spending it." },
        { label = "Savings under 20%", text = "Automate the transfer for payday so the money never reaches checking, and count extra debt payments as savings rather than as wants." }
    },

    footer_note = "50/30/20 is a starting point, not a rule · percentages apply to take-home pay · adjust for rent, dependants and debt"
}
