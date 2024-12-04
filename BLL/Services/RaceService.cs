using Azure;
using BLL.ParserModels;
using BLL.Tools;
using DAL.Repositories;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace BLL.Services
{
    public class RaceService(ResultRepo rer, RunnerRepo rur, RaceRepo rar)
    {
        public int AddRace(Race race)
        {
            return rar.AddRace(race);
        }

        public void ParsePDF(string filePath, int raceId)
        {
            using (PdfDocument document = PdfDocument.Open(filePath))
            {
                int generalRank = 1;
                int maleRank = 1;
                int femaleRank = 1;
                foreach (Page page in document.GetPages())
                {
                    var lines = page.GetWords().OrderBy(x => x.BoundingBox.Left).GroupBy(x => x.BoundingBox.Bottom, new ToleranceEqualityComparer());

                    List<Line> parsedLines = [];

                    foreach (var line in lines)
                    {
                        Line newLine = new()
                        {
                            Position = line.Key,
                        };
                        Word[] words = line.ToArray();
                        for (int i = 0; i < words.Length; i++)
                        {
                            int startMerge = i;
                            double position = words[startMerge].BoundingBox.Left;
                            while (i < words.Length - 1 && words[i].BoundingBox.Right + 7 > words[i + 1].BoundingBox.Left)
                            {
                                i++;
                            }
                            string text = words[startMerge].Text;
                            startMerge++;
                            for (; startMerge <= i; startMerge++)
                            {
                                text += " " + words[startMerge].Text;
                            }
                            newLine.Items.Add(new Item()
                            {
                                Position = position,
                                Text = text,
                            });
                        }
                        parsedLines.Add(newLine);
                    }

                    for (int i = 0; i < parsedLines.Count; i++)
                    {
                        if (parsedLines[i].Items.Count <= 3)
                        {
                            parsedLines.RemoveAt(i);
                            i--;
                        }
                    }

                    double generalRankPosition = parsedLines[0].Items.Where(i => i.Text.ToLower() == "place").First()?.Position ?? throw new Exception("Parsing error");
                    double lastnamePosition = parsedLines[0].Items.Where(i => i.Text.ToLower() == "nom").First()?.Position ?? throw new Exception("Parsing error");
                    double firstnamePosition = parsedLines[0].Items.Where(i => i.Text.ToLower().Replace('é', 'e') == "prenom").First()?.Position ?? throw new Exception("Parsing error");
                    double genderPosition = parsedLines[0].Items.Where(i => i.Text.ToLower() == "sexe").First()?.Position ?? throw new Exception("Parsing error");
                    double timePosition = parsedLines[0].Items.Where(i => i.Text.ToLower() == "temps").First()?.Position ?? throw new Exception("Parsing error");

                    string toDelete = parsedLines[0].Items[0].Text;

                    for (int i = 0; i < parsedLines.Count; i++)
                    {
                        if (parsedLines[i].Items[0].Text == toDelete)
                        {
                            parsedLines.RemoveAt(i);
                            i--;
                        }
                    }

                    foreach (var line in parsedLines)
                    {
                        string? time = line.FindItemByPosition(timePosition)?.Text;
                        string? gender = line.FindItemByPosition(genderPosition)!.Text;
                        int? genderRank;
                        if (gender.Contains('m') || gender.Contains('M')) {
                            gender = "m";
                            genderRank = maleRank++;
                        } else if (gender.Contains('f') || gender.Contains('F')) {
                            gender = "f";
                            genderRank = femaleRank++;
                        } else {
                            gender = null;
                            genderRank = null;
                        }
                        ResultInfo resultInfo = new()
                        {
                            RaceId = raceId,
                            GeneralRank = generalRank++,
                            GeneralRankShown = line.FindItemByPosition(generalRankPosition)!.Text,
                            Lastname = line.FindItemByPosition(lastnamePosition)!.Text,
                            Firstname = line.FindItemByPosition(firstnamePosition)!.Text,
                            Gender = gender,
                            Time = time is not null ? TimeOnly.Parse(time) : null,
                            GenderRank = genderRank,
                        };

                        resultInfo.RunnerId = AddRunnerIfNotExist(resultInfo);
                        rer.AddResult(resultInfo);
                    }
                }
            }
        }

        public int AddRunnerIfNotExist(ResultInfo resultInfo)
        {
            Runner? runner = rur.GetRunnerByName(resultInfo.Firstname, resultInfo.Lastname);
            if (runner != null)
            {
                return runner.RunnerId;
            }
            else
            {
                return rur.AddRunner(resultInfo.Firstname, resultInfo.Lastname, resultInfo.Gender).RunnerId;
            }
        }
    }
}
