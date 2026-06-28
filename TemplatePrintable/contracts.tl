local record M 
    record Config
        id: string
        output_name: string
        template: string
        header: string
        style: string
        overrides: string
        completed_date: string
    end

    record TemplateRender
        render: function(any): string
    end

    record RenderContext
        config: Config
        render: function(any): string
    end
end

return M
