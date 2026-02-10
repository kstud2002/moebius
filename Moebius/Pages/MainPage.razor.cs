namespace Moebius.Pages;

public partial class MainPage
{
    private int startCol;

    private readonly Random rand = new();

    private List<int> columnNumbers = [1, 2, 3, 4];

    private int MaxNumberOfTries = 1000;
    private int ScoreToWin = 61; //61

    private string column1Value;
    public string Column1Value
    {
        get => column1Value;

        set
        {
            AddToList(value, 1);

            column1Value = string.Empty; // empty Your input field value
            StateHasChanged();
        }
    }

    private string column2Value;
    public string Column2Value
    {
        get => column2Value;

        set
        {
            AddToList(value, 2);

            column2Value = string.Empty; // empty Your input field value
            StateHasChanged();
        }
    }

    private string column3Value;
    public string Column3Value
    {
        get => column3Value;

        set
        {
            AddToList(value, 3);

            column3Value = string.Empty; // empty Your input field value
            StateHasChanged();
        }
    }

    private string column4Value;
    public string Column4Value
    {
        get => column4Value;

        set
        {
            AddToList(value, 4);

            column4Value = string.Empty; // empty Your input field value
            StateHasChanged();
        }
    }

    public List<string> Column1 { get; set; } = [];
    public List<string> Column2 { get; set; } = [];
    public List<string> Column3 { get; set; } = [];
    public List<string> Column4 { get; set; } = [];

    public List<string> InitialColumn1 { get; set; } = [];
    public List<string> InitialColumn2 { get; set; } = [];
    public List<string> InitialColumn3 { get; set; } = [];
    public List<string> InitialColumn4 { get; set; } = [];

    public int Score { get; set; }

    public int NumberOfTries { get; set; }

    public List<List<SelectedCard>> Steps { get; set; } = [];

    public List<decimal> RoundTotalTotals { get; set; } = [];
    public List<RoundScore> RoundScores { get; set; } = [];

    public bool IsReadyForSolve() =>
        InitialColumn1.Count != 13 ||
        InitialColumn2.Count != 13 ||
        InitialColumn3.Count != 13 ||
        InitialColumn4.Count != 13;

    public void ShuffleDeck()
    {
        ResetList(1);
        ResetList(2);
        ResetList(3);
        ResetList(4);

        var rand = new Random();

        var list = new List<string> { "J", "J", "J", "J", "Q", "Q", "Q", "Q", "K", "K", "K", "K", "A", "A", "A", "A" };
        list.AddRange(Enumerable.Range(2, 9).Select(x => x.ToString()));
        list.AddRange(Enumerable.Range(2, 9).Select(x => x.ToString()));
        list.AddRange(Enumerable.Range(2, 9).Select(x => x.ToString()));
        list.AddRange(Enumerable.Range(2, 9).Select(x => x.ToString()));

        while (InitialColumn1.Count < 13)
        {
            var index = rand.Next(0, list.Count);
            InitialColumn1.Add(list.ElementAt(index).ToString());
            list.RemoveAt(index);
        }

        while (InitialColumn2.Count < 13)
        {
            var index = rand.Next(0, list.Count);
            InitialColumn2.Add(list.ElementAt(index).ToString());
            list.RemoveAt(index);
        }

        while (InitialColumn3.Count < 13)
        {
            var index = rand.Next(0, list.Count);
            InitialColumn3.Add(list.ElementAt(index).ToString());
            list.RemoveAt(index);
        }

        while (InitialColumn4.Count < 13)
        {
            var index = rand.Next(0, list.Count);
            InitialColumn4.Add(list.ElementAt(index).ToString());
            list.RemoveAt(index);
        }
    }

    public void Win()
    {
        NumberOfTries = 0;
        Score = 0;

        while (Score < ScoreToWin && NumberOfTries < MaxNumberOfTries)
        {
            Column1 = new List<string>(InitialColumn1);
            Column2 = new List<string>(InitialColumn2);
            Column3 = new List<string>(InitialColumn3);
            Column4 = new List<string>(InitialColumn4);
            columnNumbers = [1, 2, 3, 4];
            Solve();
            Score = CalculateScore();

#if DEBUG
            Console.WriteLine($"Try Nr. {NumberOfTries}: Score => {Score}");
#endif

            NumberOfTries++;
        }
    }

