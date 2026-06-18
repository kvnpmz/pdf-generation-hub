local record M
    record Config
        id: string
        output_name: string
        template: string
        header: string
        style: string
        completed_date: string
    end

    record RenderStrategy
        render: function(any): string
    end
end

return M
