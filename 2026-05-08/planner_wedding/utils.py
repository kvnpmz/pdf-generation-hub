from bs4 import BeautifulSoup

# Constants for A4 (can be adjusted for Letter in the function)
MARGIN = 72
H1_HEIGHT = 36
H1_MARGIN = 10
H2_HEIGHT = 27
ITEM_HEIGHT = 16
HEADER_TABLE = 40
SECTION_BORDER = 1.3
SECTION_PADDING = 5

def calculate_dynamic_margins(html_content, page_height, page_width, scale):
    """
    Parses HTML to calculate the necessary bottom-margin to justify 
    content vertically within columns.
    """
    container_height = page_height - MARGIN - H1_HEIGHT - H1_MARGIN - HEADER_TABLE - 1
    soup = BeautifulSoup(html_content, 'html.parser')
    columns = soup.find_all(class_='column')
    
    margins = []
    
    for col in columns:
        sections = col.find_all(class_='section')
        num_sections = len(sections)
        
        if num_sections <= 1:
            margins.append(0)
            continue
            
        h2_count = len(col.find_all('h2'))
        item_count = len(col.find_all(class_='item'))
        
        used_space = (h2_count * H2_HEIGHT * scale) + (item_count * ITEM_HEIGHT * scale)
        leftover_space = container_height - used_space
        
        margin_per = leftover_space / (num_sections - 1)
        margins.append(round(max(0, margin_per), 4))

    return {
        "h1_height": H1_HEIGHT,
        "h1_margin": H1_MARGIN,
        "h2_height": H2_HEIGHT * scale,
        "item_height": ITEM_HEIGHT * scale,
        "header_table": HEADER_TABLE,
        "container_height": container_height,
        "section_border": SECTION_BORDER,
        "section_padding": SECTION_PADDING,
        "page_height": page_height,
        "page_width": page_width,
        "margins": margins
    }
