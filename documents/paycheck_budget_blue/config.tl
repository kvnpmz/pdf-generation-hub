return {
    id = "paycheck_budget_blue",
    output_name = "paycheck_budget",
    template = "paycheck_budget",
    header = "Paycheck Budget",
    style = "blue_style",
    completed_date = "2026-06-21",
    columns = {
        { -- Column 1
            items = {
                { title = "Bills", id = 9, fields = { {key="due", label="Due"}, {key="amount", label="Amount"}, {key="paid", label="Paid"} } },
                { title = "Spending", id = 8, fields = { {key="due", label="Budget"}, {key="amount", label="Actual"} } }
            }
        },
        { -- Column 2 
            items = {
                { title = "Savings Goal", id = 5, fields = { {key="amount", label="Amount"} } },
                { title = "Extra Debts", id = 4, fields = { {key="amount", label="Amount"} } },
                { type = "summary" }
            }
        }
    }
} 
