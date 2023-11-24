using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using static System.Formats.Asn1.AsnWriter;

namespace Möbius.Pages
{
    public partial class MainPage
    {
        private int startCol;

        private Random rand = new Random();

        private List<int> columnNumbers = new() { 1, 2, 3, 4 };

        private int MaxNumberOfTries = 1000000;
        private int ScoreToWin = 61; //61

        private string column1Value;
        public string Column1Value
        {
            get => column1Value;

            set
            {
                AddToList(value, 1);

                // do stuff if input value is not valid

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

                // do stuff if input value is not valid

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

                // do stuff if input value is not valid

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

                // do stuff if input value is not valid

                column4Value = string.Empty; // empty Your input field value
                StateHasChanged();
            }
        }

        public List<string> Column1 { get; set; } = new List<string>();
        public List<string> Column2 { get; set; } = new List<string>();
        public List<string> Column3 { get; set; } = new List<string>();
        public List<string> Column4 { get; set; } = new List<string>();

        public List<string> InitialColumn1 { get; set; } = new List<string>();
        public List<string> InitialColumn2 { get; set; } = new List<string>();
        public List<string> InitialColumn3 { get; set; } = new List<string>();
        public List<string> InitialColumn4 { get; set; } = new List<string>();

        public int Score { get; set; }

        public int NumberOfTries { get; set; }

        public List<List<(int column, decimal card)>> Steps { get; set; } = new();

        public List<decimal> RoundTotalTotals { get; set; } = new();
        public List<(List<(string bonus, int points)> values, int total, int runningTotal)> RoundScores { get; set; } = new();

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

            while (Score < ScoreToWin && NumberOfTries < MaxNumberOfTries)
            {
                Column1 = new List<string>(InitialColumn1);
                Column2 = new List<string>(InitialColumn2);
                Column3 = new List<string>(InitialColumn3);
                Column4 = new List<string>(InitialColumn4);
                columnNumbers = new() { 1, 2, 3, 4 };
                Solve();
                Score = CalculateScore();
                NumberOfTries++;
            }
        }

        public void Solve()
        {
            Steps.Clear();
            RoundTotalTotals.Clear();
            RoundScores.Clear();

            while (Column1.Any()
                || Column2.Any()
                || Column3.Any()
                || Column4.Any())
            {
                Steps.Add(new());
                List<decimal> RoundTotal = new();

                while (Math.Floor(RoundTotal.Sum()) < 32)
                {
                    if (!columnNumbers.Any())
                    {
                        RoundTotalTotals.Add(RoundTotal.Sum());
                        return;
                    }
                    int randCol = GetRandomColumn(Steps.LastOrDefault()?.LastOrDefault().card);
                    startCol = randCol;

                    var number = GetNumber(randCol, 31 - RoundTotal.Sum());

                    if (number.number == null)
                    {
                        break;
                    }
                    else
                    {
                        switch (number.col)
                        {
                            case 1:
                                Column1.RemoveAt(Column1.Count - 1);
                                if (!Column1.Any())
                                {
                                    columnNumbers.Remove(1);
                                }
                                break;
                            case 2:
                                Column2.RemoveAt(Column2.Count - 1);
                                if (!Column2.Any())
                                {
                                    columnNumbers.Remove(2);
                                }
                                break;
                            case 3:
                                Column3.RemoveAt(Column3.Count - 1);
                                if (!Column3.Any())
                                {
                                    columnNumbers.Remove(3);
                                }
                                break;
                            case 4:
                                Column4.RemoveAt(Column4.Count - 1);
                                if (!Column4.Any())
                                {
                                    columnNumbers.Remove(4);
                                }
                                break;
                            default:
                                break;
                        }
                    }

                    RoundTotal.Add(number.number.Value);
                    Steps.Last().Add((number.col.Value, number.number.Value));
                }
                RoundTotalTotals.Add(RoundTotal.Sum());
            }
        }

        private int GetRandomColumn(decimal? lastCard)
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

            var card = ConvertToString(lastCard.Value);

            if (Column1.LastOrDefault() == card)
            {
                return 1;
            }
            else if (Column2.LastOrDefault() == card)
            {
                return 2;
            }
            else if (Column3.LastOrDefault() == card)
            {
                return 3;
            }
            else if (Column4.LastOrDefault() == card)
            {
                return 4;
            }