    public void Solve()
    {
        Steps.Clear();
        RoundTotalTotals.Clear();
        RoundScores.Clear();

        while (Column1.Count != 0
            || Column2.Count != 0
            || Column3.Count != 0
            || Column4.Count != 0)
        {
            Steps.Add([]);
            List<decimal> RoundTotal = [];

            while (Math.Floor(RoundTotal.Sum()) < 32)
            {
                if (columnNumbers.Count == 0)
                {
                    RoundTotalTotals.Add(RoundTotal.Sum());
                    return;
                }
                int randCol = GetRandomColumn(Steps.LastOrDefault()?.LastOrDefault()?.Card);
                startCol = randCol;

                var selectedCard = GetCard(randCol, 31 - RoundTotal.Sum());

                if (selectedCard == null)
                {
                    break;
                }
                else
                {
                    switch (selectedCard.Column)
                    {
                        case 1:
                            Column1.RemoveAt(Column1.Count - 1);
                            if (Column1.Count == 0)
                            {
                                columnNumbers.Remove(1);
                            }
                            break;
                        case 2:
                            Column2.RemoveAt(Column2.Count - 1);
                            if (Column2.Count == 0)
                            {
                                columnNumbers.Remove(2);
                            }
                            break;
                        case 3:
                            Column3.RemoveAt(Column3.Count - 1);
                            if (Column3.Count == 0)
                            {
                                columnNumbers.Remove(3);
                            }
                            break;
                        case 4:
                            Column4.RemoveAt(Column4.Count - 1);
                            if (Column4.Count == 0)
                            {
                                columnNumbers.Remove(4);
                            }
                            break;
                        default:
                            break;
                    }
                }

                RoundTotal.Add(selectedCard.Card.CardValue);
                Steps.Last().Add(selectedCard);
            }
            RoundTotalTotals.Add(RoundTotal.Sum());
        }
    }

    private int GetRandomColumn(Card? lastCard)
    {
        if (lastCard == null)
        {
            if (Column1.LastOrDefault()?.ToUpper() == "J")
            {
                return 1;
            }
            else if (Column2.LastOrDefault()?.ToUpper() == "J")
            {
                return 2;
            }
            else if (Column3.LastOrDefault()?.ToUpper() == "J")
            {
                return 3;
            }
            else if (Column4.LastOrDefault()?.ToUpper() == "J")
            {
                return 4;
            }

            return columnNumbers.ElementAt(rand.Next(0, columnNumbers.Count));
        }

        if (Column1.LastOrDefault() == lastCard.DisplayText)
        {
            return 1;
        }
        else if (Column2.LastOrDefault() == lastCard.DisplayText)
        {
            return 2;
        }
        else if (Column3.LastOrDefault() == lastCard.DisplayText)
        {
            return 3;
        }
        else if (Column4.LastOrDefault() == lastCard.DisplayText)
        {
            return 4;
        }

        return columnNumbers.ElementAt(rand.Next(0, columnNumbers.Count));
    }

    private SelectedCard? GetCard(int randCol, decimal remaining)
    {
        string? result = randCol switch
        {
            1 => Column1.LastOrDefault(),
            2 => Column2.LastOrDefault(),
            3 => Column3.LastOrDefault(),
            4 => Column4.LastOrDefault(),
            _ => null,
        };
        if (result == null || new Card(result).CardValue > Math.Ceiling(remaining))
        {
            randCol = randCol == 4 ? 1 : randCol + 1;
            if (randCol == startCol)
            {
                return null;
            }
            else
            {
                return GetCard(randCol, remaining);
            }
        }

        return new SelectedCard(randCol, new Card(result));
    }

    private int CalculateScore()
    {
        foreach (var step in Steps)
        {
            Dictionary<string, int> pointSources = [];
            if (step.First().Card.DisplayText == "J")
            {
                pointSources.Add("Joker", 2);
            }

            pointSources.Add("15 Bonus", CalculateTotal15(step));

            if (step.Sum(x => x.Card.CardValue) == 31)
            {
                pointSources.Add("31 Bonus", 2);
            }

            pointSources.Add("Same Cards", CalculateSameCards(step));

            pointSources.Add("Run Bonus", SumOfNewConsecutiveBlockLengths(step.Select(s => s.Card.SortOrder).ToList()));

            RoundScores.Add(new RoundScore
            {
                PointSources = pointSources,
                Total = pointSources.Values.Sum(),
                RunningTotal = (RoundScores.LastOrDefault()?.RunningTotal ?? 0) + pointSources.Values.Sum(),
            });

        }
        return RoundScores.Select(rs => rs.Total).Sum();
    }

