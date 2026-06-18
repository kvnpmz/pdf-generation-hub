local record M
    record Config
        id: string
        output_name: string
        template: string
        header: string
        style: string | nil
        completed_date: string | nil
    end

    record RenderStrategy
        render: function(any): string
    end
end

return M
