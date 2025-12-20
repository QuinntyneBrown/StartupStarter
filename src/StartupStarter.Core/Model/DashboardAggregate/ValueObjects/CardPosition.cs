namespace StartupStarter.Core.Model.DashboardAggregate.ValueObjects;

public class CardPosition
{
    public int Row { get; private set; }
    public int Column { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }

    // EF Core constructor
    private CardPosition()
    {
    }

    public CardPosition(int row, int column, int width, int height)
    {
        Row = row;
        Column = column;
        Width = width;
        Height = height;
    }
}