            return columnNumbers.ElementAt(rand.Next(0, columnNumbers.Count));
        }

        private (decimal? number, int? col) GetNumber(int randCol, decimal remaining)
        {
            (decimal? number, int? col) result = randCol switch
            {
                1 => (ConvertToDecimal(Column1.LastOrDefault(), randCol), randCol),
                2 => (ConvertToDecimal(Column2.LastOrDefault(), randCol), randCol),
                3 => (ConvertToDecimal(Column3.LastOrDefault(), randCol), randCol),
                4 => (ConvertToDecimal(Column4.LastOrDefault(), randCol), randCol),
                _ => (null, null),
            };
            if (result.number == null || result.number > Math.Ceiling(remaining))
            {
                randCol = randCol == 4 ? 1 : randCol + 1;
                if (randCol == startCol)
                {
                    return (null, null);
                }
                else
                {
                    result = GetNumber(randCol, remaining);
                }
                return result;
            }

            return result;
        }

        private int CalculateScore()
        {
            foreach (var step in Steps)
            {
                List<(string bonus, int points)> score = new();
                if (ConvertToString(step.First().card) == "J")
                {
                    score.Add(("Joker", 2));
                }

                score.Add(("15 Bonus", CalculateTotal15(step)));

                if (Math.Floor(step.Sum(x => x.card)) == 31)
                {
                    score.Add(("31 Bonus", 2));
                }

                score.Add(("Same Cards", CalculateSameCards(step)));

                score.Add(("Run Bonus", CalculateCardsInARow(step)));
                RoundScores.Add((score, score.Select(x => x.points).Sum(), RoundScores.Select(x => x.total).Sum() + score.Select(x => x.points).Sum()));
            }
            return RoundScores.Select(x => x.total).Sum();
        }

        private static int CalculateTotal15(List<(int column, decimal card)> step)
        {
            int total = 0;
            foreach (var card in step)
            {
                total += (int)Math.Floor(card.card);
                if (total == 15) return 2;
                if (total > 15) return 0;
            }
            return 0;
        }

        private static int CalculateSameCards(List<(int column, decimal card)> step)
        {
            var tempScore = 0;
            decimal oldCard = -1;
            decimal newCard;
            int sameCardCount = 1;

            foreach (var card in step)
            {
                newCard = card.card;
                if (newCard == oldCard)
                {
                    sameCardCount++;
                }
                else
                {
                    tempScore += GetSameCardScore(sameCardCount);
                    sameCardCount = 1;
                }
                oldCard = card.card;
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

        private static int CalculateCardsInARow(List<(int column, decimal card)> step)
        {
            if (step.Count < 3) return 0;

            var cardRuns = new List<List<int>>();

            for (int i = 3; i < step.Count; i++)
            {
                foreach (var run in CheckIfCardsAreInARow(step.Take(i).Select(x => x.card).ToList()))
                {
                    cardRuns.Add(run);
                }
            }

            if (cardRuns.Select(x => x.Count).Sum() > 0)
            {

            }

            return cardRuns.Distinct().Select(x => x.Count).Sum();

            //List<List<(int column, decimal card)>> sublists = new();
            //List<int> lengths = new();
            //var scoringLists = new List<List<(int column, decimal card)>>();
            //lengths.AddRange(Enumerable.Range(3, step.Count > 7 ? 5 : step.Count - 2));

            //foreach (var length in lengths)
            //{
            //    for (int i = 0; i < step.Count - length + 1; i++)
            //    {
            //        sublists.Add(step.Skip(i).Take(length).ToList());
            //    }
            //}

            //foreach (var list in sublists)
            //{
            //    foreach (var card in list.OrderBy(x => x.card))
            //    {
            //        newCard = card.card;
            //        if (newCard - oldCard == 1 || newCard - oldCard == 0.1M)
            //        {
            //            cardsInARowCount++;
            //        }
            //        else
            //        {
            //            cardsInARowCount = 1;
            //        }
            //        oldCard = card.card;
            //    }
            //    if (cardsInARowCount == list.Count)
            //    {
            //        scoringLists.Add(list);
            //    }
            //}

            //return scoringLists.Select(x => x.Count).Distinct().Sum();
        }

        private static List<List<int>> CheckIfCardsAreInARow(List<decimal> step)
        {
            var cardRuns = new List<List<int>>();
            for (int i = 0; i < step.Count - 3; i++)
            {
                var tempRun = GetStepNumbers(step.Skip(i).OrderBy(x => x));
                if (tempRun.Count() != tempRun.Distinct().Count()) return new();
                var lowest = tempRun.First();
                var highest = tempRun.Last();
                var numberOfTerms = highest - lowest + 1;

                if (numberOfTerms * ((highest + lowest) / 2) == tempRun.Sum())
                {
                    cardRuns.Add(tempRun.ToList());
                }
            }

            return cardRuns;
        }

        private static IEnumerable<int> GetStepNumbers(IEnumerable<decimal> step)
        {
            foreach (var card in step)
            {
                yield return card switch
                {
                    10.1M => 11,
                    10.2M => 12,
                    12.3M => 13,
                    _ => (int)card
                };
            }
        }

        public static IEnumerable<T> Yield<T>(T value)
        {
            yield return value;
        }

        public static IEnumerable<IEnumerable<T>> GetOrderedPermutations<T>(IEnumerable<T> source, int k)
        {
            if (k == 0) return new[] { Enumerable.Empty<T>() };

            int length = source.Count();

            if (k == length) return new[] { source };

            if (k > length) return Enumerable.Empty<IEnumerable<T>>();

            return GetOrderedHelper<T>(source, k, length);
        }

        private static IEnumerable<IEnumerable<T>> GetOrderedHelper<T>(IEnumerable<T> source, int k, int length)
        {
            if (k == 0)
            {
                yield return Enumerable.Empty<T>();
                yield break;
            }
            int i = 0;
            foreach (var item in source)
            {
                if (i + k > length) yield break;
                var permutations = GetOrderedHelper<T>(source.Skip(i + 1), k - 1, length - i);
                i++;

                foreach (var subPerm in permutations)
                {
                    yield return Yield(item).Concat(subPerm);
                }
            }
        }

        private static decimal? ConvertToDecimal(string value, int col)
        {
            if (value == null) return null;

            return value.ToUpper() switch
            {
                "A" => 1,
                "J" => 10.1M,
                "Q" => 10.2M,
                "K" => 10.3M,
                _ => Convert.ToInt32(value),
            };
        }

        private static string ConvertToString(decimal value)
        {
            return value switch
            {
                1 => "A",
                10.1M => "J",
                10.2M => "Q",
                10.3M => "K",
                _ => value.ToString(),
            };
        }


        public void AddToList(string value, int column)
        {
            string pattern = "A2345678910JQK";
            if (!pattern.Contains(value.ToUpper()) || value.Length != 1) return;

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
    }
}
