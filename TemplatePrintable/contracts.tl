local record Config
    id: string
    output_name: string
    template: string
    header: string
    style: string
    overrides: string
    completed_date: string
end

local record TemplateRender
    render: function(any): string
end

local record RenderContext
    config: Config
    render: function(any): string
end

local record M
    type Config = Config
    type TemplateRender = TemplateRender
    type RenderContext = RenderContext
end

return M
