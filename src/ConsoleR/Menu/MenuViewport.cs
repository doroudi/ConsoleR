namespace ConsoleR.Menu.Models;

/// <summary>
/// Keeps track of which slice of the options is currently visible inside the scroll box.
/// The viewport never moves further than needed: it only scrolls when the selected option
/// would leave the visible box.
/// </summary>
internal sealed class MenuViewport
{
    private int _pageSize;

    public MenuViewport(int itemCount)
    {
        ItemCount = itemCount < 0 ? 0 : itemCount;
        _pageSize = ItemCount;
    }

    /// <summary>Total number of options.</summary>
    public int ItemCount { get; }

    /// <summary>Number of options that fit into the box.</summary>
    public int PageSize => _pageSize;

    /// <summary>Index of the first visible option.</summary>
    public int Offset { get; private set; }

    /// <summary>There are more options than the box can show.</summary>
    public bool CanScroll => ItemCount > _pageSize;

    /// <summary>Options that are hidden above the visible box.</summary>
    public int ItemsAbove => Offset;

    /// <summary>Options that are hidden below the visible box.</summary>
    public int ItemsBelow => Math.Max(0, ItemCount - Offset - _pageSize);

    public bool HasItemsAbove => ItemsAbove > 0;

    public bool HasItemsBelow => ItemsBelow > 0;

    /// <summary>Number of rows of the box that are filled with options.</summary>
    public int VisibleCount => Math.Min(_pageSize, Math.Max(0, ItemCount - Offset));

    /// <summary>Resizes the box, for example when the console window changes size.</summary>
    public void Resize(int pageSize)
    {
        if (pageSize < 0) pageSize = 0;
        if (pageSize > ItemCount) pageSize = ItemCount;
        _pageSize = pageSize;
        ClampOffset();
    }

    /// <summary>
    /// Scrolls the viewport by the smallest amount that makes the given option index visible.
    /// </summary>
    public void ScrollTo(int index)
    {
        if (ItemCount == 0 || index < 0) return;

        if (index < Offset)
            Offset = index;
        else if (index >= Offset + _pageSize)
            Offset = index - _pageSize + 1;

        ClampOffset();
    }

    private void ClampOffset()
    {
        var maxOffset = Math.Max(0, ItemCount - _pageSize);
        if (Offset > maxOffset) Offset = maxOffset;
        if (Offset < 0) Offset = 0;
    }
}
