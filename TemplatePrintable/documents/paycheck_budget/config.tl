return {
    id = "paycheck_budget",
    output_name = "paycheck_budget",
    template = "paycheck_budget",
    header = "Paycheck Budget",
    columns = {
        { -- Column 1
            items = {
                { title = "Bills", id = 9, fields = { {key="due", label="Due"}, {key="amount", label="Amount"}, {key="paid", label="Paid"} } },
                { title = "Spending", id = 8, fields = { {key="due", label="Due"}, {key="amount", label="Amount"} } }
            }
        },
        { -- Column 2 
            items = {
                { title = "Savings Goal", id = 5, fields = { {key="amount", label="Amount"} } },
                { title = "Extra Debt", id = 4, fields = { {key="amount", label="Amount"} } },
                { type = "summary" }
            }
        }
    }
} 
