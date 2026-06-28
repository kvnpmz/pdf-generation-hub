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

    record LayoutProcessor
        render: function(any): string
    end
end

return M
