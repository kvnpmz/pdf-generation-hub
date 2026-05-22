

def get_column_gap(item_count, category_count, total_pt=720):
    # Header ~16pt, Item ~11pt (adjust based on your specific CSS)
    content_h = (category_count * 16) + (item_count * 11)
    
    # Calculate gap to fill the remaining 720pt of the page
    if category_count <= 1: return 0
    gap = (total_pt - content_h) / (category_count - 1)
    
    return max(5, gap) # Ensure a minimum 5pt gap


