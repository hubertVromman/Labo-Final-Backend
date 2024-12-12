using BLL.ParserModels;
using BLL.Tools;
using DAL.Repositories;
using Domain.Forms;
using Domain.Models;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace BLL.Services {
  public class RaceService(ResultRepo rer, RunnerRepo rur, RaceRepo rar) {
    public Race AddRaceIfNotExist(Race race) {
      return rar.AddRaceIfNotExist(race);
    }

    public void ParsePDF(string filePath, Race race) {
      using (PdfDocument document = PdfDocument.Open(filePath)) {
        int generalRank = 1;
        int maleRank = 1;
        int femaleRank = 1;
        int resultNumber = 0;
        foreach (Page page in document.GetPages()) {
          var lines = page.GetWords().OrderBy(x => x.BoundingBox.Left).GroupBy(x => x.BoundingBox.Bottom, new ToleranceEqualityComparer());

          List<Line> parsedLines = [];

          foreach (var line in lines) {
            Line newLine = new() {
              Position = line.Key,
            };
            Word[] words = line.ToArray();
            for (int i = 0; i < words.Length; i++) {
              int startMerge = i;
              double position = words[startMerge].BoundingBox.Left;
              while (i < words.Length - 1 && words[i].BoundingBox.Right + 7 > words[i + 1].BoundingBox.Left) {
                i++;
              }
              string text = words[startMerge].Text;
              startMerge++;
              for (; startMerge <= i; startMerge++) {
                text += " " + words[startMerge].Text;
              }
              newLine.Items.Add(new Item() {
                Position = position,
                Text = text,
              });
            }
            parsedLines.Add(newLine);
          }

          for (int i = 0; i < parsedLines.Count; i++) {
            if (parsedLines[i].Items.Count <= 3) {
              parsedLines.RemoveAt(i);
              i--;
            }
          }

          string[] generalRankLabels = ["place", "general", "rang"];
          double generalRankPosition = FindPosition(parsedLines[0].Items, generalRankLabels)
              ?? throw new Exception("Parsing error: GeneralRank not found");

          string[] namesLabels = ["nom prenom"];
          double? namesPosition = FindPosition(parsedLines[0].Items, namesLabels);

          double? lastnamePosition = null;
          double? firstnamePosition = null;
          if (namesPosition is null) {
            string[] lastnameLabels = ["nom"];
            lastnamePosition = FindPosition(parsedLines[0].Items, lastnameLabels)
                 ?? throw new Exception("Parsing error: Lastname and Names not found");
            string[] firstnameLabels = ["prenom"];
            firstnamePosition = FindPosition(parsedLines[0].Items, firstnameLabels)
                 ?? throw new Exception("Parsing error: Firstname and Names not found");
          }

          string[] genderLabels = ["sexe", "mf"];
          double genderPosition = FindPosition(parsedLines[0].Items, genderLabels)
              ?? throw new Exception("Parsing error: Gender not found");

          string[] timeLabels = ["temps"];
          double timePosition = FindPosition(parsedLines[0].Items, timeLabels)
              ?? throw new Exception("Parsing error: Time not found");

          string toDelete = parsedLines[0].Items[0].Text;

          for (int i = 0; i < parsedLines.Count; i++) {
            if (parsedLines[i].Items[0].Text == toDelete) {
              parsedLines.RemoveAt(i);
              foreach (var line in parsedLines) {
                string generalRankShown = line.FindItemByPosition(generalRankPosition)!.Text.Trim().Trim('.').Trim(')');
                string? time = line.FindItemByPosition(timePosition)?.Text;
                string? gender = line.FindItemByPosition(genderPosition)!.Text;
                int? genderRank;
                if (gender.Contains('m') || gender.Contains('M')) {
                  gender = "M";
                  genderRank = maleRank++;
                } else if (gender.Contains('f') || gender.Contains('F')) {
                  gender = "F";
                  genderRank = femaleRank++;
                } else {
                  gender = null;
                  genderRank = null;
                }
                TimeOnly? parsedTime = null;
                Decimal? speed = null;
                string? pace = null;
                if (time is not null && char.ToLower(generalRankShown[0]) != 'd' && char.ToLower(time[0]) != 'd') {
                  if (time.Length == 5)
                    time = $"00:{time}";
                  parsedTime = TimeOnly.Parse(time);
                  speed = (Decimal)race.RealDistance / ((Decimal)parsedTime.Value.Hour + (Decimal)parsedTime.Value.Minute / 60 + (Decimal)parsedTime.Value.Second / 3600);
                  int minutes = (int)(60M / speed);
                  int seconds = (int)(60 * (60 - minutes * speed) / speed);
                  pace = $"{minutes}:{seconds:00}";
                }
                string lastname, firstname;
                if (namesPosition is not null) {
                  string names = line.FindItemByPosition((double)namesPosition)!.Text;
                  lastname = names[..names.LastIndexOf(' ')];
                  firstname = names[names.LastIndexOf(' ')..];
                } else {
                  lastname = line.FindItemByPosition((double)lastnamePosition!)!.Text;
                  firstname = line.FindItemByPosition((double)firstnamePosition!)!.Text;
                }
                ResultForm resultInfo = new() {
                  RaceId = race.RaceId,
                  GeneralRank = generalRank++,
                  GeneralRankShown = generalRankShown,
                  Lastname = lastname,
                  Firstname = firstname,
                  Gender = gender,
                  Time = parsedTime,
                  Speed = speed,
                  Pace = pace,
                  GenderRank = genderRank,
                };

                resultInfo.Firstname = Capitalize(resultInfo.Firstname.Trim().Trim('*'));
                resultInfo.Lastname = Capitalize(resultInfo.Lastname.Trim().Trim('*'));

                resultInfo.RunnerId = AddRunnerIfNotExist(resultInfo);
                rer.AddResult(resultInfo);
                resultNumber++;
              }
            }
            rar.UpdateResultNumber(race.RaceId, resultNumber);
          }
        }
      }
    }

    private double? FindPosition(List<Item> items, string[] labels) {
      return items.Where(i => labels.Contains(i.Text.ToLower().Replace('é', 'e').Replace('è', 'e'))).FirstOrDefault()?.Position;
    }

    private string Capitalize(string input) {
      string result = string.Join("-", input.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries).Select(c => char.ToUpper(c[0]) + c[1..].ToLower()));
      result = string.Join(" ", input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(c => char.ToUpper(c[0]) + c[1..]));
      return result;
    }

    public int AddRunnerIfNotExist(ResultForm resultInfo) {
      Runner? runner = rur.GetRunnerByName(resultInfo.Firstname, resultInfo.Lastname);
      if (runner != null) {
        return runner.RunnerId;
      } else {
        return rur.AddRunner(resultInfo.Firstname, resultInfo.Lastname, resultInfo.Gender).RunnerId;
      }
    }

    public ObjectList<Race> GetByDate(int offset, int limit) {
      return rar.GetByDate(offset, limit);
    }

    public Race GetById(int id) {
      return rar.GetById(id);
    }

    public IEnumerable<Race> Search(string query) {
      return rar.Search(query);
    }
  }
}