    private static int CalculateTotal15(List<SelectedCard> step)
    {
        int total = 0;
        foreach (var selectedCard in step)
        {
            total += selectedCard.Card.CardValue;
            if (total == 15) return 2;
            if (total > 15) return 0;
        }
        return 0;
    }

    private static int CalculateSameCards(List<SelectedCard> step)
    {
        var tempScore = 0;
        string oldCard = string.Empty;
        string newCard;
        int sameCardCount = 1;

        foreach (var selectedCard in step)
        {
            newCard = selectedCard.Card.DisplayText;
            if (newCard == oldCard)
            {
                sameCardCount++;
            }
            else
            {
                tempScore += GetSameCardScore(sameCardCount);
                sameCardCount = 1;
            }
            oldCard = selectedCard.Card.DisplayText;
        }

        tempScore += GetSameCardScore(sameCardCount);
        return tempScore;
    }

    private static int GetSameCardScore(int cardCount) => cardCount switch
    {
        2 => 2,
        3 => 2 + 6,
        4 => 2 + 6 + 12,
        _ => 0,
    };

    public static int SumOfNewConsecutiveBlockLengths(List<int> numbers)
    {
        int total = 0;
        var seenBlocks = new HashSet<string>(); // avoid duplicates

        for (int end = 0; end < numbers.Count; end++)
        {
            int min = numbers[end];
            int max = numbers[end];
            var seen = new HashSet<int> { numbers[end] };

            int bestLength = 0;
            int bestStart = -1;

            for (int start = end - 1; start >= 0; start--)
            {
                int value = numbers[start];

                // Duplicate breaks the possibility
                if (!seen.Add(value))
                    break;

                min = Math.Min(min, value);
                max = Math.Max(max, value);

                int length = end - start + 1;

                // Check if this window is a consecutive set
                if (max - min + 1 == length && length >= 3)
                {
                    string signature = $"{start}-{end}";

                    if (!seenBlocks.Contains(signature))
                    {
                        // Track the longest new block for this step
                        if (length > bestLength)
                        {
                            bestLength = length;
                            bestStart = start;
                        }
                    }
                }
            }

            // If we found a new block at this step, record it
            if (bestLength > 0)
            {
                string signature = $"{bestStart}-{end}";
                seenBlocks.Add(signature);
                total += bestLength;
            }
        }

        return total;
    }


    public void AddToList(string value, int column)
    {
        string pattern = "A2345678910JQK";
        if (!pattern.Contains(value, StringComparison.OrdinalIgnoreCase) || value.Length != 1) return;

        if (value.Contains('0') || value.Contains('1')) value = "10";
        switch (column)
        {
            case 1:
                if (InitialColumn1.Count < 13)
                {
                    InitialColumn1.Add(value);
                }
                break;
            case 2:
                if (InitialColumn2.Count < 13)
                {
                    InitialColumn2.Add(value);
                }
                break;
            case 3:
                if (InitialColumn3.Count < 13)
                {
                    InitialColumn3.Add(value);
                }
                break;
            case 4:
                if (InitialColumn4.Count < 13)
                {
                    InitialColumn4.Add(value);
                }
                break;
            default:
                break;
        }

    }

    public void ResetList(int column)
    {
        switch (column)
        {
            case 1:
                InitialColumn1.Clear();
                break;
            case 2:
                InitialColumn2.Clear();
                break;
            case 3:
                InitialColumn3.Clear();
                break;
            case 4:
                InitialColumn4.Clear();
                break;
        }
    }

    public class SelectedCard(int column, Card card)
    {
        public int Column { get; set; } = column;
        public Card Card { get; set; } = card;
    }

    public class Card(string displayText)
    {
        public string DisplayText { get; set; } = displayText.ToUpper();
        public int CardValue => DisplayText switch
        {
            "A" => 1,
            "J" or "Q" or "K" => 10,
            _ => Convert.ToInt32(DisplayText),
        };
        public int SortOrder => DisplayText switch
        {
            "A" => 1,
            "J" => 11,
            "Q" => 12,
            "K" => 13,
            _ => Convert.ToInt32(DisplayText),
        };
    }

    public class RoundScore
    {
        public required Dictionary<string, int> PointSources { get; set; }
        public required int Total { get; set; }
        public required int RunningTotal { get; set; }
    }
}
