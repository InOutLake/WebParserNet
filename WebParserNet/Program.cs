using AngleSharp.Dom;
using AngleSharp;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebParserNet;
using WebParserNet.Models;

public static class Program
{
    public static void Main()
    {
        bool exit = false;
        bool parsed = false;

            while (!exit)
            {
                Console.Write("1 - View all, 2 - Parse page, 0 - Exit\nEnter the command: ");
                string input = Console.ReadLine();
                Console.WriteLine();
                switch (input)
                {
                case "1":
                    using (var db = new HabrVacancyContext())
                    {
                        foreach (Vacancy v in db.Vacancies.Include("Company"))
                            Console.WriteLine(v.ToString() + "\n");
                    }
                    break;

                case "2":
                    parsed = false;
                    int pagenum = 0;
                    Console.Write("Enter page number: ");
                    while (!int.TryParse(Console.ReadLine(), out pagenum))
                    {
                        Console.WriteLine("Wrong Input!");
                    }
                    try
                    {
                        parsed = ParsePage(pagenum).Result;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Problem occured while parsing. Page number may be too big, or internet connection is unstable.");
                        Console.WriteLine("Ex message: " + e.Message);
                        parsed = true;
                    }
                    while (!parsed) { Task.Delay(1000); }
                    break;
                case "0":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Unknown command");
                    break;
                }
            }
    }
    public static async Task<bool> ParsePage(int pageNumber)
    {
        IConfiguration config = Configuration.Default.WithDefaultLoader();
        IBrowsingContext context = BrowsingContext.New(config);
        IDocument doc = await context.OpenAsync($"https://career.habr.com/vacancies?page={pageNumber}&type=all");


        Vacancy v = new Vacancy();
        var names = doc.QuerySelectorAll("a.vacancy-card__title-link");


        var companies = doc.QuerySelectorAll("div.vacancy-card__company-title");

        var metas = doc.QuerySelectorAll("div.vacancy-card__meta");
        v.Meta = string.Join(", ", metas[0].Children.Select(child => child.TextContent).Where(s => s != " • "));

        var skills = doc.QuerySelectorAll("div.vacancy-card__skills");
        v.Skills = string.Join(", ", skills[0].Children.Select(child => child.TextContent).Where(s => s != " • "));

        using (var db = new HabrVacancyContext())
        {
            db.CheckDatabaseConnection();
            db.GetTableNames();
            List<Company> DbCompanies = db.Companies.ToList();
            List<Vacancy> DbVacancies = db.Vacancies.ToList();
            for (int i = 0; i < names.Length; i++)
            {
                if (DbCompanies.Where(c => c.CompanyName == companies[i].TextContent).Count() == 0)
                {
                    db.Companies.Add(new Company() { CompanyName = companies[i].TextContent });
                    db.SaveChanges();
                }
                if (DbVacancies.Where(v => v.Name == names[i].TextContent &&
                                      v.Company.CompanyName == companies[i].TextContent)
                                      .Count() == 0)
                {
                    db.Vacancies.Add(new Vacancy()
                    {
                        Company = db.Companies.Where(c => c.CompanyName == companies[i].TextContent).FirstOrDefault(),
                        Name = names[i].TextContent,
                        Meta = metas[i].TextContent,
                        Skills = skills[i].TextContent
                    });
                }
            }
            db.SaveChanges();
        }
        return true;
    }
}
