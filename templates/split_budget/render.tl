local dom = require("dom")
local contracts = require("contracts")

local record Bucket
    name: string
    pct: integer
    tag: string
    note: string
    total_label: string
    categories: {string}
end

local record Rule
    label: string
    text: string
end

local record Config
    header_line1: string
    header_line2: string
    subtitle: string
    header_fields: {string}
    split_title: string
    split_aside: string
    columns: {string}
    buckets: {Bucket}
    check_title: string
    check_aside: string
    check_fields: {string}
    check_result: string
    rules_title: string
    rules_aside: string
    rules: {Rule}
end

local M = {}

local function render_header_fields(fields: {string}): string
    local items = {}
    for _, label in ipairs(fields) do
        table.insert(items, dom.div("header-item",
            dom.div("", label),
            dom.div("line", "")
        ))
    end
    return dom.div("header-fields", table.unpack(items))
end

local function render_split_bar(buckets: {Bucket}): string
    local segments = {}
    for index, bucket in ipairs(buckets) do
        table.insert(segments, dom.div(
            "seg seg-" .. tostring(bucket.pct) .. " seg-tone-" .. tostring(index),
            dom.tag("span", "seg-pct", tostring(bucket.pct) .. "%"),
            dom.tag("span", "seg-name", bucket.name)
        ))
    end
    return dom.div("split-bar", table.unpack(segments))
end

local function render_bucket_table(columns: {string}, categories: {string}, total_label: string): string
    local head_cells = { dom.th("col-cat", "Category") }
    for _, column in ipairs(columns) do
        table.insert(head_cells, dom.th("col-money", column))
    end

    local rows = {}
    for index, name in ipairs(categories) do
        local shade = (index % 2 == 0) and " shade" or ""
        local cells = { dom.td("col-cat", name) }
        for _ = 1, #columns do
            table.insert(cells, dom.td("col-money", ""))
        end
        table.insert(rows, dom.tr("item-row" .. shade, table.unpack(cells)))
    end

    local total_cells = { dom.td("col-cat", total_label) }
    for _ = 1, #columns do
        table.insert(total_cells, dom.td("col-money", ""))
    end
    table.insert(rows, dom.tr("bucket-total-row", table.unpack(total_cells)))

    return dom.table("bucket-table",
        dom.thead("", dom.tr("", table.unpack(head_cells))),
        dom.tbody("", table.unpack(rows))
    )
end

local function render_bucket(bucket: Bucket, columns: {string}): string
    local head = dom.div("bucket-head",
        dom.h3("", bucket.name),
        dom.tag("span", "bucket-badge", tostring(bucket.pct) .. "%")
    )

    return dom.section("bucket",
        head,
        dom.div("bucket-meta",
            dom.tag("span", "bucket-tag", bucket.tag),
            dom.tag("span", "bucket-note", bucket.note)
        ),
        render_bucket_table(columns, bucket.categories, bucket.total_label),
        dom.div("bucket-check",
            dom.div("check-line", dom.tag("span", "check-label", "Share of income"), dom.div("line", "")),
            dom.div("check-line", dom.tag("span", "check-label", "Left to assign"), dom.div("line", ""))
        )
    )
end

local function render_buckets(config: Config): string
    local items = {}
    for _, bucket in ipairs(config.buckets) do
        table.insert(items, render_bucket(bucket, config.columns))
    end
    return dom.div("buckets", table.unpack(items))
end

local function render_check(config: Config): string
    local items = {}
    for _, label in ipairs(config.check_fields) do
        table.insert(items, dom.div("check-item",
            dom.tag("span", "check-label", label),
            dom.div("line", "")
        ))
    end

    return dom.section("zero-check",
        dom.div("section-head",
            dom.h2("", config.check_title),
            dom.tag("span", "section-aside", config.check_aside)
        ),
        dom.div("check-grid", table.unpack(items)),
        dom.div("check-result",
            dom.tag("span", "result-label", config.check_result),
            dom.div("line", "")
        )
    )
end

local function render_rules(config: Config): string
    local rows = {}
    for _, rule in ipairs(config.rules) do
        table.insert(rows, dom.div("rule-row",
            dom.tag("span", "rule-label", rule.label),
            dom.tag("span", "rule-text", rule.text)
        ))
    end

    return dom.section("rules",
        dom.div("section-head",
            dom.h2("", config.rules_title),
            dom.tag("span", "section-aside", config.rules_aside)
        ),
        dom.div("rule-rows", table.unpack(rows))
    )
end

function M.render(config: Config): string
    local header = dom.header("masthead",
        dom.h1("", config.header_line1),
        dom.div("h1-line-two", config.header_line2),
        dom.tag("p", "subtitle", config.subtitle),
        render_header_fields(config.header_fields)
    )

    local main = dom.main("",
        dom.div("section-head",
            dom.h2("", config.split_title),
            dom.tag("span", "section-aside", config.split_aside)
        ),
        render_split_bar(config.buckets),
        render_buckets(config),
        render_check(config),
        render_rules(config)
    )

    return header .. main
end

return M as contracts.TemplateRender
